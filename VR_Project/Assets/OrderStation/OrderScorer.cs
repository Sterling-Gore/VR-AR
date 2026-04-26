using UnityEngine;
using System.Collections.Generic;

public static class OrderScorer
{
    public static int CalculateScore(Order order, List<ServedItem> servedItems, float completionTime)
    {
        float totalScore = 0;

        List<OrderItemRequest> remainingRequests = new List<OrderItemRequest>(order.RequestedItems);
        List<ServedItem> remainingServed = servedItems != null
            ? new List<ServedItem>(servedItems)
            : new List<ServedItem>();

        for (int i = remainingRequests.Count - 1; i >= 0; i--)
        {
            OrderItemRequest req = remainingRequests[i];
            int servedIndex = remainingServed.FindIndex(s => s.FoodType == req.FoodType);

            if (servedIndex != -1)
            {
                totalScore += CalculateItemScore(req, remainingServed[servedIndex]);
                remainingRequests.RemoveAt(i);
                remainingServed.RemoveAt(servedIndex);
            }
            else
            {
                totalScore -= 50f; // If an item is missing, the player's score is penalized
            }
        }

        foreach (ServedItem extra in remainingServed)
        {
            totalScore -= 30f; // Unnecessary additional items
        }

        if (totalScore > 0)
        {
            float timeRemaining = order.TimeLimit - (completionTime - order.StartTime);

            if (timeRemaining > 0)
            {
                totalScore += timeRemaining * 2f; // If the player completes the order quickly, they gain a bonus
            }
        }

        return Mathf.Max(0, Mathf.RoundToInt(totalScore));
    }

    private static float CalculateItemScore(OrderItemRequest req, ServedItem served)
    {
        float itemBaseScore = 0;

        if (served.FoodType == FoodType.Burger)
        {
            itemBaseScore += CalculateBurgerCookScore(req, served);
            itemBaseScore += CalculateIngredientScore(req, served);
        }
        else if (served.FoodType == FoodType.Fries)
        {
            itemBaseScore += CalculateSingleCookScore(served);
        }

        return Mathf.Max(0, itemBaseScore);
    }

    private static float CalculateBurgerCookScore(OrderItemRequest req, ServedItem served)
    {
        if (served.ActualCookLevels == null || served.ActualCookLevels.Count == 0)
        {
            Debug.Log($"Burger has no patties.");
            return 0;
        }

        float score = 0;

        foreach (CookLevel cookLevel in served.ActualCookLevels)
        {
            score += GetCookLevelScore(cookLevel);
        }

        if (served.ActualPattyCount != req.PattyCount)
        {
            int pattyDifference = Mathf.Abs(served.ActualPattyCount - req.PattyCount);
            score -= pattyDifference * 15f;
        }

        return Mathf.Max(0, score);
    }

    private static float CalculateSingleCookScore(ServedItem served)
    {
        if (served.ActualCookLevels == null || served.ActualCookLevels.Count == 0)
        {
            return 0;
        }

        return GetCookLevelScore(served.ActualCookLevels[0]);
    }

    private static float GetCookLevelScore(CookLevel cookLevel)
    {
        if (cookLevel == CookLevel.Raw || cookLevel == CookLevel.Overcooked)
        {
            Debug.Log($"Item cook level is {cookLevel}");
            return 0;
        }

        if (cookLevel == CookLevel.Undercooked)
        {
            return 10f;
        }

        if (cookLevel == CookLevel.Cooked)
        {
            return 30f;
        }

        return 0;
    }

    private static float CalculateIngredientScore(OrderItemRequest req, ServedItem served)
    {
        float ingredientScore = 0;

        foreach (BurgerIngredients ingredient in req.Ingredients)
        {
            if (served.ActualIngredients.Contains(ingredient))
            {
                ingredientScore += 10f;
            }
        }

        foreach (BurgerIngredients servedIng in served.ActualIngredients)
        {
            if (!req.Ingredients.Contains(servedIng))
            {
                ingredientScore -= 5f;
            }
        }

        if (IsSequencePerfect(req.Ingredients, served.ActualIngredients))
        {
            ingredientScore += 50f;
        }

        return ingredientScore;
    }

    private static bool IsSequencePerfect(List<BurgerIngredients> req, List<BurgerIngredients> served)
    {
        if (req.Count != served.Count || req.Count == 0)
        {
            return false;
        }

        for (int i = 0; i < req.Count; i++)
        {
            if (req[i] != served[i])
            {
                return false; // Order matters here!
            }
        }

        return true;
    }
}