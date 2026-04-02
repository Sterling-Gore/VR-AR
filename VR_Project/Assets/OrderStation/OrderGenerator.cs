using System.Collections.Generic;
using UnityEngine;

//logic to create random Orders
//using "modes" instead of days because idk how many days we may be able to make
//endless mode possibly?
public class OrderGenerator
{
    // Creates and returns a randomized order based on the selected mode
    public Order GenerateOrder(int mode, int orderId, float startTime)
    {
        
        mode = Mathf.Clamp(mode, 1, 7);

        List<OrderItemRequest> requestedItems = GenerateRequestedItems(mode);
        float timeLimit = GetTimeLimitForMode(mode);
        int baseScore = GetBaseScoreForMode(mode);

        return new Order(orderId, requestedItems, startTime, timeLimit, baseScore);
    }

    // Builds the list of requested items for this order
    private List<OrderItemRequest> GenerateRequestedItems(int mode)
    {
        List<OrderItemRequest> items = new List<OrderItemRequest>();
        int itemCount = GetItemCountForMode(mode);

        for (int i = 0; i < itemCount; i++)
        {
            items.Add(GenerateRandomItem(mode));
        }

        return items;
    }

    // Creates one random requested item based on the mode
    private OrderItemRequest GenerateRandomItem(int mode)
    {
        FoodType foodType = GetRandomFoodType();
        CookLevel cookLevel = GetRandomCookLevel(mode);

        return new OrderItemRequest(foodType, cookLevel);
    }

    // Randomly picks a food type
    private FoodType GetRandomFoodType()
    {
        FoodType[] foodTypes = { FoodType.Burger, FoodType.Fries };
        int randomIndex = Random.Range(0, foodTypes.Length);
        return foodTypes[randomIndex];
    }

    // for now we only want perfectly cooked food
    //unless? overcooked? yummy!
    // i dont think yun would like that idea tho
    //if everyone agrees that we only doing perfectly cooked then we can just move this to OrderItemRequest
    
    private OrderItemRequest GenerateRandomItem()
    {
        FoodType foodType = GetRandomFoodType();
        return new OrderItemRequest(foodType, CookLevel.Cooked);
    }

    // Determines how many items should be in the order
    private int GetItemCountForMode(int mode)
    {
        switch (mode)
        {
            case 1:
            case 2:
                return 1;

            case 3:
                return Random.Range(1, 3); // 1 or 2

            case 4:
            case 5:
                return 2;

            case 6:
                return Random.Range(2, 4); // 2 or 3

            case 7:
                return 3;

            default:
                return 1;
        }
    }

    // Returns the time limit for the selected mode
    private float GetTimeLimitForMode(int mode)
    {
        switch (mode)
        {
            case 1: return 90f;
            case 2: return 80f;
            case 3: return 70f;
            case 4: return 60f;
            case 5: return 50f;
            case 6: return 45f;
            case 7: return 40f;
            default: return 90f;
        }
    }

    // Returns the base score for the selected mode
    // maximum score possible basically
    private int GetBaseScoreForMode(int mode)
    {
        switch (mode)
        {
            case 1: return 100;
            case 2: return 125;
            case 3: return 150;
            case 4: return 175;
            case 5: return 200;
            case 6: return 225;
            case 7: return 250;
            default: return 100;
        }
    }
}