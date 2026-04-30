using System.Runtime.CompilerServices;
using System.Security;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class Presser : MonoBehaviour
{
    // Reference to the patty currently inside the trigger zone
    // private Patty currentPatty;
    private List<Patty> allPattiesOnGrill = new List<Patty>(); // List of patties to handle multiple patties

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    }

    private void OnDisable()
    {
        audioManager?.StopGrillLoop();
    }

    // Called when a collider enters the trigger area
    private void OnTriggerEnter(Collider other)
    {
        Patty patty = other.GetComponent<Patty>();

        if (patty != null && !allPattiesOnGrill.Contains(patty))
        {
            allPattiesOnGrill.Add(patty);
            Debug.Log("Patty entered presser zone");
        }
    }

    // Called when a collider exits the trigger area
    private void OnTriggerExit(Collider other)
    {
         Patty patty = other.GetComponent<Patty>();

        if (patty != null)
        {
            allPattiesOnGrill.Remove(patty);
            Debug.Log("Patty left presser zone");
        }
    }

    // Called every frame
    private void Update()
    {
        bool isGrilling = false;

        // Apply heat to all patties that are inside the zone
        foreach (Patty patty in allPattiesOnGrill) 
        {
            if (patty != null && patty.IsOnGrill)
            {
                patty.ApplyHeat(Time.deltaTime);
                isGrilling = true;
            }    
        }

        if (isGrilling)
        {
            audioManager?.PlayGrillLoop();
        }
        else
        {
            audioManager?.StopGrillLoop();
        }
    }
}
