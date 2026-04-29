using UnityEngine;

public class Grill : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    private int pattyCount = 0;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();

        if (patty != null)
        {
            patty.IsOnGrill = true;
            pattyCount++;
            if (pattyCount == 1)
            {
                audioManager?.PlayGrillLoop();
            }
            Debug.Log("Patty placed on grill");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();

        if (patty != null)
        {
            patty.IsOnGrill = false;
            pattyCount--;
            if (pattyCount == 0)
            {
                audioManager?.StopGrillLoop();
            }
            Debug.Log("Patty removed from grill");
        }
    }
}