using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class TVTester : MonoBehaviour
{
    public OrderDisplayManager displayManager;
    public DisplayManager displayManager_script; 
    private OrderSystem orderSystem;
    private Order currentOrder;

    [SerializeField] private int currentMode = 3;

    private void Start()
    {
        // Use DisplayManager's OrderSystem instead of creating a new one
        if (displayManager_script != null)
        {
            orderSystem = displayManager_script.GetOrderSystem();
            Debug.Log("TVTester using DisplayManager's OrderSystem");
        }
        else
        {
            orderSystem = new OrderSystem();
            Debug.LogWarning("DisplayManager script not assigned on TVTester. Creating a separate OrderSystem.");
        }

        if (displayManager != null)
        {
            displayManager.ConnectToOrderSystem(orderSystem);
        }

        orderSystem.OnOrderFinished += HandleOrderFinished;
        currentOrder = orderSystem.CreateOrder(currentMode, Time.time);
        
        // Log the initial order
        LogOrderDetails(currentOrder);
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.T))
        // {
        //     Debug.Log("<color=cyan><b>Generating New Test Order...</b></color>");
        //     currentOrder = orderSystem.CreateOrder(currentMode, Time.time);
        //     LogOrderDetails(currentOrder);
        // }

        // if (Input.GetKeyDown(KeyCode.Y))
        // {
        //     if (currentOrder != null)
        //     {
        //         List<ServedItem> servedItems = BuildPerfectSubmission(currentOrder);
        //         orderSystem.SubmitOrder(currentOrder.OrderId, servedItems, Time.time);
        //     }
        // }
    }

    private void HandleOrderFinished(Order finishedOrder)
    {
        Debug.Log($"Order {finishedOrder.OrderId} finished. Status: {finishedOrder.Status}");
        currentOrder = orderSystem.CreateOrder(currentMode, Time.time);
        LogOrderDetails(currentOrder);
    }
    private void LogOrderDetails(Order order)
    {
        if (order == null) return;

        StringBuilder sb = new StringBuilder();
        // Color coding the header for visibility
        sb.AppendLine($"<color=cyan><b>[ORDER LOG] ID: {order.OrderId}</b></color>");

        foreach (var item in order.RequestedItems)
        {
            if (item.FoodType == FoodType.Burger)
            {
                string ingredientsList = string.Join(", ", item.Ingredients);
                sb.AppendLine($"- <color=orange><b>{item.FoodType}</b></color> | Cook: {item.RequiredCookLevel} | Ingredients: [{ingredientsList}]");
            }
            else if (item.FoodType == FoodType.Fries || item.FoodType == FoodType.CrinkleFries)
            {
                // Highlighting fries in a different color to distinguish them from the main burger
                sb.AppendLine($"- <color=yellow><b>{item.FoodType}</b></color> | Cook: {item.RequiredCookLevel} | (Side Dish)");
            }
            else
            {
                sb.AppendLine($"- <b>{item.FoodType}</b> | Cook: {item.RequiredCookLevel}");
            }
        }

        Debug.Log(sb.ToString());
    }

    private List<ServedItem> BuildPerfectSubmission(Order order)
    {
        List<ServedItem> servedItems = new List<ServedItem>();
        foreach (OrderItemRequest request in order.RequestedItems)
        {
            if (request.FoodType == FoodType.Burger)
            {
                servedItems.Add(new ServedItem(request.FoodType, request.RequiredCookLevel, request.PattyCount, request.Ingredients));
            }
            else
            {
                servedItems.Add(new ServedItem(request.FoodType, request.RequiredCookLevel));
            }
        }
        return servedItems;
    }
}