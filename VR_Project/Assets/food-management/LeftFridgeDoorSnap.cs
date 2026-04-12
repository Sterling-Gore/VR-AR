using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LeftFridgeDoorSnap : MonoBehaviour
{
    public HingeJoint doorHinge;
    public XRGrabInteractable grabInteractable;

    public float closedAngle = 0f;
    public float openAngle = -80f;

    public float snapSpring = 200f;
    public float snapDamper = 20f;

    private void Start()
    {
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
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        float currentAngle = doorHinge.angle;

        float distanceToClosed = Mathf.Abs(Mathf.DeltaAngle(currentAngle, closedAngle));
        float distanceToOpen = Mathf.Abs(Mathf.DeltaAngle(currentAngle, openAngle));

        float targetAngle = distanceToClosed < distanceToOpen ? closedAngle : openAngle;

        JointSpring spring = doorHinge.spring;
        spring.spring = snapSpring;
        spring.damper = snapDamper;
        spring.targetPosition = targetAngle;

        doorHinge.spring = spring;
        doorHinge.useSpring = true;
    }
}