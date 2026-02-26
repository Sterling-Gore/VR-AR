using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine;

public class Presser : MonoBehaviour
{
    private Patty currentPatty;

    private void OnTriggerEnter(Collider other)
{
    Debug.Log("Trigger hit by: " + other.name);
}

    private void OnTriggerExit(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();
        if (patty != null && patty == currentPatty)
        {
            currentPatty = null;
        }
    }

    private void Update()
    {
        if (currentPatty != null)
        {
            currentPatty.ApplyHeat(Time.deltaTime);
        }
    }
}