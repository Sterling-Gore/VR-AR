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

        // for now all generated orders want perfectly cooked food
        CookLevel cookLevel = CookLevel.Cooked;

        if (foodType == FoodType.Burger)
        {
            // Harder modes have a chance for more patties
            int patties = (mode > 4 && Random.value < 0.35f) ? 2 : 1;

            List<BurgerIngredients> toppings = GenerateRandomIngredients(mode);

            return new OrderItemRequest(foodType, cookLevel, patties, toppings);
        }

        return new OrderItemRequest(foodType, cookLevel);
    }

    // Randomly picks a food type
    private FoodType GetRandomFoodType()
    {
        FoodType[] foodTypes = { FoodType.Burger, FoodType.Fries };
        int randomIndex = Random.Range(0, foodTypes.Length);
        return foodTypes[randomIndex];
    }

    // creates random ingredients for the burger
    private List<BurgerIngredients> GenerateRandomIngredients(int mode)
    {
        List<BurgerIngredients> chosen = new List<BurgerIngredients>();

        List<BurgerIngredients> sauces = new List<BurgerIngredients>
        {
            BurgerIngredients.Ketchup,
            BurgerIngredients.Mustard,
            BurgerIngredients.Mayo
        };

        List<BurgerIngredients> physicalToppings = new List<BurgerIngredients>
        {
            BurgerIngredients.Lettuce,
            BurgerIngredients.Tomato
        };

        // max sauce and toppings tied to the difficulty mode
        int maxSauces = (mode > 3) ? 2 : 1;
        int sauceCount = Random.Range(0, maxSauces + 1);

        int maxTopping = Mathf.Min(mode, 3);
        int toppingCount = Random.Range(0, maxTopping + 1);

        // sauces should not duplicate
        List<BurgerIngredients> availableSauces = new List<BurgerIngredients>(sauces);

        for (int i = 0; i < sauceCount && availableSauces.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, availableSauces.Count);
            chosen.Add(availableSauces[randomIndex]);
            availableSauces.RemoveAt(randomIndex);
        }

        // toppings can appear up to twice
        for (int i = 0; i < toppingCount; i++)
        {
            BurgerIngredients pick = physicalToppings[Random.Range(0, physicalToppings.Count)];

            int existingCount = chosen.FindAll(x => x == pick).Count;
            if (existingCount < 2)
            {
                chosen.Add(pick);
            }
            else
            {
                if (physicalToppings.TrueForAll(t => chosen.FindAll(x => x == t).Count >= 2))
                {
                    break;
                }

                i--;
            }
        }

        return chosen;
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