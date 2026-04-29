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

    // Builds the list of requested items for this order with new caps
    private List<OrderItemRequest> GenerateRequestedItems(int mode)
    {
        List<OrderItemRequest> items = new List<OrderItemRequest>();
        int targetCount = GetItemCountForMode(mode);
        int burgerCount = 0;
        int friesCount = 0;

        while (items.Count < targetCount) // Loop until we reach the target count or hit the global cap
        {
            FoodType type = GetRandomFoodType();

            if (type == FoodType.Burger && burgerCount < 1) // Max 1 burger
            {
                items.Add(GenerateRandomItem(mode, type));
                burgerCount++;
            }

            else if ((type == FoodType.Fries || type == FoodType.CrinkleFries) && friesCount < 2) // max 2 type of fries 
            {
                items.Add(GenerateRandomItem(mode, type));
                friesCount++;
            }

            if (burgerCount >= 1 && friesCount >= 2) break;
            
            if (items.Count >= targetCount) break;
        }

        return items;
    }

    // Creates one random requested item based on the mode and pre-determined type
    private OrderItemRequest GenerateRandomItem(int mode, FoodType foodType)
    {
        CookLevel cookLevel = CookLevel.Cooked; // must be perfectly cooked for max score

        if (foodType == FoodType.Burger)
        {
            // Harder modes have a chance for more patties
            int maxPatties = (mode <= 3) ? 1 : (mode <= 5) ? 2 : 3;
            int actualPatties = Random.Range(1, maxPatties + 1);

            List<BurgerIngredients> toppings = GenerateRandomIngredients(mode, actualPatties);

            return new OrderItemRequest(foodType, cookLevel, actualPatties, toppings);
        }

        return new OrderItemRequest(foodType, cookLevel);
    }

    // Randomly picks a food type
    private FoodType GetRandomFoodType()
    {
        float rand = Random.value;
        if (rand < 0.4f) return FoodType.Burger;
        if (rand < 0.7f) return FoodType.Fries;
        return FoodType.CrinkleFries;
    }

    // creates random ingredients for the burger with some constraints
    private List<BurgerIngredients> GenerateRandomIngredients(int mode, int pattyCount) {
        List<BurgerIngredients> layers = new List<BurgerIngredients>();
        const int MAX_TOTAL_LAYERS = 10; 
        
        
        layers.Add(BurgerIngredients.BottomBun); 
        
        BurgerIngredients[] possibleSauces = { BurgerIngredients.Ketchup, BurgerIngredients.Mustard, BurgerIngredients.Mayo };
        layers.Add(possibleSauces[Random.Range(0, possibleSauces.Length)]);

        for (int i = 0; i < pattyCount; i++)
        {
            if (layers.Count >= MAX_TOTAL_LAYERS - 4) break; 

            layers.Add(BurgerIngredients.Patty);

            if (Random.value < 0.6f) layers.Add(BurgerIngredients.Cheese);
            if (mode >= 3 && Random.value < 0.4f) layers.Add(BurgerIngredients.Lettuce);
        }

        layers.Add(possibleSauces[Random.Range(0, possibleSauces.Length)]);

 
        layers.Add(BurgerIngredients.TopBun); 
        
        return layers;
    }

    private int GetItemCountForMode(int mode)     // Determines how many items should be in the order
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
