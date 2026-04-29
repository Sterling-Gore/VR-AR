using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

// RUN THIS CODE THROUGH COMMAND PROMPT WITHOUT UNITY ON.
// THE PROMPT WILL BE LIKE THIS
// "<their unity path>\Unity.exe" -batchmode -quit -projectPath "<their project path>" -executeMethod OrderScoringTerminalTest.Run -logFile "<where they want the log>"
//MINE FOR EXAMPLE
//"C:\Program Files\Unity\Hub\Editor\6000.3.6f1\Editor\Unity.exe" -batchmode -quit -projectPath "C:\Users\arsal\VR-AR\VR_Project" -executeMethod OrderScoringTerminalTest.Run -logFile "C:\Users\arsal\VR-AR\VR_Project\Assets\OrderStation\order_test_log.txt"

public static class OrderScoringTerminalTest
{
    public static void Run()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);

        Debug.Log("=== SCORING TEST START ===\n");

        OrderGenerator generator = new OrderGenerator();
        float startTime = 0f;
        float completionTime = 10f;

        for (int mode = 1; mode <= 7; mode++)
        {
            Order testOrder = generator.GenerateOrder(mode, mode, startTime);

            Debug.Log($"MODE {mode} ORDER GENERATED (Limit: {testOrder.TimeLimit}s)");
            Debug.Log(FormatOrder(testOrder));

            List<ServedItem> perfectServed = BuildPerfectSubmission(testOrder);
            int perfectScore = OrderScorer.CalculateScore(testOrder, perfectServed, completionTime);
            Debug.Log($"SCENARIO 1 [Perfect Submission]: Score = {perfectScore}");

            List<ServedItem> undercookedServed = BuildImperfectCookSubmission(testOrder, CookLevel.Undercooked);
            int undercookedScore = OrderScorer.CalculateScore(testOrder, undercookedServed, completionTime);
            Debug.Log($"SCENARIO 2 [Undercooked Food]: Score = {undercookedScore}");

            List<ServedItem> missingIngredientServed = BuildMissingIngredientSubmission(testOrder);
            int missingIngredientScore = OrderScorer.CalculateScore(testOrder, missingIngredientServed, completionTime);
            Debug.Log($"SCENARIO 3 [Missing Burger Ingredient If Burger Exists]: Score = {missingIngredientScore}");

            List<ServedItem> overcookedServed = BuildImperfectCookSubmission(testOrder, CookLevel.Overcooked);
            int overcookedScore = OrderScorer.CalculateScore(testOrder, overcookedServed, completionTime);
            Debug.Log($"SCENARIO 4 [Overcooked Food]: Score = {overcookedScore}");

            List<ServedItem> partialItems = BuildPartialSubmission(testOrder);
            int missingItemScore = OrderScorer.CalculateScore(testOrder, partialItems, completionTime);
            Debug.Log($"SCENARIO 5 [Missing Entire Item If Multiple Items Exist]: Score = {missingItemScore}");

            Debug.Log("----------------------------------------\n");
        }

        Debug.Log("=== SCORING TEST END ===");

        EditorApplication.Exit(0);
    }

    private static List<ServedItem> BuildPerfectSubmission(Order order)
    {
        List<ServedItem> served = new List<ServedItem>();

        foreach (OrderItemRequest req in order.RequestedItems)
        {
            served.Add(BuildServedItemFromRequest(req, req.RequiredCookLevel, req.Ingredients));
        }

        return served;
    }

    private static List<ServedItem> BuildImperfectCookSubmission(Order order, CookLevel level)
    {
        List<ServedItem> served = new List<ServedItem>();

        foreach (OrderItemRequest req in order.RequestedItems)
        {
            served.Add(BuildServedItemFromRequest(req, level, req.Ingredients));
        }

        return served;
    }

    private static List<ServedItem> BuildMissingIngredientSubmission(Order order)
    {
        List<ServedItem> served = new List<ServedItem>();
        bool removedIngredientAlready = false;

        foreach (OrderItemRequest req in order.RequestedItems)
        {
            if (req.FoodType == FoodType.Burger && req.Ingredients.Count > 0 && !removedIngredientAlready)
            {
                List<BurgerIngredients> modifiedIngredients = new List<BurgerIngredients>(req.Ingredients);
                modifiedIngredients.RemoveAt(0);
                removedIngredientAlready = true;

                served.Add(BuildServedItemFromRequest(req, req.RequiredCookLevel, modifiedIngredients));
            }
            else
            {
                served.Add(BuildServedItemFromRequest(req, req.RequiredCookLevel, req.Ingredients));
            }
        }

        return served;
    }

    private static List<ServedItem> BuildPartialSubmission(Order order)
    {
        List<ServedItem> served = BuildPerfectSubmission(order);

        if (served.Count > 0)
        {
            served.RemoveAt(served.Count - 1);
        }

        return served;
    }

    private static ServedItem BuildServedItemFromRequest(
        OrderItemRequest req,
        CookLevel cookLevel,
        List<BurgerIngredients> ingredients
    )
    {
        if (req.FoodType == FoodType.Burger)
        {
            return new ServedItem(
                req.FoodType,
                cookLevel,
                req.PattyCount,
                ingredients
            );
        }

        return new ServedItem(
            req.FoodType,
            cookLevel
        );
    }

    private static string FormatOrder(Order order)
    {
        StringBuilder sb = new StringBuilder();

        foreach (OrderItemRequest item in order.RequestedItems)
        {
            if (item.FoodType == FoodType.Burger)
            {
                string ingredients = item.Ingredients.Count > 0
                    ? string.Join(" > ", item.Ingredients)
                    : "None";

                sb.AppendLine($"- Burger ({item.PattyCount}P) layers: {ingredients}");
            }
            else
            {
                sb.AppendLine("- Fries");
            }
        }

        return sb.ToString();
    }
}