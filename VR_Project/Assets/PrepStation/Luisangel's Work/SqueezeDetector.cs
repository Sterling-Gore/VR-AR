using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR;
using UnityEngine;
using System.Collections.Generic;

public class SqueezeDetector : MonoBehaviour
{
  [SerializeField] private string inputName = "Fire1";  // Input axis/button name when gated
  [SerializeField] private XRGrabInteractable xrGrab;
  [SerializeField] private ParticleSystem condimentSpray;
  [SerializeField] float rayRange = 10f;
  [SerializeField] CondimentData condimentData;
  [SerializeField] Transform nozzle;
  private int mask; 
  List<InputDevice> leftDevices = new List<InputDevice>();
  List<InputDevice> rightDevices = new List<InputDevice>();

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
    if (!xrGrab.isSelected)
            return;

    var interactor = xrGrab.firstInteractorSelecting;
    if (interactor == null)
        return;

    bool isLeftHand = interactor.transform.name.ToLower().Contains("left");
    bool isRightHand = interactor.transform.name.ToLower().Contains("right");
    InputDevice device = default;

    if (isLeftHand)
    {
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Left | InputDeviceCharacteristics.Controller,
            leftDevices
        );

        if (leftDevices.Count > 0)
            device = leftDevices[0];
    }
    else if (isRightHand)
    {
        InputDevices.GetDevicesWithCharacteristics(
            InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller,
            rightDevices
        );

        if (rightDevices.Count > 0)
            device = rightDevices[0];
    }

    if (!device.isValid)
        return;

    float triggerValue;
    if (device.TryGetFeatureValue(CommonUsages.trigger, out triggerValue)
        && triggerValue > 0.5f)
    {
        SprayCondiment();
    }
    // if (Input.GetButtonDown(inputName) && xrGrab.isSelected)
    // {
    //   SprayCondiment();
    // }
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
