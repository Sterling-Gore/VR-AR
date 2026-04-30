public enum FoodType
{
    Burger,
    Fries,
    CrinkleFries
}

public enum CookLevel
{
    Raw,
    Undercooked,
    Cooked,
    Overcooked
}

public enum OrderStatus
{
    Active,
    Completed,
    Failed,
    Expired
    //failed represents an incorrect order in time
    //expired represents an order thats not in time
}

public enum BurgerIngredients
{
    Null,
    BottomBun,
    TopBun,
    Patty,
    Cheese,
    Ketchup,
    Mustard,
    Mayo,
    Lettuce,
    Fries, // Strictly for the ingredient mapping to show on the TV
    CrinkleFries, //same for crinkle fries
    Null
}