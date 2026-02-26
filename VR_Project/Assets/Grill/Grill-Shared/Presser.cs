using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine;

public class Presser : MonoBehaviour
{
    // Reference to the patty currently inside the trigger zone
    private Patty currentPatty;

    // Called when a collider enters the trigger area
    private void OnTriggerEnter(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();

        if (patty != null)
        {
            currentPatty = patty;
            Debug.Log("Patty entered presser zone");
        }
    }

    // Called when a collider exits the trigger area
    private void OnTriggerExit(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();

        if (patty != null && patty == currentPatty)
        {
            currentPatty = null;
            Debug.Log("Patty left presser zone");
        }
    }

    // Called every frame
    private void Update()
    {

        // Only apply heat if a patty is currently inside
        if (currentPatty != null)
        {
            currentPatty.ApplyHeat(Time.deltaTime);
        }
    }
}