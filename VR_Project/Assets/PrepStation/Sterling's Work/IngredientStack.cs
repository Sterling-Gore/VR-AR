using System.Collections.Generic;
using UnityEngine;

public class IngredientStack : MonoBehaviour
{
    public List<GameObject> ingredientStack;
    public Rigidbody rb;

    void Start()
    {
        UpdateIngredientStack();
    }

    public void MergeStacks(IngredientStack otherStack, bool isAbove)
    {
        otherStack.ReparentAndDestory(transform, isAbove);
        UpdateIngredientStack();
    }

    private void UpdateIngredientStack()
    {
        ConvertIngredientHierarchyToStack();
        SnapAllIngredients();
    }

    private void ConvertIngredientHierarchyToStack()
    {
        List<GameObject> instantiatedIngredientList = new List<GameObject>();
        foreach (Transform childIngredient in transform)
        {
            childIngredient.GetComponent<StackableIngredient>().parentStack = this;
            instantiatedIngredientList.Add(childIngredient.gameObject);
        }
        ingredientStack = instantiatedIngredientList;
    }

    private void SnapAllIngredients()
    {
        if (ingredientStack.Count > 0)
        {
            GameObject prevIngredient = ingredientStack[0];
            prevIngredient.transform.localPosition = Vector3.zero;
            prevIngredient.transform.localRotation = Quaternion.identity;
            for (int index = 1; index < ingredientStack.Count; index++)
            {
                GameObject currIngredient = ingredientStack[index];
                SnapIngredient(prevIngredient, currIngredient);
                if (index != ingredientStack.Count - 1)
                {
                    SwapToDeloadCollider(currIngredient.GetComponent<StackableIngredient>());
                }
                prevIngredient = currIngredient;

            }
        }
    }

    private void SnapIngredient(GameObject oldTop, GameObject newTop)
    {
        Vector3 oldTopPos = oldTop.transform.localPosition;
        Quaternion oldTopRot = oldTop.transform.localRotation;
        float oldTopHeight = oldTop.GetComponent<StackableIngredient>().ingredientHeight;
        float newTopHeight = newTop.GetComponent<StackableIngredient>().ingredientHeight;
        newTop.transform.localPosition = new Vector3(oldTopPos.x, oldTopPos.y + ((oldTopHeight + newTopHeight) * 0.5f), oldTopPos.z);
        newTop.transform.localRotation = oldTopRot;
    }

    private void SwapToDeloadCollider(StackableIngredient ingredient)
    {
        ingredient.deloadCollider.enabled = true;
        ingredient.meshCollider.enabled = false;
    }

    private void SwapToMeshCollider(StackableIngredient ingredient)
    {
        ingredient.meshCollider.enabled = true;
        ingredient.deloadCollider.enabled = false;
    }

    public void ReparentAndDestory(Transform newMergedStack, bool isAbove)
    {
        rb.isKinematic = false;

        if(isAbove)
        {
            for (int index = 0; index < ingredientStack.Count; index++)
            {
                GameObject ingredient = ingredientStack[index];
                ingredient.transform.SetParent(newMergedStack);
                ingredient.transform.SetAsLastSibling();
            } 
        }
        else
        {
            for (int index = ingredientStack.Count-1; index >= 0; index--)
            {
                GameObject ingredient = ingredientStack[index];
                ingredient.transform.SetParent(newMergedStack);
                ingredient.transform.SetAsFirstSibling();
            } 
        }
        
        Destroy(gameObject);
    }





    /*
    ///
           -- Potential speed optimization --
           The thought process here is that instead of iterating through every single ingredient to update the positions per merge --> 
           --> we instead will only update the positions of the smaller stack being merged in

           I ran into issues where the positions werent being duplicated correctly --> 
           --> an item would be slightly off center or wildly off center, so it seems there is some issue with only scanning the small stack.
    /// 
    */

    private void BetterMerge(IngredientStack otherStack, bool isAbove)
    {
        List<GameObject> otherIngredientStack = otherStack.ingredientStack;
        otherStack.ReparentAndDestory(transform, isAbove);
        bool otherStackIsLarger = otherIngredientStack.Count > ingredientStack.Count;

        Debug.Log("---------\nMerging");
        Debug.Log("Leader Stack - " + string.Join(", ", ingredientStack));
        Debug.Log("other Stack - " + string.Join(", ", otherIngredientStack));
        Debug.Log("~~~");

        switch((isAbove,otherStackIsLarger))
        {
            case (false, false):
                //otherStack is below and otherStack is smaller
                ingredientStack = MergeStacksBackwards(ingredientStack, otherIngredientStack);
                break;
            case (false, true):
                //otherStack is below and otherStack is larger
                ingredientStack = MergeStacksForwards(otherIngredientStack, ingredientStack);
                break;
            case (true, false):
                //otherStack is above and otherStack is smaller
                ingredientStack = MergeStacksForwards(ingredientStack, otherIngredientStack);
                break;
            case (true, true):
                //otherStack is above and otherStack is larger
                ingredientStack = MergeStacksBackwards(otherIngredientStack, ingredientStack);
                break;
        }

        //AllDeloadCollider();
        Debug.Log("Merged Stack - " + string.Join(", ", ingredientStack));
        Debug.Log("End Merge\n---------");
    }

    private List<GameObject> MergeStacksForwards(List<GameObject> biggerStack, List<GameObject> smallerStack)
    {
        Debug.Log("Forwards Merge");
        Debug.Log(string.Join(", ", biggerStack) + " <--> " + string.Join(", ", smallerStack));
        GameObject belowItem = biggerStack[biggerStack.Count-1];
        foreach (GameObject aboveItem in smallerStack)
        {
            SnapIngredient(belowItem, aboveItem);
            biggerStack.Add(aboveItem);
            belowItem = aboveItem;
        }
        return biggerStack;
    }

    private List<GameObject> MergeStacksBackwards(List<GameObject> biggerStack, List<GameObject> smallerStack)
    {
        Debug.Log("Backwards Merge");
        Debug.Log(string.Join(", ", smallerStack) + " <--> " + string.Join(", ", biggerStack));
        GameObject aboveItem = biggerStack[0];
        for (int index = smallerStack.Count - 1; index >= 0; index--)
        {
            GameObject belowItem = smallerStack[index];
            SnapIngredient(belowItem, aboveItem);
            biggerStack.Insert(0,belowItem);
            aboveItem = belowItem;
        }
        return biggerStack;
    }
}
