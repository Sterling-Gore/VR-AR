using UnityEngine;
using System.Collections.Generic;

//we still need to build order scor
public class OrderSystem
{
    private readonly List<Order> activeOrders = new List<Order>();
    private readonly OrderGenerator orderGenerator = new OrderGenerator();

    private int nextOrderId = 1;

    // Creates a new order using the generator and stores it in the active order list
    public Order CreateOrder(int mode, float startTime)
    {
        Order newOrder = orderGenerator.GenerateOrder(mode, nextOrderId, startTime);
        activeOrders.Add(newOrder);
        nextOrderId++;

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
            return false;
        }

        activeOrders.Remove(order);
        return true;
    }

    public bool SubmitOrder(int orderId, List<ServedItem> servedItems, float completionTime)
    {
        Order order = GetOrderById(orderId);

        if (order == null)
        {
            return false;
        }

        if (order.IsExpired(completionTime))
        {
            order.MarkExpired();
            activeOrders.Remove(order);
            return false;
        }

        int finalScore = OrderScorer.CalculateScore(order, servedItems, completionTime);

        if (finalScore > 0)
        {
            order.MarkCompleted(completionTime, finalScore);
            activeOrders.Remove(order);
            return true;
        }

        order.MarkFailed(completionTime, 0);
        activeOrders.Remove(order);
        return false;
    }

    // Checks all active orders and expires any that run out of time
    public void ExpireOrders(float currentTime)
    {
        for (int i = activeOrders.Count - 1; i >= 0; i--)
        {
            if (activeOrders[i].IsExpired(currentTime))
            {
                activeOrders[i].MarkExpired();
                activeOrders.RemoveAt(i);
            }
        }
    }
}