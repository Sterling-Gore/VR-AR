using System.Collections.Generic;
using UnityEngine;

public class StackableIngredient : MonoBehaviour
{
    [Header("Main Ingredient Colliders")]
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private BoxCollider deloadCollider;
    [Header("Snap Colliders")]
    [SerializeField] private GameObject aboveSnapCollider;
    [SerializeField] private GameObject belowSnapCollider;
    [SerializeField] private bool ignoreAboveCollider = false;
    [SerializeField] private bool ignoreBelowCollider = false;
    public IngredientStack parentStack;

//---------------------------------------------------------------//
/*                      Unity Functions                          */
    private void Awake()
    {
        aboveSnapCollider.SetActive(!ignoreAboveCollider);
        belowSnapCollider.SetActive(!ignoreBelowCollider);
    }


//---------------------------------------------------------------//
/*                      Public Functions                         */
    public void SwapToDeloadCollider()
    {
        deloadCollider.enabled = true;
        meshCollider.enabled = false;
    }

    public void SwapToMeshCollider()
    {
        meshCollider.enabled = true;
        deloadCollider.enabled = false;
    }

    public void DisableSnapColliders(bool aboveCollider)
    {
        if(aboveCollider)
            aboveSnapCollider.GetComponent<BoxCollider>().enabled = false;
        else
            belowSnapCollider.GetComponent<BoxCollider>().enabled = false;
    }   

    public void EnableSnapColliders(bool aboveCollider)
    {
        if(aboveCollider)
            aboveSnapCollider.GetComponent<BoxCollider>().enabled = true;
        else
            belowSnapCollider.GetComponent<BoxCollider>().enabled = true;
    }
    
    public void AttachIngredient(StackableIngredient otherIngredient, bool isAbove)
    {
        if(this.gameObject.GetInstanceID() > otherIngredient.gameObject.GetInstanceID())
        {
            parentStack.MergeStacks(otherIngredient.parentStack, isAbove);
        }
    }

    [ContextMenu("Detach Ingredient")]
    public void DetachIngredient()
    {
        List<GameObject> ingredientStack = parentStack.GetIngredientStack();
        int stackSize = ingredientStack.Count;

        if (stackSize > 1) // if there is multiple ingredients combined in the stack
        {
            if(aboveSnapCollider.GetComponent<BoxCollider>().enabled == true) // detach the top most ingredient
            {
                parentStack.RemoveFromStack(stackSize-1);
            }
            else if(belowSnapCollider.GetComponent<BoxCollider>().enabled == true) // detach the bottom most ingredient
            {
                parentStack.RemoveFromStack(0);
            }
        }
    }

}
