using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientMap", menuName = "Orders/IngredientMap")] // this is in the OrderDisplay dir check it out IngrdientMap.asset
public class IngredientAtlas : ScriptableObject
{
    [System.Serializable]
    public struct IngredientUI
    {
        // From the previously used burger enum
        public BurgerIngredients ingredient;
        public Sprite drawing;
    }

    public List<IngredientUI> atlas;


    public Sprite GetSprite(BurgerIngredients type)
    {
        var entry = atlas.Find(x => x.ingredient == type);
        return entry.drawing;
    }
}