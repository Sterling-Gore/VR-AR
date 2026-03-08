using System.Collections.Generic;
using UnityEngine;

/*
    TLDR: Heavily referenced from Patty.cs
*/

public enum FryState
{
    Raw,
    Undercooked,
    Cooked,
    Overcooked
}

[System.Serializable]
public struct FryCookingThreshold
{
    public FryState state;
    public float maxProgress;
    public Color stateColor;
}

public class FryItem : MonoBehaviour
{
    [Header("Cooking Settings")]
    [SerializeField] private float heatPerSecond = 10f;
    [SerializeField] private float maxProgress = 100f;

    [Header("State Configuration")]
    [SerializeField] private List<FryCookingThreshold> cookingThresholds = new List<FryCookingThreshold>();

    private float cookingProgress;
    private FryState currentState = FryState.Raw;

    public FryState State => currentState;
    public float CookingProgress => cookingProgress;

    private void Start()
    {
        UpdateVisuals();
    }

    public void ApplyHeat(float deltaTime)
    {
        if (currentState == FryState.Overcooked)
        {
            return;
        }

        cookingProgress += heatPerSecond * deltaTime;
        cookingProgress = Mathf.Clamp(cookingProgress, 0f, maxProgress);

        DetermineCookedState();
    }

    public void ResetCooking()
    {
        cookingProgress = 0f;
        currentState = FryState.Raw;
        UpdateVisuals();
    }

    private void DetermineCookedState()
    {
        FryState previousState = currentState;

        if (cookingThresholds.Count == 0)
        {
            currentState = FryState.Raw;
            return;
        }

        currentState = cookingThresholds[cookingThresholds.Count - 1].state;

        foreach (FryCookingThreshold threshold in cookingThresholds)
        {
            if (cookingProgress <= threshold.maxProgress)
            {
                currentState = threshold.state;
                break;
            }
        }

        if (previousState != currentState)
        {
            UpdateVisuals();
            Debug.Log($"FryItem state changed: {currentState} | Progress: {cookingProgress:F1}");
        }
    }

    private void UpdateVisuals()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer == null)
        {
            return;
        }

        renderer.material.color = GetColorForState(currentState);
    }

    private Color GetColorForState(FryState state)
    {
        foreach (FryCookingThreshold threshold in cookingThresholds)
        {
            if (threshold.state == state)
            {
                return threshold.stateColor;
            }
        }

        return Color.yellow;
    }
}
