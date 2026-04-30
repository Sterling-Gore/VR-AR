using UnityEngine;

public class Grill : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;
    private int pattyCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();

        if (patty != null)
        {
            patty.IsOnGrill = true;
            pattyCount++;

            Debug.Log("Patty placed on grill");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();

        if (patty != null)
        {
            patty.IsOnGrill = false;
            pattyCount = Mathf.Max(0, pattyCount - 1);

            Debug.Log("Patty removed from grill");
        }
    }
}