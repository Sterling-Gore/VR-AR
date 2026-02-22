using System.Collections.Generic;
using UnityEngine;

public class IngredientStack : MonoBehaviour
{
    public Rigidbody rb = null;
    [SerializeField] [Range(0f, 0.03f)] private float maxOffsetDistance = 0.02f;
    [SerializeField] private bool updateOnStart = false;
    private List<GameObject> ingredientStack;

//---------------------------------------------------------------//
/*                      Unity Functions                          */
   void Awake()
    {
        if(rb == null)
            rb = this.gameObject.GetComponent<Rigidbody>();   
    }

    void Start()
    {
        if(updateOnStart)
            _UpdateIngredientStack();
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
            _SwapToDeloadCollider(this.ingredientStack[thisStackIndex].GetComponent<StackableIngredient>());
        if(otherStack.GetIngredientStack().Count > 1)
            _SwapToDeloadCollider(otherStack.GetIngredientStack()[otherStackIndex].GetComponent<StackableIngredient>());

        _SnapStacks(this, otherStack, isAbove);
        ingredientStack = _CombineLists(this.ingredientStack, otherStack.GetIngredientStack(), isAbove);
        otherStack.ReparentAndDestoryEntireStack(this.transform, isAbove);
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

//---------------------------------------------------------------//
/*                      Private Functions                        */
    [ContextMenu("Update Ingredient Stack")]
    private void _UpdateIngredientStack()
    {
        _ConvertIngredientHierarchyToStack();
        _SnapAllIngredients();
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
                    _SwapToDeloadCollider(currIngredient.GetComponent<StackableIngredient>());
                }
                prevIngredient = currIngredient;

            }
        }
    }

    private void _SnapIngredients(GameObject leaderIngredient, GameObject followerIngredient, bool isAbove)
    {
        Vector3 topFacePos;
        Vector3 bottomFacePos;
        followerIngredient.transform.rotation = leaderIngredient.transform.rotation;
        if(isAbove)
        {
            topFacePos = leaderIngredient.GetComponent<StackableData>().GetTopFacePosition();
            bottomFacePos = followerIngredient.GetComponent<StackableData>().GetBottomFacePosition();   
        }
        else
        {
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
            topFacePos = leaderStack.GetIngredientStack()[topIndex].GetComponent<StackableData>().GetTopFacePosition();
            bottomFacePos = followerStack.GetIngredientStack()[bottomIndex].GetComponent<StackableData>().GetBottomFacePosition();
        }
        else
        {
            topIndex = followerStack.GetIngredientStack().Count - 1;
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

    private void _SwapToDeloadCollider(StackableIngredient ingredient)
    {
        ingredient.deloadCollider.enabled = true;
        ingredient.meshCollider.enabled = false;
    }

    private void _SwapToMeshCollider(StackableIngredient ingredient)
    {
        ingredient.meshCollider.enabled = true;
        ingredient.deloadCollider.enabled = false;
    }
}
