using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine;

public class SqueezeDetector : MonoBehaviour
{
  [SerializeField] private string inputName = "Fire1";  // Input axis/button name when gated
  [SerializeField] private XRGrabInteractable xrGrab;
  [SerializeField] private ParticleSystem condimentSpray;
  [SerializeField] float rayRange = 10f;
  [SerializeField] CondimentData condimentData;
  [SerializeField] Transform nozzle;
  private int mask; 

//---------------------------------------------------------------//
/*                      Unity Functions                          */
  private void Awake()
  {
    mask = ~LayerMask.GetMask("snapCollider");
    if (condimentData == null)
      condimentData = GetComponent<CondimentData>();
  }

  private void Update()
  {
    if (Input.GetButtonDown(inputName) && xrGrab.isSelected)
    {
      SprayCondiment();
    }
  }

//---------------------------------------------------------------//
/*                      Private Functions                        */
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
            Debug.Log("INGREDIENT FOUND");
            receiver.SetCondiment(condimentData.GetName(), condimentData.GetColor());
          }
        }
      }
  }

}
