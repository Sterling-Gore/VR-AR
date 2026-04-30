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

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    private OrderSystem connectedOrderSystem;

    private void Awake()
    {
        if (audioManager == null)
            audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
    }

    // Connects this display to an OrderSystem instance
    public void ConnectToOrderSystem(OrderSystem orderSystem)
    {
        if (connectedOrderSystem != null)
        {
            connectedOrderSystem.OnOrderCreated -= UpdateDisplay;
        }

        connectedOrderSystem = orderSystem;

        if (connectedOrderSystem != null)
        {
            connectedOrderSystem.OnOrderCreated += UpdateDisplay;
        }
    }

    private void OnDestroy()
    {
        if (connectedOrderSystem != null)
        {
            connectedOrderSystem.OnOrderCreated -= UpdateDisplay;
        }
    }

    public void UpdateDisplay(Order order)
    {
        if (order == null)
        {
            return;
        }

        ClearDisplay();

        foreach (var item in order.RequestedItems)
        {
            if (item.FoodType == FoodType.Burger)
            {
                foreach (var ing in item.Ingredients)
                {
                    if (IsSauce(ing))
                    {
                        SpawnSticker(ing, sauceList, saucePrefab);
                    }
                    else
                    {
                        SpawnSticker(ing, burgerStack, iconPrefab);
                    }
                }
            }
            else // This handles Fries and CrinkleFries
            {
                BurgerIngredients sideType;

                if (System.Enum.TryParse(item.FoodType.ToString(), out sideType))
                {
                    SpawnSticker(sideType, friesStack, friesPrefab);
                }
                else
                {
                    Debug.LogWarning($"No matching BurgerIngredients enum value for food type: {item.FoodType}");
                }
            }
        }
        audioManager?.PlayOrderDisplaySound();
    }

    private bool IsSauce(BurgerIngredients ing)
    {
        return ing == BurgerIngredients.Ketchup ||
               ing == BurgerIngredients.Mustard ||
               ing == BurgerIngredients.Mayo;
    }

    private void SpawnSticker(BurgerIngredients type, Transform container, GameObject prefabToUse)
    {
        if (prefabToUse == null || container == null || atlas == null)
        {
            return;
        }

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
        ClearContainer(burgerStack);
        ClearContainer(sauceList);
        ClearContainer(friesStack);
    }

    private void ClearContainer(Transform container)
    {
        if (container == null)
        {
            return;
        }

        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }
    }
}
