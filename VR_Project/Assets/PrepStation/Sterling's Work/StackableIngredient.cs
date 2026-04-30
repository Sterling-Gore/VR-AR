using System.Collections.Generic;
using UnityEngine;

public class StackableIngredient : MonoBehaviour
{
    [Header("Ingredient Stack Data")]
    public IngredientStack parentStack;
    public BurgerIngredients burgerIngredientName;
    [Header("Main Ingredient Colliders")]
    [SerializeField] private MeshCollider meshCollider;
    [SerializeField] private BoxCollider deloadCollider;
    [Header("Snap Colliders")]
    [SerializeField] public GameObject aboveSnapCollider;
    [SerializeField] public GameObject belowSnapCollider;
    [SerializeField] public bool ignoreAboveCollider = false;
    [SerializeField] public bool ignoreBelowCollider = false;
    [Header("Condiments")]
    [SerializeField] public GameObject aboveCondiment;
    [SerializeField] public GameObject belowCondiment;
    [SerializeField] public bool ignoreAboveCondiment = false;
    [SerializeField] public bool ignoreBelowCondiment = false;
    

//---------------------------------------------------------------//
/*                      Unity Functions                          */
    private void Awake()
    {
        aboveSnapCollider.SetActive(!ignoreAboveCollider);
        belowSnapCollider.SetActive(!ignoreBelowCollider);
        aboveCondiment.SetActive(!ignoreAboveCondiment);
        belowCondiment.SetActive(!ignoreBelowCondiment);
    }

    private void Start()
    {
        if(parentStack == null)
            parentStack = transform.parent.GetComponent<IngredientStack>();
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

}
