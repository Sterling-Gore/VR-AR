using UnityEngine;

public enum PattyState
{
    Raw,
    Cooking,
    Cooked,
    Burned
}

public class Patty : MonoBehaviour
{
    public PattyState State { get; private set; } = PattyState.Raw;

    [SerializeField] private float cookThreshold = 5f;
    [SerializeField] private float burnThreshold = 10f;

    private float cookTime = 0f;

    public void ApplyHeat(float deltaTime)
    {
        if (State == PattyState.Burned)
            return;

        cookTime += deltaTime;

        if (cookTime >= burnThreshold)
            State = PattyState.Burned;
        else if (cookTime >= cookThreshold)
            State = PattyState.Cooked;
        else
            State = PattyState.Cooking;
    }
}