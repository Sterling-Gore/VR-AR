
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
    [Header("Condiment Information")]
    [SerializeField] private GameObject aboveCondiment;
    [SerializeField] private GameObject belowCondiment;
    [SerializeField] [Range(0f, 0.25f)] private float condimentWidth = 0.2f;
    [SerializeField] [Range(0f,0.1f)] private float condimentDistanceOffset = 0f;
    [SerializeField] bool overrideCondimentDistance = false;
    [SerializeField] [Range(0f, 1)] private float condimentDistance = 0f;
    //--------------------------------------------------------------------------//

    private float snapColliderHeight;
    private float ingredientHeight; 
    private bool positionLocked = true;
    private Vector3 lockedPosition;
    private Quaternion lockedRotation;

//---------------------------------------------------------------//
/*                      Unity Functions                          */
    private void Awake()
    {
        lockedPosition = transform.localPosition;
        lockedRotation = transform.localRotation;
        _CalculateIngredientHeight();
        _CalculateSnapColliderHeight();
        _SizeDeloadingBoxCollider();
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            _CalculateIngredientHeight();
            _CalculateSnapColliderHeight();
            _UpdateCondimentScale();
            _UpdateCondimentDistanceFromMainIngredient();
            _UpdateColliderScale();
            _UpdateColliderDistanceFromMainIngredient();
            _SizeDeloadingBoxCollider();
            _LockPositionAndRotationOfModels();
        }

    }

    private void OnDrawGizmos()
    {
        if(toggleGizmo)
        {
            _MainIngredientGizmos();
            _SnapColliderGizmos();
        }

    }

//---------------------------------------------------------------//
/*                      Public Functions                         */
    public float GetIngredientHeight()
    {
        return this.ingredientHeight;
    }

    public Vector3 GetIngredientPosition()
    {
        return mainIngredient.transform.position;
    }

    public Vector3 GetTopFacePosition()
    {
        return mainIngredient.transform.position + (mainIngredient.transform.up * (ingredientHeight* 0.5f));
    }

    public Vector3 GetBottomFacePosition()
    {
        return mainIngredient.transform.position - (mainIngredient.transform.up * (ingredientHeight* 0.5f));
    }

    public void SwitchPositionLock(bool lockedStatus)
    {
        lockedPosition = transform.localPosition;
        lockedRotation = transform.localRotation;
        positionLocked = lockedStatus;
    }

//---------------------------------------------------------------//
/*                      Private Functions                        */
    private void _MainIngredientGizmos()
    {
        Vector3 top = mainIngredient.transform.position + (mainIngredient.transform.up * (ingredientHeight* 0.5f));
        Vector3 bottom = mainIngredient.transform.position - (mainIngredient.transform.up * (ingredientHeight* 0.5f));

        //Center Gizmo
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mainIngredient.transform.position,gizmoSize);
        //Edges Gizmo
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(top, gizmoSize);
        Gizmos.DrawSphere(bottom, gizmoSize);
    }

    private void _SnapColliderGizmos()
    {
        Vector3 aboveTop = aboveCollider.transform.position + (aboveCollider.transform.up * (snapColliderHeight * 0.5f));
        Vector3 aboveBottom = aboveCollider.transform.position - (aboveCollider.transform.up * (snapColliderHeight * 0.5f));
        Vector3 belowTop = belowCollider.transform.position + (belowCollider.transform.up * (snapColliderHeight * 0.5f));
        Vector3 belowBottom = belowCollider.transform.position - (belowCollider.transform.up * (snapColliderHeight * 0.5f));

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

    private void _CalculateIngredientHeight()
    {
        if(!overrideIngredientDistance)
        {
            Bounds bounds = mainIngredient.GetComponent<MeshCollider>().sharedMesh.bounds;
            Vector3 top = mainIngredient.transform.TransformPoint(bounds.center + Vector3.up * bounds.extents.y);
            Vector3 bottom = mainIngredient.transform.TransformPoint(bounds.center + Vector3.down * bounds.extents.y);
            ingredientHeight = Mathf.Abs( Vector3.Dot(top - bottom, mainIngredient.transform.up) );
        }
        else
        {
            ingredientHeight = ingredientDistance*2;
        }

    }

    private void _CalculateSnapColliderHeight()
    {
        if(!overrideSnapColliderDistance)
        {
            Vector3 size = aboveCollider.GetComponent<BoxCollider>().size;
            snapColliderHeight = size.y * aboveCollider.transform.lossyScale.y;
        }
        else
        {
            snapColliderHeight = snapColliderDistance*2;
        }
    }

    private void _UpdateColliderScale()
    {
        aboveCollider.transform.localScale = new Vector3(colliderWidth, colliderHeight, colliderWidth);
        belowCollider.transform.localScale = new Vector3(colliderWidth, colliderHeight, colliderWidth);
    }

    private void _UpdateCondimentScale()
    {
        float height = aboveCondiment.transform.localScale.y;
        aboveCondiment.transform.localScale = new Vector3(condimentWidth, height, condimentWidth);
        belowCondiment.transform.localScale = new Vector3(condimentWidth, height, condimentWidth);    
    }

    private void _SizeDeloadingBoxCollider()
    {
        Bounds bounds = mainIngredient.GetComponent<MeshCollider>().sharedMesh.bounds;
        float radius = Mathf.Min(bounds.extents.x, bounds.extents.y);
        Vector3 colliderSize = mainIngredient.GetComponent<BoxCollider>().size;
        mainIngredient.GetComponent<BoxCollider>().size = new Vector3(radius*Mathf.Sqrt(2), colliderSize.y, radius*Mathf.Sqrt(2));
    }

    private void _UpdateColliderDistanceFromMainIngredient()
    {
        Vector3 abovePosition = aboveCollider.transform.localPosition;
        Vector3 belowPosition = belowCollider.transform.localPosition;
        float distance = (snapColliderHeight * 0.5f) + (ingredientHeight * 0.5f);
        aboveCollider.transform.localPosition = new Vector3(abovePosition.x, distance, abovePosition.z);
        belowCollider.transform.localPosition = new Vector3(belowPosition.x, -1 * distance, belowPosition.z);
    }

    private void _UpdateCondimentDistanceFromMainIngredient()
    {
        Vector3 abovePosition = aboveCondiment.transform.localPosition;
        Vector3 belowPosition = belowCondiment.transform.localPosition;
        float distance = overrideCondimentDistance
            ? condimentDistance
            : (ingredientHeight * 0.5f) + condimentDistanceOffset;
        aboveCondiment.transform.localPosition = new Vector3(abovePosition.x, distance, abovePosition.z);
        belowCondiment.transform.localPosition = new Vector3(belowPosition.x, -1 * distance, belowPosition.z);
    }

    private void _LockPositionAndRotationOfModels()
    {
        if(positionLocked)
        {
            this.transform.localPosition = lockedPosition;
            this.transform.localRotation = lockedRotation;
        }
        mainIngredient.transform.localPosition = Vector3.zero;
        mainIngredient.transform.localRotation = Quaternion.identity;
    }
}
