using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FridgeDoorSnap : MonoBehaviour
{
    public HingeJoint hingeJoint;
    public XRGrabInteractable grabInteractable;

    public float closedAngle = 0f;
    public float openAngle = 80f;

    public float snapSpring = 200f;
    public float snapDamper = 20f;

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
        hingeJoint.useSpring = false;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        float currentAngle = hingeJoint.angle;

        float distanceToClosed = Mathf.Abs(currentAngle - closedAngle);
        float distanceToOpen = Mathf.Abs(currentAngle - openAngle);

        float targetAngle = distanceToClosed < distanceToOpen ? closedAngle : openAngle;

        JointSpring spring = hingeJoint.spring;
        spring.spring = snapSpring;
        spring.damper = snapDamper;
        spring.targetPosition = targetAngle;

        hingeJoint.spring = spring;
        hingeJoint.useSpring = true;
    }
}