using UnityEngine;

public class SqueezeDetector : MonoBehaviour
{
  [SerializeField] private float pourThreshold = -45f; // Degrees from upright before spraying
  [SerializeField] private Transform nozzle;           // Where the spray should originate
  [SerializeField] private ParticleSystem sprayPrefab; // Condiment spray prefab
  [SerializeField] private bool requireInput = false;  // If true, gate spray behind an input
  [SerializeField] private string inputName = "Fire1";  // Input axis/button name when gated

  private ParticleSystem activeSpray;
  private bool isSpraying;

  private void Awake()
  {
    if (nozzle == null)
    {
      nozzle = transform;
    }
  }

  private void Update()
  {
    bool pastAngle = CalculatePourAngle() >= pourThreshold;
    bool inputOk = !requireInput || Input.GetButton(inputName);
    bool shouldSpray = pastAngle && inputOk;

    if (shouldSpray && !isSpraying)
    {
      StartSpray();
    }
    else if (!shouldSpray && isSpraying)
    {
      StopSpray();
    }

    if (isSpraying && activeSpray != null)
    {
      activeSpray.transform.SetPositionAndRotation(nozzle.position, nozzle.rotation);
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

    if (activeSpray == null)
    {
      activeSpray = Instantiate(sprayPrefab, nozzle.position, nozzle.rotation);
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
  }

  private float CalculatePourAngle()
  {
    return Vector3.Angle(transform.up, Vector3.up);
  }

  private void OnDisable()
  {
    StopSpray();
  }
}