using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LeftFridgeDoorSnap : MonoBehaviour
{
    public HingeJoint doorHinge;
    public XRGrabInteractable grabInteractable;
    public FridgeRestocker fridgeRestocker;
    public float closedAngle = 0f;
    public float openAngle = -80f;

    // -1 for the left door
    public float angleSign = 1f;

    public float snapSpeed = 240f;
    public float exactSnapTolerance = 2f;
    public float closedTolerance = 2f;

    private Rigidbody rb;

    private Quaternion closedLocalRotation;
    private bool autoSnapping = false;
    private float targetLogicalAngle = 0f;

    private bool waitingToRestock = false;
    private bool hasRestockedThisClose = false;

    private void Start()
    {
        rb = doorHinge.GetComponent<Rigidbody>();
        doorHinge.useSpring = false;

        // Save the exact closed pose from the starting scene setup
        closedLocalRotation = transform.localRotation;

        if (rb != null)
        {
            rb.isKinematic = true; // start locked in place
        }
    }

    private void FixedUpdate()
    {
        if (grabInteractable != null && grabInteractable.isSelected)
            return;

        if (!autoSnapping || rb == null)
            return;

        Quaternion targetLocalRotation = GetTargetLocalRotation(targetLogicalAngle);
        Quaternion targetWorldRotation = GetTargetWorldRotation(targetLocalRotation);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Quaternion nextRotation = Quaternion.RotateTowards(
            rb.rotation,
            targetWorldRotation,
            snapSpeed * Time.fixedDeltaTime
        );

        rb.MoveRotation(nextRotation);

        float currentLogicalAngle = GetLogicalAngle();
        float remaining = Mathf.Abs(Mathf.DeltaAngle(currentLogicalAngle, targetLogicalAngle));

        if (remaining <= exactSnapTolerance)
        {
            rb.MoveRotation(targetWorldRotation);
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            autoSnapping = false;
            rb.isKinematic = true; // lock it exactly in place

            Debug.Log($"[LEFT LOCKED] LogicalAngle={targetLogicalAngle}", this);

            if (Mathf.Abs(Mathf.DeltaAngle(targetLogicalAngle, closedAngle)) <= closedTolerance)
            {
                if (!hasRestockedThisClose && fridgeRestocker != null)
                {
                    hasRestockedThisClose = true;
                    waitingToRestock = false;
                    fridgeRestocker.RestockAll();
                    Debug.Log("[LEFT] Door fully closed -> Restocking", this);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        autoSnapping = false;
        waitingToRestock = false;
        hasRestockedThisClose = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        doorHinge.useSpring = false;

        Debug.Log($"[LEFT GRABBED] LogicalAngle={GetLogicalAngle()} RawLocalX={GetRawLocalAngle()}", this);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        float currentLogicalAngle = GetLogicalAngle();

        float distanceToClosed = Mathf.Abs(Mathf.DeltaAngle(currentLogicalAngle, closedAngle));
        float distanceToOpen = Mathf.Abs(Mathf.DeltaAngle(currentLogicalAngle, openAngle));

        targetLogicalAngle = (distanceToClosed <= distanceToOpen) ? closedAngle : openAngle;

        autoSnapping = true;
        waitingToRestock = Mathf.Abs(Mathf.DeltaAngle(targetLogicalAngle, closedAngle)) <= 0.01f;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"[LEFT RELEASED] CurrentLogical={currentLogicalAngle} TargetLogical={targetLogicalAngle}", this);
    }

    // Reads current local X rotation relative to the starting closed pose
    private float GetRawLocalAngle()
    {
        Quaternion relative = Quaternion.Inverse(closedLocalRotation) * transform.localRotation;
        float x = relative.eulerAngles.x;

        if (x > 180f)
            x -= 360f;

        return x;
    }

    // Converts runtime/raw angle into your intended logic:
    // closed = 0, open = -80
    private float GetLogicalAngle()
    {
        return GetRawLocalAngle() * angleSign;
    }

    // Builds the exact target rotation for the chosen logical angle
    private Quaternion GetTargetLocalRotation(float logicalAngle)
    {
        float rawAngle = logicalAngle * angleSign;
        return closedLocalRotation * Quaternion.AngleAxis(rawAngle, Vector3.right);
    }

    private Quaternion GetTargetWorldRotation(Quaternion targetLocalRotation)
    {
        if (transform.parent != null)
            return transform.parent.rotation * targetLocalRotation;

        return targetLocalRotation;
    }
}