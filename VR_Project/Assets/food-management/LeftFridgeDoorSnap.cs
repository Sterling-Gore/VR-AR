using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LeftFridgeDoorSnap : MonoBehaviour
{
    public HingeJoint doorHinge;
    public XRGrabInteractable grabInteractable;
    public FridgeRestocker fridgeRestocker;

    public float closedAngle = -78f;
    public float openAngle = 0f;

    public float snapSpring = 50f;
    public float snapDamper = 12f;

    public float openTolerance = 5f;
    public float closedTolerance = 5f;

    private Rigidbody rb;

    private void Start()
    {
        rb = doorHinge.GetComponent<Rigidbody>();
        doorHinge.useSpring = false;
    }

    private void Update()
    {
        if (grabInteractable != null && grabInteractable.isSelected)
        {
            doorHinge.useSpring = false;
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
        doorHinge.useSpring = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Debug.Log($"[LEFT GRABBED] CurrentAngle={doorHinge.angle}", this);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        float currentAngle = doorHinge.angle;

        float distanceToClosed = Mathf.Abs(Mathf.DeltaAngle(currentAngle, closedAngle));

        Debug.Log($"[LEFT RELEASED] Angle={currentAngle} DistanceToClosed={distanceToClosed}");

        // Only restock if actually closed
        if (distanceToClosed <= closedTolerance)
        {
            if (fridgeRestocker != null)
            {
                fridgeRestocker.RestockAll();
                Debug.Log("[LEFT] Fridge CLOSED → Restocking");
            }
        }
    }
}