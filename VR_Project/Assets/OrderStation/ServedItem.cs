using System.Collections.Generic;

//will eventually be used by OrderScore.cs
//judge these people baby

[System.Serializable]
public class ServedItem
{
    public FoodType FoodType { get; }
    public CookLevel ActualCookLevel { get; }
    public int ActualPattyCount { get; }
    public List<BurgerIngredients> ActualIngredients { get; }

    public ServedItem(
        FoodType foodType, 
        CookLevel actualCookLevel, 
        int actualPattyCount, 
        List<BurgerIngredients> actualIngredients = null
    )
    {
        FoodType = foodType;
        ActualCookLevel = actualCookLevel;
        ActualPattyCount = actualPattyCount;
        ActualIngredients = actualIngredients ?? new List<BurgerIngredients>();
    }
}
