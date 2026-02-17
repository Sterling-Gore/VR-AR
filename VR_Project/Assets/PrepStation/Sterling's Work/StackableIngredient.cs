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

    [HideInInspector] public float ingredientHeight { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ingredientHeight = stackableData.ingredientHeight;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AttachIngredient(StackableIngredient otherIngredient, bool isAbove)
    {
        if(gameObject.GetInstanceID() > otherIngredient.gameObject.GetInstanceID())
        {
            if(isAbove && otherIngredient.GetComponent<StackableIngredient>().belowCollider.enabled)
            {
                aboveCollider.enabled = false;
                otherIngredient.belowCollider.enabled = false;

            }
            else if(!isAbove && otherIngredient.GetComponent<StackableIngredient>().aboveCollider.enabled)
            {
                belowCollider.enabled = false;
                otherIngredient.aboveCollider.enabled = false;
            }
            parentStack.MergeStacks(otherIngredient.parentStack, isAbove);
        }
    }

}
