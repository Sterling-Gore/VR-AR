using System.Collections.Generic;
using UnityEngine;

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
        if (order == null || order.IsExpired(completionTime)) return false;

        // Use the flexible matching scorer
        int finalScore = OrderScorer.CalculateScore(order, servedItems, completionTime);

        if (finalScore > 0) // If they earned any points order is counted as completed
        {
            order.MarkCompleted(completionTime, finalScore);
            activeOrders.Remove(order);
            return true;
        }
        else
        {
            // 0 points means they failed
            order.MarkFailed(completionTime, 0);
            activeOrders.Remove(order);
            return false;
        }
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

    // Compares a submitted order against the requested order
    // This is the first simple validation version
    private bool ValidateOrder(Order order, List<ServedItem> servedItems)
    {
        if (servedItems == null)
        {
            return false;
        }

        if (order.RequestedItems.Count != servedItems.Count)
        {
            return false;
        }

        for (int i = 0; i < order.RequestedItems.Count; i++)
        {
            OrderItemRequest requested = order.RequestedItems[i];
            ServedItem served = servedItems[i];

            if (requested.FoodType != served.FoodType)
            {
                return false;
            }

            if (requested.RequiredCookLevel != served.ActualCookLevel)
            {
                return false;
            }

            if (requested.PattyCount != served.ActualPattyCount)
            {
                return false;
            }

            if (!IngredientsMatch(requested.Ingredients, served.ActualIngredients))
            {
                return false;
            }
        }

        return true;
    }

    // Checks whether two ingredient lists match exactly
    private bool IngredientsMatch(List<BurgerIngredients> requestedIngredients, List<BurgerIngredients> servedIngredients)
    {
        if (requestedIngredients == null)
        {
            requestedIngredients = new List<BurgerIngredients>();
        }

        if (servedIngredients == null)
        {
            servedIngredients = new List<BurgerIngredients>();
        }

        if (requestedIngredients.Count != servedIngredients.Count)
        {
            return false;
        }

        List<BurgerIngredients> requestedCopy = new List<BurgerIngredients>(requestedIngredients);
        List<BurgerIngredients> servedCopy = new List<BurgerIngredients>(servedIngredients);

        requestedCopy.Sort();
        servedCopy.Sort();

        for (int i = 0; i < requestedCopy.Count; i++)
        {
            if (requestedCopy[i] != servedCopy[i])
            {
                return false;
            }
        }

        return true;
    }
}
