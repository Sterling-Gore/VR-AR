using System.Collections;
using UnityEngine;

public class IngredientSnapCollider : MonoBehaviour
{
    [SerializeField] private StackableIngredient ingredient;
    [SerializeField] private bool isAbove = false;
    
    private void OnTriggerEnter(Collider other)
    {
        StackableIngredient otherIngredient = other.transform.parent.GetComponent<StackableIngredient>();
        if( otherIngredient != null)
            ingredient.AttachIngredient(otherIngredient, isAbove);
    }
}
