public enum FoodType
{
    Burger,
    Fries
    //Will need more food types for sure as other stations grow
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
    BottomBun,
    TopBun,
    Patty,
    Cheese,
    Ketchup,
    Mustard,
    Mayo,
    Lettuce,
    Tomato
}