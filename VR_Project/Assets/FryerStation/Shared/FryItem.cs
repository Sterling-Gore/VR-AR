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
    [SerializeField] private List<FryCookingThreshold> cookingThresholds = new List<FryCookingThreshold>
    {
        new FryCookingThreshold { state = FryState.Raw, maxProgress = 25f, stateColor = new Color(1f, 0.95f, 0.1f, 1f) },
        new FryCookingThreshold { state = FryState.Undercooked, maxProgress = 50f, stateColor = new Color(1f, 0.75f, 0.1f, 1f) },
        new FryCookingThreshold { state = FryState.Cooked, maxProgress = 75f, stateColor = new Color(0.96f, 0.52f, 0.08f, 1f) },
        new FryCookingThreshold { state = FryState.Overcooked, maxProgress = 100f, stateColor = new Color(0.85f, 0.35f, 0.05f, 1f) }
    };

    [Header("Visual Settings")]
    [SerializeField] private Color rawColor = new Color(1f, 0.95f, 0.1f, 1f);
    [SerializeField] private Color cookedColor = new Color(0.85f, 0.35f, 0.05f, 1f);

    private float cookingProgress;
    private FryState currentState = FryState.Raw;
    private Renderer cachedRenderer;

    public FryState State => currentState;
    public float CookingProgress => cookingProgress;
    public FoodType fryType;

    private void Awake()
    {
        cachedRenderer = GetComponent<Renderer>();
    }

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
        UpdateVisuals();
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
            Debug.Log($"FryItem state changed: {currentState} | Progress: {cookingProgress:F1}");
        }
    }

    private void UpdateVisuals()
    {
        if (cachedRenderer == null)
        {
            return;
        }

        float normalizedProgress = Mathf.Clamp01(cookingProgress / maxProgress);
        Color cookingColor = Color.Lerp(rawColor, cookedColor, normalizedProgress);
        cachedRenderer.material.color = cookingColor;
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
