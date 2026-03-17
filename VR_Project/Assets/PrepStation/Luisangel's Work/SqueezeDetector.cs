using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine;

public class SqueezeDetector : MonoBehaviour
{
  [SerializeField] private float pourThreshold = -45f; // Degrees from upright before spraying
  [SerializeField] private Transform nozzle;           // Where the spray should originate
  [SerializeField] private ParticleSystem sprayPrefab; // Condiment spray prefab
  [SerializeField] private bool requireInput = false;  // If true, gate spray behind an input
  [SerializeField] private string inputName = "Fire1";  // Input axis/button name when gated
  [SerializeField] private float sprayDuration = 1f;
  [SerializeField] private XRGrabInteractable xrGrab;

  private ParticleSystem activeSpray;
  private bool isSpraying;
  private float sprayTimer;

  List<ParticleCollisionEvent> collisionEvents;

  private void Awake()
  {
    if (nozzle == null)
    {
      nozzle = transform;
    }
  }

  private void Start()
  {
    collisionEvents = new List<ParticleCollisionEvent>();
  }

  private void Update()
  {
    bool heldOk = xrGrab != null && xrGrab.isSelected;
    bool pastAngle = CalculatePourAngle() >= pourThreshold;
    bool inputPressed = Input.GetButtonDown(inputName);
    bool inputOk = !requireInput || inputPressed;
    bool shouldSpray = heldOk && pastAngle && inputOk;

    if (shouldSpray && !isSpraying)
    {
      StartSpray();
    }

    if (isSpraying)
    {
      sprayTimer -= Time.deltaTime;
      if (sprayTimer <= 0)
      {
        StopSpray();
      }

      if (activeSpray != null)
      {
        activeSpray.transform.SetPositionAndRotation(nozzle.position, nozzle.rotation);
      }
    }
  }

  private void StartSpray()
  {
    if (sprayPrefab == null)
    {
      Debug.LogError("SqueezeDetector: sprayPrefab is not assigned; cannot start spray.");
      return;
    }

    isSpraying = true;
    sprayTimer = sprayDuration;

    if (activeSpray == null)
    {
      activeSpray = Instantiate(sprayPrefab, nozzle.position, nozzle.rotation);
    } 
    else
    {
      activeSpray.Clear();
    }

    var emission = activeSpray.emission;
    emission.enabled = true;
    if (!activeSpray.isPlaying)
    {
      activeSpray.Play();
    }
  }

  private void StopSpray()
  {
    isSpraying = false;

    if (activeSpray == null)
    {
      return;
    }

    var emission = activeSpray.emission;
    emission.enabled = false;

    Destroy(activeSpray.gameObject, 1f);
    activeSpray = null;
  }

  private float CalculatePourAngle()
  {
    return Vector3.Angle(transform.up, Vector3.up);
  }

  private void OnDisable()
  {
    StopSpray();
  }
/*
  private void OnParticleCollision(GameObject other)
  {
    // stackableIngredient object will be used here to communicate
    if (ingredient == null) return;

    StartCoroutine(IngredientHitDelayed(ingredient, 0.3f));
  }

  private IEnumerator IngredientHitDelayed(stackableIngredient ingredient, float delay)
  {
    yield return new WaitForSeconds(delay);

    // this line works assuming an ingredient clas exists
    Debug.Log("Condiment hit ingredient: " + ingredient.ingredientName);

    // add more gameplay logic
  }*/
}
