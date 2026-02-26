using UnityEngine;

public enum PattyState
{
    Raw,
    Undercooked,
    Cooked,
    Overcooked
}

public class Patty : MonoBehaviour
{

    [SerializeField, Range(0f, 100f)]
    private float cookingProgress = 0f;

    public float CookingProgress => cookingProgress;
    public PattyState State { get; private set; } = PattyState.Raw;

    [Header("State Thresholds (0-100)")]
    [SerializeField] private float rawMax = 15f;          // 0-15 Raw
    [SerializeField] private float undercookedMax = 35f;  // 16-35 Undercooked
    [SerializeField] private float cookedMax = 65f;       // 36-65 Cooked
    // 66-100 Overcooked

    // How fast pressing cooks the patty
    [Header("Heating")]
    [SerializeField] private float heatPerSecond = 5f;


    /// Call this ONLY while the patty is being pressed.
    /// Pass Time.deltaTime from the caller (Presser).
    public void ApplyHeat(float deltaTime)
    {
        if (State == PattyState.Overcooked)
            return;

        cookingProgress += heatPerSecond * deltaTime;
        if (cookingProgress > 100f)
            cookingProgress = 100f;

        UpdateState();
    }

    private void UpdateState()
    {
        Debug.Log($"Patty State: {State} | Progress: {cookingProgress}");
        if (cookingProgress <= rawMax)
            State = PattyState.Raw;
        else if (cookingProgress <= undercookedMax)
            State = PattyState.Undercooked;
        else if (cookingProgress <= cookedMax)
            State = PattyState.Cooked;
        else
            State = PattyState.Overcooked;
    }
}