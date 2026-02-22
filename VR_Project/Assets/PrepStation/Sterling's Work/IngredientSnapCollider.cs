using System.Collections;
using UnityEngine;

public class IngredientSnapCollider : MonoBehaviour
{
    [SerializeField] private StackableIngredient ingredient;
    [SerializeField] private bool isAbove = false;
    
//---------------------------------------------------------------//
/*                      Unity Functions                        */
    private void OnTriggerEnter(Collider other)
    {
        StackableIngredient otherIngredient = other.transform.parent.GetComponent<StackableIngredient>();
        IngredientSnapCollider otherCollider = other.GetComponent<IngredientSnapCollider>();
        if( otherIngredient != null && otherCollider != null && otherCollider.isAbove != this.isAbove)
            ingredient.AttachIngredient(otherIngredient, isAbove);
    }
}
