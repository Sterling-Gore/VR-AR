using System.Collections.Generic;

//will eventually be using by the future OrderGenerator.cs
//randomized orders baby
//i am scared of sauces will not be using

[System.Serializable]
public class OrderItemRequest
{
    public FoodType FoodType { get; }
    public CookLevel RequiredCookLevel { get; }
    public int PattyCount { get; }
    public List<BurgerIngredients> Ingredients { get; }

    public OrderItemRequest(FoodType foodType, CookLevel requiredCookLevel, int pattyCount, List<BurgerIngredients> ingredients)
    {
        FoodType = foodType;
        RequiredCookLevel = requiredCookLevel;
        PattyCount = pattyCount;
        Ingredients = ingredients != null
            ? new List<BurgerIngredients>(ingredients)
            : new List<BurgerIngredients>();
    }

    // for foods like fries that do not use patties or ingredients
    public OrderItemRequest(FoodType foodType, CookLevel requiredCookLevel)
        : this(foodType, requiredCookLevel, 0, new List<BurgerIngredients>())
    {
    }
}