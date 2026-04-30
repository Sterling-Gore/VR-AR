using System.Collections.Generic;
using UnityEngine;

public class FoodReader : MonoBehaviour
{
    public Dictionary<int, DeliverableFood> foodQuery;

    //---------------------------------------------------------------//
    /*                      Unity Functions                          */

    public struct DeliverableFood
    {
        public GameObject gameObject;
        public Component component;

        public DeliverableFood(GameObject _gameObject, Component _component)
        {
            this.gameObject = _gameObject;
            this.component = _component;
        }
    }

    void Start()
    {
        foodQuery = new Dictionary<int, DeliverableFood>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider item)
    {
        Debug.Log("ENTERED");
        Component itemComponent = null;

        itemComponent = item.GetComponentInParent<IngredientStack>();
        if (itemComponent != null)
        {
            GameObject itemGameObject = itemComponent.gameObject;
            int itemId = itemGameObject.GetInstanceID();
            foodQuery[itemId] = new DeliverableFood(itemGameObject, itemComponent);
            return;
        }

        itemComponent = item.GetComponent<FryItem>();
        if (itemComponent != null)
        {
            GameObject itemGameObject = itemComponent.gameObject;
            int itemId = itemGameObject.GetInstanceID();
            foodQuery[itemId] = new DeliverableFood(itemGameObject, itemComponent);
            return;
        }
    }

    void OnTriggerExit(Collider item)
    {
        Debug.Log("EXITED");
        Component itemComponent = null;

        itemComponent = item.GetComponentInParent<IngredientStack>();
        if (itemComponent != null)
        {
            GameObject itemGameObject = itemComponent.gameObject;
            int itemId = itemGameObject.GetInstanceID();
            foodQuery.Remove(itemId);
            _PrintFoodQuery();
            return;
        }

        itemComponent = item.GetComponent<FryItem>();
        if (itemComponent != null)
        {
            GameObject itemGameObject = itemComponent.gameObject;
            int itemId = itemGameObject.GetInstanceID();
            foodQuery.Remove(itemId);
            _PrintFoodQuery();
            return;
        }
    }

    //---------------------------------------------------------------//
    /*                      Public Functions                         */

    [ContextMenu("Deliver Food")]
    public List<ServedItem> DeliverFood()
    {
        List<ServedItem> servedItemsOrder = new List<ServedItem>();

        if (foodQuery.Count == 0)
        {
            this._RaiseNoItemsError();
            return servedItemsOrder;
        }

        foreach (var key in new List<int>(foodQuery.Keys))
        {
            DeliverableFood trackedFood = foodQuery[key];

            if (trackedFood.component is IngredientStack ingredientStack)
            {
                servedItemsOrder.Add(this._GetBurgerIngredients(ingredientStack));
            }
            else if (trackedFood.component is FryItem fryItem)
            {
                servedItemsOrder.Add(this._GetFry(fryItem));
            }

            Destroy(trackedFood.gameObject);
            foodQuery.Remove(key);
        }

        _PrintDeliveredItems(servedItemsOrder);

        return servedItemsOrder;
    }

    //---------------------------------------------------------------//
    /*                      Private Functions                        */

    private ServedItem _GetBurgerIngredients(IngredientStack ingredientStack)
    {
        List<BurgerIngredients> burgerIngredients = new List<BurgerIngredients>();
        List<CookLevel> allCookLevels = new List<CookLevel>();
        int numOfPatties = 0;

        foreach (GameObject ingredientObject in ingredientStack.ingredientStack)
        {
            StackableIngredient ingredient = ingredientObject.GetComponent<StackableIngredient>();

            if (!ingredient.ignoreBelowCondiment)
            {
                BurgerIngredients condiment = ingredient.belowCondiment.GetComponent<CondimentIngredient>().condimentName;

                if (condiment != BurgerIngredients.Null)
                    burgerIngredients.Add(condiment);
            }

            burgerIngredients.Add(ingredient.burgerIngredientName);

            if (!ingredient.ignoreAboveCollider)
            {
                BurgerIngredients condiment = ingredient.aboveCondiment.GetComponent<CondimentIngredient>().condimentName;

                if (condiment != BurgerIngredients.Null)
                    burgerIngredients.Add(condiment);
            }

            if (ingredient.burgerIngredientName == BurgerIngredients.Patty)
            {
                numOfPatties += 1;

                Patty patty = ingredient.GetComponentInChildren<Patty>();

                if (patty != null)
                {
                    CookLevel cookLevel = _ConvertPattyStateToCookLevel(patty.State);
                    allCookLevels.Add(cookLevel);
                }
                else
                {
                    Debug.Log("Patty ingredient does not have a Patty component in children.");
                }
            }
        }

        return new ServedItem(FoodType.Burger, allCookLevels, numOfPatties, burgerIngredients);
    }

    private ServedItem _GetFry(FryItem fryItem)
    {
        FoodType fryType = fryItem.fryType;
        CookLevel cookLevel = _ConvertFryStateToCookLevel(fryItem.State);

        return new ServedItem(fryType, cookLevel);
    }

    private CookLevel _ConvertPattyStateToCookLevel(PattyState pattyState)
    {
        switch (pattyState)
        {
            case PattyState.Raw:
                return CookLevel.Raw;

            case PattyState.Undercooked:
                return CookLevel.Undercooked;

            case PattyState.Cooked:
                return CookLevel.Cooked;

            case PattyState.Overcooked:
                return CookLevel.Overcooked;

            default:
                return CookLevel.Raw;
        }
    }

    private CookLevel _ConvertFryStateToCookLevel(FryState fryState)
    {
        switch (fryState)
        {
            case FryState.Raw:
                return CookLevel.Raw;
            case FryState.Undercooked:
                return CookLevel.Undercooked;
            case FryState.Cooked:
                return CookLevel.Cooked;
            case FryState.Overcooked:
                return CookLevel.Overcooked;
            default:
                return CookLevel.Raw;
        }
    }

    [ContextMenu("No Items Error")]
    private void _RaiseNoItemsError()
    {
        Debug.Log("No Items");
        // takes a text that is not displayed and turns on the gameobject
        // displays text for 2 seconds
        // fades out for 1 second
        // turn off text
    }
    private void _PrintDeliveredItems(List<ServedItem> servedItemsOrder)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("\n------------------------");

        foreach (ServedItem item in servedItemsOrder)
        {
            sb.AppendLine("~~~~~~~~~~~~~~~~");
            FoodType foodType = item.FoodType;
            List<CookLevel> cookLevels = item.ActualCookLevels;
            int pattyCount = item.ActualPattyCount;
            List<BurgerIngredients> ingredients = item.ActualIngredients;

            sb.AppendLine($"Food type: {foodType}");
            sb.AppendLine($"num of patties: {pattyCount}");
            sb.AppendLine("Patty cook levels: ");

            foreach (CookLevel cookLevel in cookLevels)
            {
                sb.AppendLine($"cook level: {cookLevel}");
            }

            foreach (BurgerIngredients ingredient in ingredients)
            {
                sb.AppendLine($"ingredient: {ingredient}");
            }

            sb.AppendLine("~~~~~~~~~~~~~~~~");
        }

        sb.AppendLine("------------------------");
        Debug.Log(sb.ToString());
    }

    [ContextMenu("Show FoodQuery")]
    private void _PrintFoodQuery()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("\n------------------------");

        foreach (var kvp in foodQuery)
        {
            DeliverableFood trackedFood = kvp.Value;
            sb.AppendLine($"ID: {kvp.Key}, GO: {trackedFood.gameObject.name}, Component: {trackedFood.component.GetType().Name}");
        }

        sb.AppendLine("------------------------");
        Debug.Log(sb.ToString());
    }
}