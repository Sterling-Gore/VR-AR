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
    public List<BurgerIngredients> Ingredients {get ;}

    public OrderItemRequest(FoodType foodType, CookLevel requiredCookLevel, int pattyCount, List<BurgerIngredients> ingredients)
    {
        FoodType = foodType;
        RequiredCookLevel = requiredCookLevel;
        PattyCount = pattyCount;
        Ingredients = ingredients ?? new List<BurgerIngredients>();
    }

    public OrderItemRequest(FoodType foodType, CookLevel requiredCookLevel, int pattyCount = 1) 
        : this(foodType, requiredCookLevel, pattyCount, new List<BurgerIngredients>()) 
    {
    }
}
