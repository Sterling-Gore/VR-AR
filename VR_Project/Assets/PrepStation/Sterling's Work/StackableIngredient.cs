using System;
using Unity.VisualScripting;
using UnityEngine;

public class StackableIngredient : MonoBehaviour
{
    public MeshCollider meshCollider;
    public BoxCollider deloadCollider;
    public Collider aboveCollider;
    public Collider belowCollider;
    public IngredientStack parentStack;
    [SerializeField] private StackableData stackableData;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttachIngredient(StackableIngredient otherIngredient, bool isAbove)
    {
        if(this.gameObject.GetInstanceID() > otherIngredient.gameObject.GetInstanceID())
        {
            if(isAbove && otherIngredient.belowCollider.enabled)
            {
                aboveCollider.enabled = false;
                otherIngredient.belowCollider.enabled = false;

            }
            else if(!isAbove && otherIngredient.aboveCollider.enabled)
            {
                belowCollider.enabled = false;
                otherIngredient.aboveCollider.enabled = false;
            }
            parentStack.MergeStacks(otherIngredient.parentStack, isAbove);
        }
    }

    [ContextMenu("Detach Ingredient")]
    public void DetachIngredient()
    {
        // this code is detaching an ingedient at the end -->
        if (parentStack.ingredientStack.Count > 1) // if there is multiple ingredients combined in the stack
        {
            
        }
    }

}
