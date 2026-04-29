using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class OrderDisplayManager : MonoBehaviour
{
    [Header("Data Source")]
    public IngredientMap atlas;

    [Header("UI Containers")]
    public Transform burgerStack;
    public Transform sauceList;   
    public Transform friesStack; 

    [Header("Prefabs")]
    public GameObject iconPrefab;
    public GameObject saucePrefab;
    public GameObject friesPrefab;

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
                        SpawnSticker(ing, sauceList, saucePrefab);
                    else
                        SpawnSticker(ing, burgerStack, iconPrefab);
                }
            }
            else // This handles Fries and CrinkleFries
            {
                BurgerIngredients sideType = (BurgerIngredients)System.Enum.Parse(typeof(BurgerIngredients), item.FoodType.ToString());
                
                // USES THE NEW FRIES PREFAB
                SpawnSticker(sideType, friesStack, friesPrefab);
            }
        }
    }

    private bool IsSauce(BurgerIngredients ing)
    {
        return ing == BurgerIngredients.Ketchup || 
               ing == BurgerIngredients.Mustard || 
               ing == BurgerIngredients.Mayo;
    }

    private void SpawnSticker(BurgerIngredients type, Transform container, GameObject prefabToUse)
    {
        if (prefabToUse == null || container == null) return;

        GameObject newIcon = Instantiate(prefabToUse, container);
        Image img = newIcon.GetComponent<Image>();
        
        if (img != null)
        {
            img.sprite = atlas.GetSprite(type); 
            img.preserveAspect = true;
        }
    }

    private void ClearDisplay()
    {
        foreach (Transform child in burgerStack) Destroy(child.gameObject);
        foreach (Transform child in sauceList) Destroy(child.gameObject);
        if (friesStack != null) foreach (Transform child in friesStack) Destroy(child.gameObject);
    }
}
