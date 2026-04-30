using UnityEngine;
using System.Collections.Generic;

// IF YOU WANT TO TEST THE TV UNCOMMENT THIS CODE

public class TVTester : MonoBehaviour
{
    public OrderDisplayManager displayManager;

    private OrderSystem orderSystem;
    private Order currentOrder;

    [SerializeField] private int currentMode = 3;

    private void Start()
    {
        orderSystem = new OrderSystem();

        if (displayManager != null)
        {
            displayManager.ConnectToOrderSystem(orderSystem);
        }

        orderSystem.OnOrderFinished += HandleOrderFinished;

        currentOrder = orderSystem.CreateOrder(currentMode, Time.time);
    }

    private void OnDestroy()
    {
        if (orderSystem != null)
        {
            orderSystem.OnOrderFinished -= HandleOrderFinished;
        }
    }

    private void Update()
    {
        // Press T while the game is running to manually create a new test order
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Testing TV with a random order...");
            currentOrder = orderSystem.CreateOrder(currentMode, Time.time);
        }

        // Press Y while the game is running to simulate turning in the current order correctly
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("Simulating correct order submission...");

            if (currentOrder != null)
            {
                List<ServedItem> servedItems = BuildPerfectSubmission(currentOrder);
                orderSystem.SubmitOrder(currentOrder.OrderId, servedItems, Time.time);
            }
        }
    }

    private void HandleOrderFinished(Order finishedOrder)
    {
        Debug.Log($"Order {finishedOrder.OrderId} finished with status {finishedOrder.Status}. Creating next order...");

        currentOrder = orderSystem.CreateOrder(currentMode, Time.time);
    }

    private List<ServedItem> BuildPerfectSubmission(Order order)
    {
        List<ServedItem> servedItems = new List<ServedItem>();

        foreach (OrderItemRequest request in order.RequestedItems)
        {
            if (request.FoodType == FoodType.Burger)
            {
                servedItems.Add(new ServedItem(
                    request.FoodType,
                    request.RequiredCookLevel,
                    request.PattyCount,
                    request.Ingredients
                ));
            }
            else
            {
                servedItems.Add(new ServedItem(
                    request.FoodType,
                    request.RequiredCookLevel
                ));
            }
        }

        return servedItems;
    }
}
