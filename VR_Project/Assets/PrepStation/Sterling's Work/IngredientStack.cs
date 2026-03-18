using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class IngredientStack : MonoBehaviour
{
    public Rigidbody rb = null;
    [SerializeField] [Range(0f, 0.03f)] private float maxOffsetDistance = 0.02f;
    [SerializeField] private bool updateOnStart = true;
    [SerializeField] private GameObject emptyFoodStack;
    [SerializeField] private XRGrabInteractable xrGrab;
    [SerializeField] private List<GameObject> ingredientStack;
    [SerializeField] private IngredientSnapCollider topCollider = null;
    [SerializeField] private IngredientSnapCollider bottomCollider = null;

//---------------------------------------------------------------//
/*                      Unity Functions                          */
   void Awake()
    {
        if(rb == null)
            rb = this.gameObject.GetComponent<Rigidbody>();  
        if(xrGrab == null)
            xrGrab = this.gameObject.GetComponent<XRGrabInteractable>(); 
    }

    void Start()
    {
        if(updateOnStart)
            _UpdateIngredientStack();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && xrGrab.isSelected)
        {
            CheckCollisionForSnap();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && xrGrab.isSelected && ingredientStack.Count > 1)
        {
            RemoveFromStack(0);
        }
    }

//---------------------------------------------------------------//
/*                      Public Functions                         */
    public List<GameObject> GetIngredientStack()
    {
        return this.ingredientStack;
    }

    public void MergeStacks(IngredientStack otherStack, bool isAbove)
    {
        int thisStackIndex = isAbove ? ingredientStack.Count-1 : 0;
        int otherStackIndex = isAbove ? 0 : otherStack.GetIngredientStack().Count-1;
        if(this.ingredientStack.Count > 1)
            this.ingredientStack[thisStackIndex].GetComponent<StackableIngredient>().SwapToDeloadCollider();
        if(otherStack.GetIngredientStack().Count > 1)
            otherStack.GetIngredientStack()[otherStackIndex].GetComponent<StackableIngredient>().SwapToDeloadCollider();

        _SnapStacks(this, otherStack, isAbove);
        ingredientStack = _CombineLists(this.ingredientStack, otherStack.GetIngredientStack(), isAbove);
        otherStack.ReparentAndDestoryEntireStack(this.transform, isAbove);
        _RefreshXRGrab();
    }

    // right now this only works for top and bottom ingredient
    public void RemoveFromStack(int indexToRemove)
    {
        // enable the snap collider of ingredient above (if there is an ingredient above)
        if(indexToRemove < ingredientStack.Count - 1)
        {
            ingredientStack[indexToRemove+1].GetComponent<StackableIngredient>().EnableSnapColliders(aboveCollider:false);
            ingredientStack[indexToRemove+1].GetComponent<StackableIngredient>().SwapToMeshCollider();
        }
        // enable the snap collider of ingredient below (if there is an ingredient below)
        if(indexToRemove > 0)
        {
            ingredientStack[indexToRemove-1].GetComponent<StackableIngredient>().EnableSnapColliders(aboveCollider:true);
            ingredientStack[indexToRemove-1].GetComponent<StackableIngredient>().SwapToMeshCollider();
        }
        ingredientStack[indexToRemove].GetComponent<StackableIngredient>().EnableSnapColliders(aboveCollider:true);
        ingredientStack[indexToRemove].GetComponent<StackableIngredient>().EnableSnapColliders(aboveCollider:false);

        GameObject removedIngredient = ingredientStack[indexToRemove];
        ingredientStack.RemoveAt(indexToRemove);
        GameObject newStackObject = Instantiate(emptyFoodStack, emptyFoodStack.transform.parent);
        newStackObject.name = $"FoodObject_{newStackObject.GetInstanceID()}";
        newStackObject.SetActive(true);
        newStackObject.GetComponent<IngredientStack>().InstantiateNewStack(removedIngredient, emptyFoodStack);
        _RefreshXRGrab();
    }

    /// <summary>
    /// Function only called when the stack is completely empty. 
    /// Takes an ingredient, and positions the ingredient as the origin ingredient for the stack.
    /// </summary>
    /// <param name="newIngredient"></param>
    public void InstantiateNewStack(GameObject newIngredient, GameObject refreshedEmptyFoodStack)
    {
        Vector3 ingredientWorldPos = newIngredient.transform.position;
        Quaternion ingredientWorldRot = newIngredient.transform.rotation;
        this.transform.position = ingredientWorldPos;
        this.transform.rotation = ingredientWorldRot;
    
        ingredientStack.Add(newIngredient);
        newIngredient.transform.SetParent(this.transform);
        newIngredient.GetComponent<StackableData>().SwitchPositionLock(lockedStatus:false);
        newIngredient.transform.position = ingredientWorldPos;
        newIngredient.transform.rotation = ingredientWorldRot;
        newIngredient.GetComponent<StackableData>().SwitchPositionLock(lockedStatus:true);
        _RefreshXRGrab();
        xrGrab.enabled = false;
        xrGrab.enabled = true;
        emptyFoodStack = refreshedEmptyFoodStack;
    }

    public void ReparentAndDestoryEntireStack(Transform newMergedStack, bool isAbove)
    {
        rb.isKinematic = true;

        if(isAbove)
        {
            for (int index = 0; index < ingredientStack.Count; index++)
            {
                GameObject childIngredient = ingredientStack[index];
                childIngredient.transform.SetParent(newMergedStack);
                childIngredient.transform.SetAsLastSibling();
                childIngredient.GetComponent<StackableIngredient>().parentStack = newMergedStack.GetComponent<IngredientStack>();
            } 
        }
        else
        {
            for (int index = ingredientStack.Count-1; index >= 0; index--)
            {
                GameObject childIngredient = ingredientStack[index];
                childIngredient.transform.SetParent(newMergedStack);
                childIngredient.transform.SetAsFirstSibling();
                childIngredient.GetComponent<StackableIngredient>().parentStack = newMergedStack.GetComponent<IngredientStack>();
            } 
        }
        
        Destroy(this.gameObject);
    }

    public void CheckCollisionForSnap()
    {
        StackableIngredient otherIngredient = null;
        bool isAbove = true;

        if(!otherIngredient && topCollider)
        {
            otherIngredient = topCollider.CheckTriggerOverlap();
            isAbove = true;
        }
        if(!otherIngredient && bottomCollider)
        {
            otherIngredient = bottomCollider.CheckTriggerOverlap();
            isAbove = false;
        }

        if(otherIngredient)
        {
            bool dualGrabCheck = otherIngredient.parentStack.xrGrab.isSelected
                ? this.gameObject.GetInstanceID() > otherIngredient.parentStack.gameObject.GetInstanceID()
                : true;
            if(dualGrabCheck)
                MergeStacks(otherIngredient.parentStack, isAbove);
        }
    }

//---------------------------------------------------------------//
/*                      Private Functions                        */
    [ContextMenu("Update Ingredient Stack")]
    private void _UpdateIngredientStack()
    {
        _ConvertIngredientHierarchyToStack();
        _SnapAllIngredients();
        _InitiateSnapColliders();
    }

    private void _ConvertIngredientHierarchyToStack()
    {
        List<GameObject> newIngredientStack = new List<GameObject>();

        foreach (Transform childIngredient in this.transform)
        {
            childIngredient.GetComponent<StackableIngredient>().parentStack = this;
            newIngredientStack.Add(childIngredient.gameObject);
        }
        ingredientStack = newIngredientStack;
    }

    private void _SnapAllIngredients()
    {
        if (ingredientStack.Count > 0)
        {
            GameObject prevIngredient = ingredientStack[0];
            prevIngredient.transform.localPosition = Vector3.zero;
            prevIngredient.transform.localRotation = Quaternion.identity;
            for (int index = 1; index < ingredientStack.Count; index++)
            {
                GameObject currIngredient = ingredientStack[index];
                currIngredient.GetComponent<StackableData>().SwitchPositionLock(false);
                _SnapIngredients(prevIngredient, currIngredient, true);
                currIngredient.GetComponent<StackableData>().SwitchPositionLock(true);
                if (index != ingredientStack.Count - 1)
                {
                    currIngredient.GetComponent<StackableIngredient>().SwapToDeloadCollider();
                }
                prevIngredient = currIngredient;

            }
        }
        // make sure the top and bottom ingredients use a mesh collider and have their snap colliders active
        ingredientStack[0].GetComponent<StackableIngredient>().SwapToMeshCollider();
        ingredientStack[ingredientStack.Count-1].GetComponent<StackableIngredient>().SwapToMeshCollider();
        ingredientStack[0].GetComponent<StackableIngredient>().EnableSnapColliders(aboveCollider:false);
        ingredientStack[ingredientStack.Count-1].GetComponent<StackableIngredient>().EnableSnapColliders(aboveCollider:true);
    }

    private void _InitiateSnapColliders()
    {
        StackableIngredient bottomIngredient = ingredientStack[0].GetComponent<StackableIngredient>();
        StackableIngredient topIngredient = ingredientStack[ingredientStack.Count-1].GetComponent<StackableIngredient>();
        bottomCollider = bottomIngredient.ignoreAboveCollider
            ? null
            : bottomIngredient.belowSnapCollider.GetComponent<IngredientSnapCollider>();
        topCollider = topIngredient.ignoreAboveCollider
            ? null
            : topIngredient.aboveSnapCollider.GetComponent<IngredientSnapCollider>();
    }

    private void _SnapIngredients(GameObject leaderIngredient, GameObject followerIngredient, bool isAbove)
    {
        Vector3 topFacePos;
        Vector3 bottomFacePos;
        followerIngredient.transform.rotation = leaderIngredient.transform.rotation;
        if(isAbove)
        {
            //disable the snap colliders
            leaderIngredient.GetComponent<StackableIngredient>().DisableSnapColliders(aboveCollider:true);
            followerIngredient.GetComponent<StackableIngredient>().DisableSnapColliders(aboveCollider:false);
            //get the position vertex of the faces to be snapped together
            topFacePos = leaderIngredient.GetComponent<StackableData>().GetTopFacePosition();
            bottomFacePos = followerIngredient.GetComponent<StackableData>().GetBottomFacePosition();   
        }
        else
        {
            //disable the snap colliders
            leaderIngredient.GetComponent<StackableIngredient>().DisableSnapColliders(aboveCollider:false);
            followerIngredient.GetComponent<StackableIngredient>().DisableSnapColliders(aboveCollider:true);
            //get the position vertex of the faces to be snapped together
            topFacePos = followerIngredient.GetComponent<StackableData>().GetTopFacePosition();
            bottomFacePos = leaderIngredient.GetComponent<StackableData>().GetBottomFacePosition();   
        }
        _SnapRandomly(leaderIngredient.transform, followerIngredient.transform, topFacePos, bottomFacePos, isAbove);
    }

    private void _SnapStacks(IngredientStack leaderStack, IngredientStack followerStack, bool isAbove)
    {
        followerStack.rb.isKinematic = true;
        Vector3 topFacePos;
        Vector3 bottomFacePos;
        int topIndex;
        int bottomIndex = 0;
        followerStack.transform.rotation = leaderStack.transform.rotation;
        if(isAbove)
        {
            topIndex = leaderStack.GetIngredientStack().Count - 1;
            //disable the snap colliders
            leaderStack.GetIngredientStack()[topIndex].GetComponent<StackableIngredient>().DisableSnapColliders(aboveCollider:true);
            followerStack.GetIngredientStack()[bottomIndex].GetComponent<StackableIngredient>().DisableSnapColliders(aboveCollider:false);
            //get the position vertex of the faces to be snapped together
            topFacePos = leaderStack.GetIngredientStack()[topIndex].GetComponent<StackableData>().GetTopFacePosition();
            bottomFacePos = followerStack.GetIngredientStack()[bottomIndex].GetComponent<StackableData>().GetBottomFacePosition();
        }
        else
        {
            topIndex = followerStack.GetIngredientStack().Count - 1;
            //disable the snap colliders
            leaderStack.GetIngredientStack()[bottomIndex].GetComponent<StackableIngredient>().DisableSnapColliders(aboveCollider:false);
            followerStack.GetIngredientStack()[topIndex].GetComponent<StackableIngredient>().DisableSnapColliders(aboveCollider:true);
            //get the position vertex of the faces to be snapped together
            topFacePos = followerStack.GetIngredientStack()[topIndex].GetComponent<StackableData>().GetTopFacePosition();
            bottomFacePos = leaderStack.GetIngredientStack()[bottomIndex].GetComponent<StackableData>().GetBottomFacePosition();
        }
        _SnapRandomly(leaderStack.transform, followerStack.transform, topFacePos, bottomFacePos, isAbove);
    }

    private void _SnapRandomly(Transform leader, Transform follower, Vector3 topFacePos, Vector3 bottomFacePos, bool isAbove)
    {
        float randomYaw = Random.Range(0f, 259f);
        follower.transform.Rotate(Vector3.up * randomYaw);

        Vector3 randomDir = Vector3.ProjectOnPlane(Random.insideUnitSphere, leader.up).normalized;
        float distance = Random.Range(0f, maxOffsetDistance);
        follower.transform.position = isAbove
            ? topFacePos - (bottomFacePos - follower.transform.position)
            : bottomFacePos - (topFacePos - follower.transform.position);
        follower.transform.position += randomDir * distance;

    }

    private List<GameObject> _CombineLists(List<GameObject> leaderStackList, List<GameObject> followerStackList, bool isAbove)
    {
        List<GameObject> newIngredientStack;
    
        if(isAbove)
        {
            newIngredientStack = leaderStackList;
            newIngredientStack.AddRange(followerStackList);
        }
        else
        {
            newIngredientStack = followerStackList;
            newIngredientStack.AddRange(leaderStackList);
        }
        return newIngredientStack;
    }

    private void _RefreshXRGrab()
    {
        xrGrab.colliders.Clear();
        foreach (Collider col in this.GetComponentsInChildren<Collider>())
        {
            if (!col.isTrigger)
            {
                xrGrab.colliders.Add(col);
            }
        }
    }
}
