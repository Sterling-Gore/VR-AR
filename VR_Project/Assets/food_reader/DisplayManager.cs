using UnityEngine;
using System.Collections.Generic;

public class DisplayManager : MonoBehaviour
{
    [Header("References")]
    public OrderDisplayManager orderDisplayManager;
    public FoodReader foodReader;

    private OrderSystem orderSystem;
    private Order currentOrder;

    [SerializeField] private int currentMode = 3;

    private void Start()
    {
        orderSystem = new OrderSystem();

        if (orderDisplayManager != null)
        {
            orderDisplayManager.ConnectToOrderSystem(orderSystem);
        }
        else
        {
            Debug.LogError("OrderDisplayManager is not assigned on DisplayManager.");
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
        if (orderSystem != null)
        {
            orderSystem.ExpireOrders(Time.time);
        }

        // Optional desktop test
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SubmitCurrentOrder();
        }
    }

    public void SubmitCurrentOrder()
    {
        if (currentOrder == null)
        {
            Debug.LogWarning("No current order to submit.");
            return;
        }

        if (foodReader == null)
        {
            Debug.LogError("FoodReader is not assigned on DisplayManager.");
            return;
        }

        List<ServedItem> servedItems = foodReader.DeliverFood();

        if (servedItems == null || servedItems.Count == 0)
        {
            Debug.Log("No food was delivered. Order was not submitted.");
            return;
        }

        int submittedOrderId = currentOrder.OrderId;

        bool wasSuccessful = orderSystem.SubmitOrder(
            submittedOrderId,
            servedItems,
            Time.time
        );

        Debug.Log($"Submitted order {submittedOrderId}. Success: {wasSuccessful}");
    }

    private void HandleOrderFinished(Order finishedOrder)
    {
        Debug.Log($"Order {finishedOrder.OrderId} finished with status {finishedOrder.Status}. Creating next order...");

        currentOrder = orderSystem.CreateOrder(currentMode, Time.time);
    }
}