using System.Text;
using UnityEditor;
using UnityEngine;

// RUN THIS CODE THROUGH COMMAND PROMPT WITHOUT UNITY ON.
// THE PROMPT WILL BE LIKE THIS
// "<their unity path>\Unity.exe" -batchmode -quit -projectPath "<their project path>" -executeMethod OrderGeneratorTerminalTest.Run -logFile "<where they want the log>"
//MINE FOR EXAMPLE
//"C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe" -batchmode -quit -projectPath "C:\Users\arsal\VR-AR\VR_Project" -executeMethod OrderGeneratorTerminalTest.Run -logFile "C:\Users\arsal\VR-AR\VR_Project\Assets\OrderStation\order_test_log.txt"

public static class OrderGeneratorTerminalTest
{
    public static void Run()
    {
        // Remove stack traces from normal Debug.Log output
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);

        OrderGenerator generator = new OrderGenerator();

        for (int i = 1; i <= 3; i++)
        {
            //if u want to change the mode then just change the loop or the below to whatever u want
            int mode = i;
            Order order = generator.GenerateOrder(mode, i, 0f);

            Debug.Log(FormatOrder(order, mode));
        }

        EditorApplication.Exit(0);
    }

    private static string FormatOrder(Order order, int mode)
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"MODE {mode}");
        sb.AppendLine($"ORDER {order.OrderId}");

        for (int i = 0; i < order.RequestedItems.Count; i++)
        {
            OrderItemRequest item = order.RequestedItems[i];

            if (item.FoodType == FoodType.Burger)
            {
                string ingredientsText = item.Ingredients != null && item.Ingredients.Count > 0
                    ? string.Join(", ", item.Ingredients)
                    : "None";

                sb.AppendLine(
                    $"- Burger | Cook: {item.RequiredCookLevel} | Patties: {item.PattyCount} | Ingredients: {ingredientsText}"
                );
            }
            else
            {
                sb.AppendLine(
                    $"- Fries | Cook: {item.RequiredCookLevel}"
                );
            }
        }

        sb.AppendLine("----------------------------------------");
        return sb.ToString();
    }
}