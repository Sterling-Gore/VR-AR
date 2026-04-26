using System.Collections.Generic;

//will eventually be used by OrderScore.cs
//judge these people baby

[System.Serializable]
public class ServedItem
{
    public FoodType FoodType { get; }
    public List<CookLevel> ActualCookLevels { get; }
    public int ActualPattyCount { get; }
    public List<BurgerIngredients> ActualIngredients { get; }

    public ServedItem(
        FoodType foodType,
        List<CookLevel> actualCookLevels,
        int actualPattyCount,
        List<BurgerIngredients> actualIngredients = null
    )
    {
        FoodType = foodType;

        ActualCookLevels = actualCookLevels != null
            ? new List<CookLevel>(actualCookLevels)
            : new List<CookLevel>();

        ActualPattyCount = actualPattyCount;

        ActualIngredients = actualIngredients != null
            ? new List<BurgerIngredients>(actualIngredients)
            : new List<BurgerIngredients>();
    }

    // for foods like fries that only need one cook level
    public ServedItem(FoodType foodType, CookLevel actualCookLevel)
        : this(foodType, new List<CookLevel> { actualCookLevel }, 0, new List<BurgerIngredients>())
    {
    }

    // for a burger with patties
    public ServedItem(
        FoodType foodType,
        CookLevel actualCookLevel,
        int actualPattyCount,
        List<BurgerIngredients> actualIngredients = null
    )
        : this(
            foodType,
            BuildCookLevelList(actualCookLevel, actualPattyCount),
            actualPattyCount,
            actualIngredients
        )
    {
    }

    private static List<CookLevel> BuildCookLevelList(CookLevel cookLevel, int count)
    {
        List<CookLevel> cookLevels = new List<CookLevel>();

        for (int i = 0; i < count; i++)
        {
            cookLevels.Add(cookLevel);
        }

        return cookLevels;
    }
}