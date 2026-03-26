using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SqueezeDetector : MonoBehaviour
{
  [SerializeField] private XRGrabInteractable xrGrab;
  [SerializeField] private ParticleSystem condimentSpray;
  [SerializeField] float rayRange = 10f;
  [SerializeField] CondimentData condimentData;
  [SerializeField] Transform nozzle;
  [SerializeField] InputActionReference rightControllerTrigger;
  [SerializeField] InputActionReference leftControllerTrigger;
  private int mask; 
  private bool LeftControllerUsed = false;
  private bool rightControllerUsed = false;

//---------------------------------------------------------------//
/*                      Unity Functions                          */
  private void Awake()
  {
    mask = ~LayerMask.GetMask("snapCollider");
    if (condimentData == null)
      condimentData = GetComponent<CondimentData>();
  }

  private void Start()
  {
    rightControllerTrigger.action.started += RightSprayPressed;
    leftControllerTrigger.action.started += LeftSprayPressed;
    xrGrab.selectEntered.AddListener(OnGrabbed);
    xrGrab.selectExited.AddListener(OnReleased);
  }

//---------------------------------------------------------------//
/*                      Private Functions                        */
  private void RightSprayPressed(InputAction.CallbackContext context)
  {
    if (rightControllerUsed)
      SprayCondiment();
  }

  private void LeftSprayPressed(InputAction.CallbackContext context)
  {
    if (LeftControllerUsed)
      SprayCondiment();
  }

  private void OnGrabbed(SelectEnterEventArgs args)
  {
    IXRSelectInteractor interactor = args.interactorObject;
    
    if (interactor.transform.CompareTag("LeftHand"))
    {
        LeftControllerUsed = true;
    }
    else if (interactor.transform.CompareTag("RightHand"))
    {
        rightControllerUsed = true;
    }
  }

  private void OnReleased(SelectExitEventArgs args)
  {
    IXRSelectInteractor interactor = args.interactorObject;

    if (interactor.transform.CompareTag("LeftHand"))
    {
        LeftControllerUsed = false;
    }
    else if (interactor.transform.CompareTag("RightHand"))
    {
        rightControllerUsed = false;
    }
  }

  private void SprayCondiment()
  {
    condimentSpray.Play();
    Ray ray = new Ray(nozzle.position, nozzle.forward);
    RaycastHit hit;
    // Debug.DrawRay(ray.origin, ray.direction * rayRange, Color.green);
    if (Physics.Raycast(ray, out hit, rayRange, mask))
      {
        // make sure the condiment collider is facing the user
        Vector3 normal = hit.normal;
        Vector3 rayDir = ray.direction;
        float directionalDotProduct = Vector3.Dot(rayDir, normal);
        if (directionalDotProduct < 0f)
        {
          CondimentIngredient receiver = hit.collider.GetComponent<CondimentIngredient>();

          if (receiver != null)
          {
            receiver.SetCondiment(condimentData.GetName(), condimentData.GetColor());
          }
        }
      }
  }

}
