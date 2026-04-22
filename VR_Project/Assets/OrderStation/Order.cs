using System.Collections.Generic;

[System.Serializable]
public class Order
{
    public int OrderId { get; }
    public List<OrderItemRequest> RequestedItems { get; }
    public OrderStatus Status { get; private set; }

    public float StartTime { get; }
    public float TimeLimit { get; }
    public float? CompletionTime { get; private set; }

    public int BaseScore { get; }
    public int FinalScore { get; private set; }

    public Order(
        int orderId,
        List<OrderItemRequest> requestedItems,
        float startTime,
        float timeLimit,
        int baseScore
    )
    {
        OrderId = orderId;

        RequestedItems = requestedItems != null
            ? new List<OrderItemRequest>(requestedItems)
            : new List<OrderItemRequest>();

        StartTime = startTime;
        TimeLimit = timeLimit;

        BaseScore = baseScore;
        FinalScore = baseScore;

        Status = OrderStatus.Active;
        CompletionTime = null;
    }

    public bool IsExpired(float currentTime)
    {
        return Status == OrderStatus.Active &&
               currentTime > StartTime + TimeLimit;
    }

    public void MarkCompleted(float completionTime, int finalScore)
    {
        CompletionTime = completionTime;
        FinalScore = finalScore;
        Status = OrderStatus.Completed;
    }

    public void MarkFailed(float completionTime, int finalScore)
    {
        CompletionTime = completionTime;
        FinalScore = finalScore;
        Status = OrderStatus.Failed;
    }

    public void MarkExpired()
    {
        CompletionTime = null;
        FinalScore = 0;
        Status = OrderStatus.Expired;
    }
}
