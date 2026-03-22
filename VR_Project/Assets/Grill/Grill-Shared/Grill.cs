using UnityEngine;

public class Grill : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();

        if (patty != null)
        {
            patty.IsOnGrill = true;
            Debug.Log("Patty placed on grill");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();

        if (patty != null)
        {
            patty.IsOnGrill = false;
            Debug.Log("Patty removed from grill");
        }
    }
}