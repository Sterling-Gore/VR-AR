using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class Basket : MonoBehaviour
{
    public bool InFryer = false;

    [Header("Fry Placement")]
    [SerializeField] private Transform frySnapPoint;
    [SerializeField] private Vector3 fryLockedLocalOffset = new Vector3(0f, 0.03f, 0f);

    [Header("Basket Visuals")]
    [SerializeField] private float occupiedAlpha = 0.2f;

    private FryItem lockedFry;
    private Material basketMaterial;
    private Color basketBaseColor = Color.white;

    private void Awake()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            basketMaterial = renderer.material;
            basketBaseColor = basketMaterial.color;
            SetBasketAlpha(1f);
        }
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
        if (lockedFry != null)
        {
            return;
        }

        FryItem fry = other.GetComponentInParent<FryItem>();
        if (fry == null)
        {
            return;
        }

        LockFryInBasket(fry);
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
}