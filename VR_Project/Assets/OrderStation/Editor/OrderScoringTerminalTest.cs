using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class OrderScoringTerminalTest
{
    public static void Run()
    {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Debug.Log("=== SCORING TEST START ===\n");

        OrderGenerator generator = new OrderGenerator();
        float startTime = 0f;
        float completionTime = 10f; // Simulate 10 seconds passing

        // Create a base order for testing (Mode 4: potentially multiple items/patties)
        Order testOrder = generator.GenerateOrder(4, 1, startTime);
        Debug.Log($"TEST ORDER GENERATED (Mode 4, Limit: {testOrder.TimeLimit}s)");
        Debug.Log(FormatOrder(testOrder));

        // Perfect cook
        List<ServedItem> perfectServed = BuildPerfectSubmission(testOrder);
        int perfectScore = OrderScorer.CalculateScore(testOrder, perfectServed, completionTime);
        Debug.Log($"SCENARIO 1 [Perfect Stack + Cooked]: Score = {perfectScore}");

        // under cooked
        List<ServedItem> rareServed = BuildImperfectCookSubmission(testOrder, CookLevel.Undercooked);
        int rareScore = OrderScorer.CalculateScore(testOrder, rareServed, completionTime);
        Debug.Log($"SCENARIO 2 [Undercooked Patty]: Score = {rareScore} (Expected lower than Perfect)");

        // ingredient missing
        List<ServedItem> missingIngServed = BuildMissingIngredientSubmission(testOrder);
        int missingIngScore = OrderScorer.CalculateScore(testOrder, missingIngServed, completionTime);
        Debug.Log($"SCENARIO 3 [Missing 1 Ingredient]: Score = {missingIngScore} (No Master Chef Bonus)");

        // burnt 
        List<ServedItem> burntServed = BuildImperfectCookSubmission(testOrder, CookLevel.Overcooked);
        int burntScore = OrderScorer.CalculateScore(testOrder, burntServed, completionTime);
        Debug.Log($"SCENARIO 4 [Burnt/Overcooked]: Score = {burntScore} (Expected 0 for that item)");

        // missing multiple items
        // We only serve the first item, missing the rest
        List<ServedItem> partialItems = new List<ServedItem> { perfectServed[0] };
        int missingItemScore = OrderScorer.CalculateScore(testOrder, partialItems, completionTime);
        Debug.Log($"SCENARIO 5 [Missing Entire Item]: Score = {missingItemScore} (-50 Penalty)");

        Debug.Log("\n=== SCORING TEST END ===");
        EditorApplication.Exit(0);
    }

    private static List<ServedItem> BuildPerfectSubmission(Order order)
    {
        List<ServedItem> served = new List<ServedItem>();
        foreach (var req in order.RequestedItems)
        {
            served.Add(new ServedItem(req.FoodType, req.RequiredCookLevel, req.PattyCount, req.Ingredients));
        }
        return served;
    }

    private static List<ServedItem> BuildImperfectCookSubmission(Order order, CookLevel level)
    {
        List<ServedItem> served = new List<ServedItem>();
        foreach (var req in order.RequestedItems)
        {
            // Apply the bad cook level to burgers/fries specifically
            served.Add(new ServedItem(req.FoodType, level, req.PattyCount, req.Ingredients));
        }
        return served;
    }

    private static List<ServedItem> BuildMissingIngredientSubmission(Order order)
    {
        List<ServedItem> served = new List<ServedItem>();
        foreach (var req in order.RequestedItems)
        {
            List<BurgerIngredients> modifiedIngredients = new List<BurgerIngredients>(req.Ingredients);
            if (modifiedIngredients.Count > 2) // Remove one topping if possible (not a bun)
                modifiedIngredients.RemoveAt(1);

            served.Add(new ServedItem(req.FoodType, req.RequiredCookLevel, req.PattyCount, modifiedIngredients));
        }
        return served;
    }

    private static string FormatOrder(Order order)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in order.RequestedItems)
        {
            string detail = item.FoodType == FoodType.Burger 
                ? $"Burger ({item.PattyCount}P) layers: {string.Join(" > ", item.Ingredients)}" 
                : "Fries";
            sb.AppendLine($"- {detail}");
        }
        return sb.ToString();
    }
}
