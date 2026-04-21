using UnityEngine;
using System.Collections.Generic;

public static class OrderScorer
{
    public static int CalculateScore(Order order, List<ServedItem> servedItems, float completionTime)
    {
        float totalScore = 0;
        List<OrderItemRequest> remainingRequests = new List<OrderItemRequest>(order.RequestedItems);
        List<ServedItem> remainingServed = new List<ServedItem>(servedItems);

        for (int i = remainingRequests.Count - 1; i >= 0; i--)
        {
            var req = remainingRequests[i];
            int servedIndex = remainingServed.FindIndex(s => s.FoodType == req.FoodType);

            if (servedIndex != -1)
            {
                totalScore += CalculateItemScore(req, remainingServed[servedIndex]);
                remainingRequests.RemoveAt(i);
                remainingServed.RemoveAt(servedIndex);
            }
            else
            {
                totalScore -= 50f; // If an item is missing, the players score is peanlized
            }
        }

        foreach (var extra in remainingServed) totalScore -= 30f; // Unnesscary addtional items

        if (totalScore > 0)
        {
            float timeRemaining = order.TimeLimit - (completionTime - order.StartTime);
            if (timeRemaining > 0) totalScore += timeRemaining * 2f; // If the player completes the order quickly, they gain a bonus
        }

        return Mathf.Max(0, Mathf.RoundToInt(totalScore));
    }

    private static float CalculateItemScore(OrderItemRequest req, ServedItem served)
    {
        float itemBaseScore = 0;

        if (served.ActualCookLevel == CookLevel.Raw || served.ActualCookLevel == CookLevel.Overcooked)
        {

            Debug.Log($"Item Discarded: {served.FoodType} is {served.ActualCookLevel}"); // Overcooked or Raw food gets zero points for the entire item
            return 0;
        }
        else if (served.ActualCookLevel == CookLevel.Undercooked)
        {
            itemBaseScore = 10f; // Partially cooked gives partial credit
        }
        else if (served.ActualCookLevel == CookLevel.Cooked)
        {
            itemBaseScore = 30f; // Cooked item
        }

        foreach (var ingredient in req.Ingredients)
        {
            if (served.ActualIngredients.Contains(ingredient)) // Partial credit for each cooked item
                itemBaseScore += 10f;
        }

        // Small deductions for extra ingredients that shouldn't be there
        foreach (var servedIng in served.ActualIngredients)
        {
            if (!req.Ingredients.Contains(servedIng))
                itemBaseScore -= 5f;
        }

        if (IsSequencePerfect(req.Ingredients, served.ActualIngredients)) // If everything is stacked properly, bonus points
            itemBaseScore += 50f;

        return Mathf.Max(0, itemBaseScore);
    }

    private static bool IsSequencePerfect(List<BurgerIngredients> req, List<BurgerIngredients> served)
    {
        if (req.Count != served.Count || req.Count == 0) return false;
        for (int i = 0; i < req.Count; i++)
            if (req[i] != served[i]) return false; // Order matters here!
        return true;
    }
}
