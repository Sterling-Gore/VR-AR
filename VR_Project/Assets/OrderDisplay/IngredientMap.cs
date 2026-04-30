using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientMap", menuName = "Orders/IngredientMap")] // this is in the OrderDisplay dir check it out IngredientMap.asset
public class IngredientMap : ScriptableObject
{
    [System.Serializable]
    public struct IngredientUI
    {
        // From the previously used burger enum
        public BurgerIngredients ingredient;
        public Sprite drawing;
    }

    public List<IngredientUI> atlas;

    // Optional fallback sprite if something is missing from the atlas
    public Sprite fallbackSprite;

    public Sprite GetSprite(BurgerIngredients type)
    {
        if (atlas == null)
        {
            Debug.LogWarning($"IngredientMap atlas is missing. Could not find sprite for {type}.");
            return fallbackSprite;
        }

        IngredientUI entry = atlas.Find(x => x.ingredient == type);

        if (entry.drawing == null)
        {
            Debug.LogWarning($"No sprite mapped for ingredient: {type}");
            return fallbackSprite;
        }

        return entry.drawing;
    }
}