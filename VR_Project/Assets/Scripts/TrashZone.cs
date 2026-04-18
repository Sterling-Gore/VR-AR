using UnityEngine;

[RequireComponent(typeof(Collider))]
public class TrashZone : MonoBehaviour
{
    [SerializeField] private bool destroyRootObject = true;
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

        if (other.transform.root == transform.root)
            return;

        if (TryTrashBasketContents(other))
            return;

        if (TryTrashKnownFood(other))
            return;

        var target = GetTargetToDestroy(other);
        if (target != null)
            Destroy(target);
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