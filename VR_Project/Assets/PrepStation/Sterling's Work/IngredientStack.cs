using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class IngredientStack : MonoBehaviour
{
    public List<GameObject> ingredientStack;
    public Rigidbody rb;
    public float stackHeight = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void MergeStacks(IngredientStack otherStack, bool isAbove)
    {
        List<GameObject> otherIngredientStack = otherStack.ingredientStack;
        otherStack.ReparentAndDestory(transform);
        if(isAbove)
        {
            foreach(GameObject otherItem in otherIngredientStack)
            {
                SnapIngredient(ingredientStack[ingredientStack.Count-1], otherItem);
                ingredientStack.Add(otherItem);
            }
        }
        else
        {
            foreach(GameObject item in ingredientStack)
            {
                SnapIngredient(otherIngredientStack[otherIngredientStack.Count-1], item);
                otherIngredientStack.Add(item);
            }
            ingredientStack = otherIngredientStack;
        }
        SwapToDeloadCollider();

    }

    private void SnapIngredient(GameObject oldTop, GameObject newTop)
    {
        Vector3 oldTopPos = oldTop.transform.localPosition;
        float oldTopHeight = oldTop.GetComponent<StackableIngredient>().ingredientHeight;
        float newTopHeight = newTop.GetComponent<StackableIngredient>().ingredientHeight;
        newTop.transform.localPosition = new Vector3(oldTopPos.x, oldTopPos.y + ((oldTopHeight + newTopHeight) * 0.5f), oldTopPos.z);
        oldTop.transform.localRotation = Quaternion.identity;
        newTop.transform.localRotation = Quaternion.identity;
    }

    private void SwapToDeloadCollider()
    {
        for (int i = 1; i < ingredientStack.Count - 1; i++)
        {
            StackableIngredient ingredient = ingredientStack[i].GetComponent<StackableIngredient>();
            ingredient.deloadCollider.enabled = true;
            ingredient.meshCollider.enabled = false;
        }
    }

    private void SwapToMeshCollider(StackableIngredient ingredient)
    {
        ingredient.meshCollider.enabled = true;
        ingredient.deloadCollider.enabled = false;
    }

    public void ReparentAndDestory(Transform mergedStack)
    {
        rb.isKinematic = false;
        foreach (GameObject item in ingredientStack)
        {
            item.transform.SetParent(mergedStack);
            item.GetComponent<StackableIngredient>().parentStack = mergedStack.GetComponent<IngredientStack>();
        }
        Destroy(gameObject);
    }
}
