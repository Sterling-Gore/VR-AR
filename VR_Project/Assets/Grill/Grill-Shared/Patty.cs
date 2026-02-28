using System.Collections.Generic;
using UnityEngine;

// Defines all possible cooking states of the patty
public enum PattyState
{
    Raw,
    Undercooked,
    Cooked,
    Overcooked
}

[System.Serializable]
public struct CookingThreshold
{
    public PattyState state;
    public float maxProgress;
    public Color stateColor;
}

/*
Patty was refactored to follow both SRP and OCP. Cooked states are now moved to the actual objects, 
this way we dont have to modify code for when we want more states or want to change the timing thresholds.
*/

public class Patty : MonoBehaviour
{
    // How fast the patty cooks while being pressed
    [Header("Cooking Settings")]
    [SerializeField] private float heatPerSecond = 5f;
    [SerializeField] private float maxProgress = 100f;

    // Ties logic to UI, previous iteration violated the Open closed Principle
    [Header("State Configuration")]
    [SerializeField] private List<CookingThreshold> cookingThresholds = new List<CookingThreshold>();

    // Internal State Tracking
    private float cookingProgress = 0f;
    private PattyState currentState = PattyState.Raw;

    // Public accessors for UI or other scripts
    public PattyState State => currentState;
    public float CookingProgress => cookingProgress;

    private void Start()
    {
        UpdateVisuals(); // Sets color to be pink by default
    }

    // Called by Presser
    // increases cooking progress while the patty is being pressed
    public void ApplyHeat(float deltaTime)
    {
        if (currentState == PattyState.Overcooked)
            return;

        cookingProgress += heatPerSecond * deltaTime;
        cookingProgress = Mathf.Clamp(cookingProgress, 0f, maxProgress);

        DetermineCookedState();
    }

    private void DetermineCookedState()
    {
        PattyState previousState = currentState;

        foreach (var threshold in cookingThresholds)
        {
            if (cookingProgress <= threshold.maxProgress)
            {
                currentState = threshold.state;
                break; 
            }
        }

        // Only update visuals if the state actually changed
        if (previousState != currentState)
        {
            UpdateVisuals();
            Debug.Log($"Patty state changed: {currentState} | Progress: {cookingProgress:F1}");
        }
    }

    private void UpdateVisuals()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Finds the color associated with the current state from our list
            renderer.material.color = GetColorForState(currentState);
        }
    }

    private Color GetColorForState(PattyState state)
    {
        // Pulls threshold from object and determines the color
        foreach (var threshold in cookingThresholds)
        {
            if (threshold.state == state)
                return threshold.stateColor;
        }
        return Color.white;
    }
}
