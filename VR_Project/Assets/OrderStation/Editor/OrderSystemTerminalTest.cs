using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

//same logic as the last one
// THE PROMPT WILL BE LIKE THIS
// "<their unity path>\Unity.exe" -batchmode -quit -projectPath "<their project path>" -executeMethod OrderSystemTerminalTest.Run -logFile "<where they want the log>"
//MINE FOR EXAMPLE
//"C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe" -batchmode -quit -projectPath "C:\Users\arsal\VR-AR\VR_Project" -executeMethod OrderSystemTerminalTest.Run -logFile "C:\Users\arsal\VR-AR\VR_Project\Assets\OrderStation\order_system_test_log.txt"


//ignore any potential white flash if u try to run it from vs code terminal....or anywhere else lol. 
public static class OrderSystemTerminalTest
{
    public static void Run()
    {
        // makes the log much cleaner in batch mode
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);

        Debug.Log("=== TEST START ===");

        OrderSystem orderSystem = new OrderSystem();

        // 1. Create three orders
        Order order1 = orderSystem.CreateOrder(1, 0f);
        Order order2 = orderSystem.CreateOrder(3, 0f);
        Order order3 = orderSystem.CreateOrder(5, 0f);

        Debug.Log("CREATED ORDERS");
        Debug.Log(FormatOrder(order1, 1));
        Debug.Log(FormatOrder(order2, 3));
        Debug.Log(FormatOrder(order3, 5));

        // 2. Show active orders
        Debug.Log($"ACTIVE AFTER CREATE: {orderSystem.GetActiveOrders().Count}");

        // 3. Test finding an order
        Order foundOrder = orderSystem.GetOrderById(order2.OrderId);
        Debug.Log(foundOrder != null
            ? $"FOUND ORDER: {foundOrder.OrderId}"
            : "FOUND ORDER: NONE");

        // 4. Submit a correct version of order1
        List<ServedItem> correctSubmission = BuildMatchingSubmission(order1);
        bool correctResult = orderSystem.SubmitOrder(order1.OrderId, correctSubmission, 10f);

        Debug.Log($"SUBMIT CORRECT ORDER {order1.OrderId}: {correctResult}");

        // 5. Submit a wrong version of order2
        List<ServedItem> wrongSubmission = BuildWrongSubmission(order2);
        bool wrongResult = orderSystem.SubmitOrder(order2.OrderId, wrongSubmission, 10f);

        Debug.Log($"SUBMIT WRONG ORDER {order2.OrderId}: {wrongResult}");

        // 6. Force order3 to expire
        orderSystem.ExpireOrders(1000f);
        Order expiredCheck = orderSystem.GetOrderById(order3.OrderId);

        Debug.Log(expiredCheck == null
            ? $"ORDER {order3.OrderId} EXPIRED AND REMOVED"
            : $"ORDER {order3.OrderId} STILL ACTIVE");

        // 7. Final active count
        Debug.Log($"ACTIVE AT END: {orderSystem.GetActiveOrders().Count}");

        Debug.Log("=== TEST END ===");

        EditorApplication.Exit(0);
    }

    private static List<ServedItem> BuildMatchingSubmission(Order order)
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

    private static List<ServedItem> BuildWrongSubmission(Order order)
    {
        List<ServedItem> servedItems = new List<ServedItem>();

        foreach (OrderItemRequest request in order.RequestedItems)
        {
            // Make the first item intentionally wrong
            if (servedItems.Count == 0)
            {
                if (request.FoodType == FoodType.Burger)
                {
                    servedItems.Add(new ServedItem(
                        FoodType.Burger,
                        CookLevel.Overcooked,
                        request.PattyCount + 1,
                        new List<BurgerIngredients>()
                    ));
                }
                else
                {
                    servedItems.Add(new ServedItem(
                        FoodType.Fries,
                        CookLevel.Raw
                    ));
                }
            }
            else
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
        }

        return servedItems;
    }

    private static string FormatOrder(Order order, int mode)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"[MODE {mode}] ORDER {order.OrderId}");

        for (int i = 0; i < order.RequestedItems.Count; i++)
        {
            OrderItemRequest item = order.RequestedItems[i];

            if (item.FoodType == FoodType.Burger)
            {
                string ingredientsText = item.Ingredients != null && item.Ingredients.Count > 0
                    ? string.Join("+", item.Ingredients)
                    : "None";

                sb.AppendLine(
                    $"Burger | Cook:{item.RequiredCookLevel} | Patties:{item.PattyCount} | Ingredients:{ingredientsText}"
                );
            }
            else
            {
                sb.AppendLine(
                    $"Fries | Cook:{item.RequiredCookLevel}"
                );
            }
        }

        sb.AppendLine("----------------------------------------");
        return sb.ToString();
    }
}