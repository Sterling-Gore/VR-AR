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
        orderSystem.OnTotalScoreChanged += HandleTotalScoreChanged;

        currentOrder = orderSystem.CreateOrder(currentMode, Time.time);

        Debug.Log($"[DisplayManager] Game started. Current total score: {orderSystem.TotalScore}");
    }

    private void OnDestroy()
    {
        if (orderSystem != null)
        {
            orderSystem.OnOrderFinished -= HandleOrderFinished;
            orderSystem.OnTotalScoreChanged -= HandleTotalScoreChanged;
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
        Debug.Log("SubmitCurrentOrder called");

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

        Debug.Log($"Submitting order ID: {currentOrder.OrderId}");
        Debug.Log("Calling foodReader.DeliverFood()");

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
        Debug.Log($"Last submitted score: {orderSystem.LastSubmittedScore}");
        Debug.Log($"Current total score: {orderSystem.TotalScore}");
    }

    private void HandleOrderFinished(Order finishedOrder)
    {
        Debug.Log(
            $"Order {finishedOrder.OrderId} finished with status {finishedOrder.Status}. " +
            $"Final score: {finishedOrder.FinalScore}. " +
            $"Total score: {orderSystem.TotalScore}. " +
            "Creating next order..."
        );

        currentOrder = orderSystem.CreateOrder(currentMode, Time.time);
    }

    private void HandleTotalScoreChanged(int newTotalScore)
    {
        Debug.Log($"[DisplayManager] Total score changed: {newTotalScore}");
    }

    // Public method to access the OrderSystem
    public OrderSystem GetOrderSystem()
    {
        return orderSystem;
    }
}