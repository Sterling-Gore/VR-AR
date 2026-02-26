using UnityEngine;

// Defines all possible cooking states of the patty
public enum PattyState
{
    Raw,
    Undercooked,
    Cooked,
    Overcooked
}

public class Patty : MonoBehaviour
{
    // =============================
    // Cooking Configuration
    // =============================

    // How fast the patty cooks while being pressed
    [Header("Cooking Settings")]
    [SerializeField] private float heatPerSecond = 5f;

    // Threshold values that determine state transitions
    [Header("State Thresholds")]
    [SerializeField] private float rawMax = 15f;           // 0–15
    [SerializeField] private float undercookedMax = 35f;   // 16–35
    [SerializeField] private float cookedMax = 65f;        // 36–65
    [SerializeField] private float maxProgress = 100f;     // Maximum cooking cap

   
    // Internal State Tracking
    // Current cooking progress (0–100 scale)
    private float cookingProgress = 0f;

    // Current state of the patty
    private PattyState currentState = PattyState.Raw;

    // Public read-only accessors
    public PattyState State => currentState;
    public float CookingProgress => cookingProgress;

    // Called by Presser
    // increases cooking progress while the patty is being pressed
    public void ApplyHeat(float deltaTime)
    {
        

        // Stop increasing heat once overcooked
        if (currentState == PattyState.Overcooked)
            return;

        // Increase cooking progress based on time
        cookingProgress += heatPerSecond * deltaTime;

        // Clamp progress between 0 and maxProgress
        cookingProgress = Mathf.Clamp(cookingProgress, 0f, maxProgress);

        // Recalculate state after heating
        UpdateState();
    }

    private void UpdateState()
    {
        // Store previous state to detect changes
        PattyState previousState = currentState;

        if (cookingProgress <= rawMax)
            currentState = PattyState.Raw;
        else if (cookingProgress <= undercookedMax)
            currentState = PattyState.Undercooked;
        else if (cookingProgress <= cookedMax)
            currentState = PattyState.Cooked;
        else
            currentState = PattyState.Overcooked;

        // Only log when state changes
        if (previousState != currentState)
        {
            Debug.Log($"Patty state changed: {currentState} | Progress: {cookingProgress:F1}");
        }
    }
}