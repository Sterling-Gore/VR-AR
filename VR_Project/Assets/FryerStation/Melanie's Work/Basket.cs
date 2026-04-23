using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Basket : MonoBehaviour
{
    public bool InFryer = false;
    public bool HasLockedFry => lockedFry != null;

    [Header("Fry Placement")]
    [SerializeField] private Transform frySnapPoint;
    [SerializeField] private Vector3 fryLockedLocalOffset = new Vector3(0f, 0.03f, 0f);

    [Header("Basket Visuals")]
    [SerializeField] private float occupiedAlpha = 0.2f;

    [Header("Input")]
    [SerializeField] private XRGrabInteractable basketGrabInteractable;
    [SerializeField] private InputActionReference rightControllerDisconnectAction;
    [SerializeField] private InputActionReference leftControllerDisconnectAction;

    private FryItem lockedFry;
    private FryItem fryInTrigger;
    private Material basketMaterial;
    private Color basketBaseColor = Color.white;
    private bool leftControllerHoldingBasket;
    private bool rightControllerHoldingBasket;
    private float lastToggleTime = -1f;
    private float toggleCooldownDuration = 0.3f;

    private void Awake()
    {
        if (basketGrabInteractable == null)
        {
            basketGrabInteractable = GetComponent<XRGrabInteractable>();
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            basketMaterial = renderer.material;
            basketBaseColor = basketMaterial.color;
            SetBasketAlpha(1f);
        }
    }

    private void OnEnable()
    {
        RegisterGrabCallbacks();
        RegisterDisconnectInputCallbacks();
    }

    private void OnDisable()
    {
        UnregisterGrabCallbacks();
        UnregisterDisconnectInputCallbacks();
        leftControllerHoldingBasket = false;
        rightControllerHoldingBasket = false;
    }

    private void Update()
    {
        if (InFryer)
        {
            // Debug.Log("Basket is frying!");

            if (lockedFry != null)
            {
                lockedFry.ApplyHeat(Time.deltaTime);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        FryItem fry = other.GetComponentInParent<FryItem>();
        if (fry == null)
        {
            return;
        }

        fryInTrigger = fry;
        Debug.Log("Fry entered basket trigger. Press trigger to lock.");
    }

    private void OnTriggerExit(Collider other)
    {
        FryItem fry = other.GetComponentInParent<FryItem>();
        if (fry != null && fryInTrigger == fry)
        {
            fryInTrigger = null;
            Debug.Log("Fry exited basket trigger.");
        }
    }

    private void LockFryInBasket(FryItem fry)
    {
        lockedFry = fry;

        // 1. Disable Grab Interactable
        // XRGrabInteractable remembers the object's original physics state. Disabling it 
        // causes it to restore that state (re-enabling gravity). We must alter physics after.
        XRGrabInteractable grabInteractable = fry.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        // 2. disable physics
        Rigidbody fryBody = fry.GetComponent<Rigidbody>();
        if (fryBody != null)
        {
            fryBody.linearVelocity = Vector3.zero;
            fryBody.angularVelocity = Vector3.zero;
            fryBody.useGravity = false;
            fryBody.isKinematic = true;
        }

        // disable collisions to prevent clipping/vibrating against the basket or other objects
        Collider[] fryColliders = fry.GetComponentsInChildren<Collider>();
        foreach (Collider col in fryColliders)
        {
            col.enabled = false;
        }

        Transform snapTarget = frySnapPoint != null ? frySnapPoint : transform;
        fry.transform.SetParent(snapTarget, true);

        if (frySnapPoint != null)
        {
            fry.transform.localPosition = Vector3.zero;
            fry.transform.localRotation = Quaternion.identity;
        }
        else
        {
            fry.transform.localPosition = fryLockedLocalOffset;
            fry.transform.localRotation = Quaternion.identity;
        }

        SetBasketAlpha(occupiedAlpha);
    }

    [ContextMenu("Release Fry")]
    public void ReleaseLockedFry()
    {
        if (lockedFry == null)
        {
            return;
        }

        XRGrabInteractable grabInteractable = lockedFry.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = true;
        }

        Rigidbody fryBody = lockedFry.GetComponent<Rigidbody>();
        if (fryBody != null)
        {
            fryBody.isKinematic = false;
            fryBody.useGravity = true;
        }

        Collider[] fryColliders = lockedFry.GetComponentsInChildren<Collider>();
        foreach (Collider col in fryColliders)
        {
            col.enabled = true;
        }

        lockedFry.transform.SetParent(null, true);
        lockedFry = null;

        SetBasketAlpha(1f);
    }

    public void TrashLockedFry()
    {
        if (lockedFry == null)
        {
            return;
        }

        FryItem fryToDestroy = lockedFry;
        ReleaseLockedFry();
        Destroy(fryToDestroy.gameObject);
    }

    private void SetBasketAlpha(float alpha)
    {
        if (basketMaterial == null)
        {
            return;
        }

        Color color = basketBaseColor;
        color.a = Mathf.Clamp01(alpha);
        basketMaterial.color = color;
    }

    private void RegisterGrabCallbacks()
    {
        if (basketGrabInteractable == null)
        {
            return;
        }

        basketGrabInteractable.selectEntered.AddListener(OnBasketGrabbed);
        basketGrabInteractable.selectExited.AddListener(OnBasketReleased);
    }

    private void UnregisterGrabCallbacks()
    {
        if (basketGrabInteractable == null)
        {
            return;
        }

        basketGrabInteractable.selectEntered.RemoveListener(OnBasketGrabbed);
        basketGrabInteractable.selectExited.RemoveListener(OnBasketReleased);
    }

    private void OnBasketGrabbed(SelectEnterEventArgs args)
    {
        IXRSelectInteractor interactor = args.interactorObject;
        Transform interactorTransform = interactor != null ? interactor.transform : null;
        string tag = interactorTransform != null ? interactorTransform.tag : string.Empty;
        string name = interactorTransform != null ? interactorTransform.name.ToLowerInvariant() : string.Empty;

        bool isLeft = tag == "LeftHand" || name.Contains("left");
        bool isRight = tag == "RightHand" || name.Contains("right");

        leftControllerHoldingBasket = isLeft;
        rightControllerHoldingBasket = isRight;
    }

    private void OnBasketReleased(SelectExitEventArgs args)
    {
        leftControllerHoldingBasket = false;
        rightControllerHoldingBasket = false;
    }

    private void RegisterDisconnectInputCallbacks()
    {
        if (rightControllerDisconnectAction != null && rightControllerDisconnectAction.action != null)
        {
            rightControllerDisconnectAction.action.started += OnRightDisconnectPressed;
        }

        if (leftControllerDisconnectAction != null && leftControllerDisconnectAction.action != null)
        {
            leftControllerDisconnectAction.action.started += OnLeftDisconnectPressed;
        }
    }

    private void UnregisterDisconnectInputCallbacks()
    {
        if (rightControllerDisconnectAction != null && rightControllerDisconnectAction.action != null)
        {
            rightControllerDisconnectAction.action.started -= OnRightDisconnectPressed;
        }

        if (leftControllerDisconnectAction != null && leftControllerDisconnectAction.action != null)
        {
            leftControllerDisconnectAction.action.started -= OnLeftDisconnectPressed;
        }
    }

    private void OnRightDisconnectPressed(InputAction.CallbackContext context)
    {
        if (ShouldReleaseForHand(true))
        {
            Debug.Log("Right trigger: toggle lock/unlock");
            ToggleFryLock();
        }
    }

    private void OnLeftDisconnectPressed(InputAction.CallbackContext context)
    {
        if (ShouldReleaseForHand(false))
        {
            Debug.Log("Left trigger: toggle lock/unlock");
            ToggleFryLock();
        }
    }

    private void ToggleFryLock()
    {
        // Check cooldown
        if (Time.time - lastToggleTime < toggleCooldownDuration)
        {
            return;
        }

        lastToggleTime = Time.time;

        if (lockedFry != null)
        {
            // Already locked, so unlock
            ReleaseLockedFry();
            Debug.Log("Fry unlocked");
        }
        else if (fryInTrigger != null)
        {
            // Not locked but fry is in trigger, so lock it
            LockFryInBasket(fryInTrigger);
            Debug.Log("Fry locked");
        }
        else
        {
            Debug.Log("No fry to lock or unlock.");
        }
    }

    private bool ShouldReleaseForHand(bool isRightHand)
    {
        // Allow toggle if hand is holding basket or if there's a fry available
        if (isRightHand && rightControllerHoldingBasket)
        {
            return true;
        }

        if (!isRightHand && leftControllerHoldingBasket)
        {
            return true;
        }

        // Also allow if basket is selected by the hand, even if we're not sure which hand
        if (basketGrabInteractable != null && basketGrabInteractable.isSelected)
        {
            return true;
        }

        return false;
    }
}