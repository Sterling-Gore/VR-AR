using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashZone : MonoBehaviour
{
    [SerializeField] private bool destroyRootObject = true;
    [SerializeField] private bool ignoreCondimentBottles = true;
    [SerializeField] private bool ignorePlayer = true;
    [SerializeField] private string playerRootNameContains = "XR Origin";
    private Collider trashCollider;

    private void Awake()
    {
        trashCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleTrashContact(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null)
            return;

        HandleTrashContact(collision.collider);
    }

    private void HandleTrashContact(Collider other)
    {
        if (other == null)
            return;

        if (ShouldIgnoreTrashTarget(other))
            return;

        if (TryTrashBasketContents(other))
            return;

        if (TryTrashKnownFood(other))
            return;

        var target = GetTargetToDestroy(other);
        if (target != null)
            Destroy(target);
    }

    private bool ShouldIgnoreTrashTarget(Collider other)
    {
        if (other.transform.root == transform.root)
            return true;

        if (ignoreCondimentBottles && other.GetComponentInParent<CondimentData>() != null)
            return true;

        if (ignorePlayer && IsPlayerObject(other))
            return true;

        return false;
    }

    private bool IsPlayerObject(Collider other)
    {
        if (other.CompareTag("LeftHand") || other.CompareTag("RightHand"))
            return true;

        Transform root = other.transform.root;
        if (root == null || string.IsNullOrEmpty(playerRootNameContains))
            return false;

        return root.name.IndexOf(playerRootNameContains, System.StringComparison.OrdinalIgnoreCase) >= 0;
    }

    private bool TryTrashBasketContents(Collider other)
    {
        Basket basket = other.GetComponentInParent<Basket>();
        if (basket == null)
            return false;

        basket.TrashLockedFry();
        return true;
    }

    private bool TryTrashKnownFood(Collider other)
    {
        FryItem fry = other.GetComponentInParent<FryItem>();
        if (fry != null)
        {
            Destroy(fry.gameObject);
            return true;
        }

        Patty patty = other.GetComponentInParent<Patty>();
        if (patty != null)
        {
            Destroy(patty.gameObject);
            return true;
        }

        return false;
    }

    private GameObject GetTargetToDestroy(Collider other)
    {
        if (other.attachedRigidbody != null)
            return destroyRootObject ? other.attachedRigidbody.transform.root.gameObject : other.attachedRigidbody.gameObject;

        return destroyRootObject ? other.transform.root.gameObject : other.gameObject;
    }
}