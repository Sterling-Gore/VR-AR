using System.Collections;
using UnityEngine;

public class IngredientSnapCollider : MonoBehaviour
{
    [SerializeField] private bool isAbove = false;
    [SerializeField] private BoxCollider triggerBox;
    [SerializeField] private LayerMask detectableLayers = ~0;
    
//---------------------------------------------------------------//
/*                      Unity Functions                        */
    private void Start()
    {
        if(!triggerBox)
            triggerBox = gameObject.GetComponent<BoxCollider>();
    }

//---------------------------------------------------------------//
/*                      Public Functions                        */
    public StackableIngredient CheckTriggerOverlap()
    {
        Vector3 worldCenter = triggerBox.transform.TransformPoint(triggerBox.center);

        Vector3 worldHalfExtents = Vector3.Scale(triggerBox.size * 0.5f, triggerBox.transform.lossyScale);

        Collider[] hits = Physics.OverlapBox(
            worldCenter,
            worldHalfExtents,
            triggerBox.transform.rotation,
            detectableLayers,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider hit in hits)
        {
            // Ignore the trigger's own collider
            if (hit == triggerBox)
                continue;

            // Ignore anything that belongs to the same root object if needed
            if (hit.transform.root == triggerBox.transform.root)
                continue;
            
            IngredientSnapCollider otherCollider = hit.GetComponent<IngredientSnapCollider>();
            StackableIngredient otherIngredient = hit.transform.parent.GetComponent<StackableIngredient>();
            if( otherIngredient != null && otherCollider != null && otherCollider.isAbove != this.isAbove)
            {
                return otherIngredient;
            }
        }
        return null;
    }

}
