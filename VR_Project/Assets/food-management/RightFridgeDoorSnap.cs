using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RightFridgeDoorSnap : MonoBehaviour
{
    public HingeJoint doorHinge;
    public XRGrabInteractable grabInteractable;

    public float closedAngle = 0f;
    public float openAngle = 78f;

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

        Debug.Log($"[RIGHT GRABBED] CurrentAngle={doorHinge.angle}", this);
    }

    // private void OnReleased(SelectExitEventArgs args)
    // {
    //     float currentAngle = doorHinge.angle;

    //     if (float.IsNaN(currentAngle) || float.IsInfinity(currentAngle))
    //     {
    //         Debug.LogWarning("[RIGHT] Invalid hinge angle detected. Snapping closed.", this);
    //         currentAngle = closedAngle;
    //     }

    //     float distanceToClosed = Mathf.Abs(Mathf.DeltaAngle(currentAngle, closedAngle));
    //     float distanceToOpen = Mathf.Abs(Mathf.DeltaAngle(currentAngle, openAngle));

    //     float targetAngle;

    //     if (distanceToOpen <= openTolerance)
    //     {
    //         targetAngle = openAngle;
    //     }
    //     else if (distanceToClosed <= closedTolerance)
    //     {
    //         targetAngle = closedAngle;
    //     }
    //     else
    //     {
    //         targetAngle = distanceToClosed < distanceToOpen ? closedAngle : openAngle;
    //     }

    //     JointSpring spring = doorHinge.spring;
    //     spring.spring = snapSpring;
    //     spring.damper = snapDamper;
    //     spring.targetPosition = targetAngle;

    //     doorHinge.spring = spring;
    //     doorHinge.useSpring = true;

    //     Debug.Log($"[RIGHT RELEASED] Current={currentAngle} Target={targetAngle}", this);
    // }

    private void OnReleased(SelectExitEventArgs args)
    {
        doorHinge.useSpring = false;
        Debug.Log($"[RIGHT RELEASED TEST] No spring applied. Current={doorHinge.angle}", this);
    }
}