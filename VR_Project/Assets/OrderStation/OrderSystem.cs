using UnityEngine;
using System;
using System.Collections.Generic;

public class OrderSystem
{
    private readonly List<Order> activeOrders = new List<Order>();
    private readonly OrderGenerator orderGenerator = new OrderGenerator();

    private int nextOrderId = 1;

    public int TotalScore { get; private set; }
    public int LastSubmittedScore { get; private set; }

    // Other systems, like the TV display, can listen for these events
    public event Action<Order> OnOrderCreated;
    public event Action<Order> OnOrderFinished;

    // Future score display can listen for this event
    public event Action<int> OnTotalScoreChanged;

    // Creates a new order using the generator and stores it in the active order list
    public Order CreateOrder(int mode, float startTime)
    {
        Order newOrder = orderGenerator.GenerateOrder(mode, nextOrderId, startTime);
        activeOrders.Add(newOrder);
        nextOrderId++;

        Debug.Log($"[OrderSystem] Created order {newOrder.OrderId}. Active orders: {activeOrders.Count}");

        OnOrderCreated?.Invoke(newOrder);

        return newOrder;
    }

    // Returns a copy of the current active orders
    public List<Order> GetActiveOrders()
    {
        return new List<Order>(activeOrders);
    }

    // Finds an order by its id
    public Order GetOrderById(int orderId)
    {
        return activeOrders.Find(order => order.OrderId == orderId);
    }

    // Removes an order from the active order list
    public bool RemoveOrder(int orderId)
    {
        Order order = GetOrderById(orderId);

        if (order == null)
        {
            Debug.LogWarning($"[OrderSystem] Could not remove order {orderId}. Order was not found.");
            return false;
        }

        activeOrders.Remove(order);

        Debug.Log($"[OrderSystem] Removed order {order.OrderId}. Active orders: {activeOrders.Count}");

        OnOrderFinished?.Invoke(order);

        return true;
    }

    public bool SubmitOrder(int orderId, List<ServedItem> servedItems, float completionTime)
    {
        Debug.Log($"[OrderSystem] SubmitOrder called for order {orderId}.");

        Order order = GetOrderById(orderId);

        if (order == null)
        {
            LastSubmittedScore = 0;
            Debug.LogWarning($"[OrderSystem] Order {orderId} was not found. Submission failed.");
            return false;
        }

        int servedItemCount = servedItems != null ? servedItems.Count : 0;
        Debug.Log($"[OrderSystem] Order {orderId} found. Served item count: {servedItemCount}");

        if (order.IsExpired(completionTime))
        {
            LastSubmittedScore = 0;

            order.MarkExpired();
            activeOrders.Remove(order);

            Debug.LogWarning($"[OrderSystem] Order {orderId} expired. No points awarded. Total score: {TotalScore}");

            OnOrderFinished?.Invoke(order);

            return false;
        }

        int finalScore = OrderScorer.CalculateScore(order, servedItems, completionTime);
        LastSubmittedScore = finalScore;

        Debug.Log($"[OrderSystem] Order {orderId} calculated score: {finalScore}");

        if (finalScore > 0)
        {
            order.MarkCompleted(completionTime, finalScore);
            activeOrders.Remove(order);

            AddToTotalScore(finalScore);

            Debug.Log($"[OrderSystem] Order {orderId} completed. Added score: {finalScore}. Total score: {TotalScore}");

            OnOrderFinished?.Invoke(order);

            return true;
        }

        order.MarkFailed(completionTime, 0);
        activeOrders.Remove(order);

        Debug.Log($"[OrderSystem] Order {orderId} failed. Added score: 0. Total score: {TotalScore}");

        OnOrderFinished?.Invoke(order);

        return false;
    }

    // Checks all active orders and expires any that run out of time
    public void ExpireOrders(float currentTime)
    {
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            if (activeOrders[i].IsExpired(currentTime))
            {
                Order expiredOrder = activeOrders[i];

                LastSubmittedScore = 0;

                expiredOrder.MarkExpired();
                activeOrders.RemoveAt(i);

                Debug.LogWarning($"[OrderSystem] Order {expiredOrder.OrderId} expired automatically. No points awarded. Total score: {TotalScore}");

                OnOrderFinished?.Invoke(expiredOrder);
            }
        }
    }

    private void AddToTotalScore(int scoreToAdd)
    {
        TotalScore += scoreToAdd;

        Debug.Log($"[OrderSystem] Added {scoreToAdd} points. New total score: {TotalScore}");

        OnTotalScoreChanged?.Invoke(TotalScore);
    }
}