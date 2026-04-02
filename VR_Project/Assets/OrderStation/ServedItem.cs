using System.Collections.Generic;

//will eventually be used by OrderScore.cs
//judge these people baby

[System.Serializable]
public class ServedItem
{
    public FoodType FoodType { get; }
    public CookLevel ActualCookLevel { get; }
    public List<SauceType> ActualSauces { get; }

    public ServedItem(
        FoodType foodType,
        CookLevel actualCookLevel
        //List<SauceType> actualSauces = null
    )
    {
        FoodType = foodType;
        ActualCookLevel = actualCookLevel;

        /*ActualSauces = actualSauces != null
            ? new List<SauceType>(actualSauces)
            : new List<SauceType>();*/
    }
}