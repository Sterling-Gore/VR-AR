using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrillHinge : MonoBehaviour
{
    [SerializeField] private Rigidbody grillHingeRb; 
    [SerializeField] private XRGrabInteractable xrGrab;
    private bool grabbed = false;

    void Start()
    {
        grabbed = false;
        if(grillHingeRb == null)
            grillHingeRb = GetComponent<Rigidbody>();
        grillHingeRb.isKinematic = true;
    }

    void Update()
    {
        if(grabbed != xrGrab.isSelected)
        {
            grabbed = !grabbed;
            grillHingeRb.isKinematic = !grabbed;
        }
    }
}
