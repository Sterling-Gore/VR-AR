using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OrderDisplayManager : MonoBehaviour
{
    [Header("Data Source")]
    public IngredientMap atlas;

    [Header("UI Containers")]

     // The BurgerContainer (Right side)
    public Transform burgerStack;

    // The SauceContainer (Left side)
    public Transform sauceList;   

    [Header("Settings")]

     // Your Sticker_Template prefab
    public GameObject iconPrefab;

    // OrderStation will connect to this portion
    public void UpdateDisplay(Order order)
    {
        ClearDisplay();

        foreach (var item in order.RequestedItems)
        {
            if (item.FoodType == FoodType.Burger)
            {
                foreach (var ing in item.Ingredients)
                {
                    if (IsSauce(ing))
                        SpawnSticker(ing, sauceList);
                    else
                        SpawnSticker(ing, burgerStack);
                }
            }
            else 
            {
                // Logic for the fries
                // Convert the FoodType enum to BurgerIngredients to find the sticker in the map
                BurgerIngredients sideType = (BurgerIngredients)System.Enum.Parse(typeof(BurgerIngredients), item.FoodType.ToString());
                SpawnSticker(sideType, burgerStack);
            }
        }
    }

    private bool IsSauce(BurgerIngredients ing)
    {
        return ing == BurgerIngredients.Ketchup || 
               ing == BurgerIngredients.Mustard || 
               ing == BurgerIngredients.Mayo;
    }

    private void SpawnSticker(BurgerIngredients type, Transform container)
    {
        if (iconPrefab == null) return;

        GameObject newIcon = Instantiate(iconPrefab, container);
        Image img = newIcon.GetComponent<Image>();
        
        // Finds drawing from ingredient map
        img.sprite = atlas.GetSprite(type); 
        img.preserveAspect = true;
    }

    private void ClearDisplay()
    {
        foreach (Transform child in burgerStack) Destroy(child.gameObject);
        foreach (Transform child in sauceList) Destroy(child.gameObject);
    }
}
