
using UnityEngine;

[ExecuteAlways]
public class StackableData : MonoBehaviour
{

    [Header("Gizmo Information")]
    [SerializeField] private bool toggleGizmo = true;
    [SerializeField] [Range(0f, .1f)] private float gizmoSize = 0.1f;

    [Header("Main Ingredient Information")]
    [SerializeField] private GameObject mainIngredient;
    [SerializeField] bool overrideIngredientDistance = false;
    [SerializeField] [Range(0f, .2f)] private float ingredientDistance = 0f;
    
    [Header("Snap Colliders Information")]
    [SerializeField] private GameObject aboveCollider;
    [SerializeField] private GameObject belowCollider; 
    [SerializeField] [Range(0f, .2f)] private float colliderHeight = 0.2f;
    [SerializeField] [Range(0f, 1f)] private float colliderWidth = 0.2f;
    [SerializeField] bool overrideSnapColliderDistance = false;
    [SerializeField] [Range(0f, 1)] private float snapColliderDistance = 0f;
    //--------------------------------------------------------------------------//

    private float snapColliderHeight;
    [HideInInspector] public float ingredientHeight; 

    private void OnDrawGizmos()
    {
        if(toggleGizmo)
        {
            MainIngredientGizmos();
            SnapColliderGizmos();
        }

    }

    private void MainIngredientGizmos()
    {
        Vector3 top = new Vector3(mainIngredient.transform.position.x, mainIngredient.transform.position.y + (ingredientHeight * 0.5f) ,mainIngredient.transform.position.z);
        Vector3 bottom = new Vector3(mainIngredient.transform.position.x, mainIngredient.transform.position.y - (ingredientHeight * 0.5f) ,mainIngredient.transform.position.z);
        
        //Center Gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mainIngredient.transform.position,gizmoSize);
        //Edges Gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(top, gizmoSize);
        Gizmos.DrawSphere(bottom, gizmoSize);
    }

    private void SnapColliderGizmos()
    {
        Vector3 aboveTop = new Vector3(aboveCollider.transform.position.x, aboveCollider.transform.position.y + (snapColliderHeight * 0.5f) ,aboveCollider.transform.position.z);
        Vector3 aboveBottom = new Vector3(aboveCollider.transform.position.x, aboveCollider.transform.position.y - (snapColliderHeight * 0.5f) ,aboveCollider.transform.position.z);
        Vector3 belowTop = new Vector3(belowCollider.transform.position.x, belowCollider.transform.position.y + (snapColliderHeight * 0.5f) ,belowCollider.transform.position.z);
        Vector3 belowBottom = new Vector3(belowCollider.transform.position.x, belowCollider.transform.position.y - (snapColliderHeight * 0.5f) ,belowCollider.transform.position.z);

        //Center Gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(aboveCollider.transform.position,gizmoSize);
        Gizmos.DrawSphere(belowCollider.transform.position,gizmoSize);

        //Edges Gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(aboveTop,gizmoSize);
        Gizmos.DrawSphere(aboveBottom,gizmoSize);
        Gizmos.DrawSphere(belowTop,gizmoSize);
        Gizmos.DrawSphere(belowBottom,gizmoSize);
    }

    private void Awake()
    {
        CalculateIngredientHeight();
        CalculateSnapColliderHeight();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            CalculateIngredientHeight();
            CalculateSnapColliderHeight();
            UpdateColliderScale();
            UpdateColliderDistanceFromMainIngredient();
        }
        
    } 

    private void CalculateIngredientHeight()
    {
        if(!overrideIngredientDistance)
        {
            Bounds bounds = mainIngredient.GetComponent<MeshCollider>().sharedMesh.bounds;
            Vector3 top = mainIngredient.transform.TransformPoint(bounds.center + Vector3.up * bounds.extents.y);
            Vector3 bottom = mainIngredient.transform.TransformPoint(bounds.center + Vector3.down * bounds.extents.y);
            ingredientHeight = Mathf.Abs(top.y - bottom.y);
        }
        else
        {
            ingredientHeight = ingredientDistance*2;
        }
        
    }

    private void CalculateSnapColliderHeight()
    {
        if(!overrideSnapColliderDistance)
        {
            Bounds aboveColliderBounds = aboveCollider.GetComponent<BoxCollider>().bounds;
            Vector3 top = aboveColliderBounds.center + Vector3.up * aboveColliderBounds.extents.y;
            Vector3 bottom = aboveColliderBounds.center + Vector3.down * aboveColliderBounds.extents.y;
            snapColliderHeight = Mathf.Abs(top.y - bottom.y);
        }
        else
        {
            snapColliderHeight = snapColliderDistance*2;    
        }
    }

    private void UpdateColliderScale()
    {
        aboveCollider.transform.localScale = new Vector3(colliderWidth, colliderHeight, colliderWidth);
        belowCollider.transform.localScale = new Vector3(colliderWidth, colliderHeight, colliderWidth);
    }  

    private void UpdateColliderDistanceFromMainIngredient()
    {
        Vector3 abovePosition = aboveCollider.transform.localPosition;
        Vector3 belowPosition = belowCollider.transform.localPosition;
        float distance = (snapColliderHeight * 0.5f) + (ingredientHeight * 0.5f);
        aboveCollider.transform.localPosition = new Vector3(abovePosition.x, distance, abovePosition.z);
        belowCollider.transform.localPosition = new Vector3(belowPosition.x, -1 * distance, belowPosition.z);
    } 
}
