using System.Collections.Generic;

//will eventually be using by the future OrderGenerator.cs
//randomized orders baby
//i am scared of sauces will not be using

[System.Serializable]
public class OrderItemRequest
{
    public FoodType FoodType { get; }
    public CookLevel RequiredCookLevel { get; }
   // public List<SauceType> RequiredSauces { get; }

    public OrderItemRequest(
        FoodType foodType,
        CookLevel requiredCookLevel
        // List<SauceType> requiredSauces = null
    )
    {
        FoodType = foodType;
        RequiredCookLevel = requiredCookLevel;

        /*RequiredSauces = requiredSauces != null
            ? new List<SauceType>(requiredSauces)
            : new List<SauceType>();*/
    }
}
