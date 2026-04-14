using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FridgeItemSlot : MonoBehaviour
{
    public GameObject realItemPrefab;
    public Transform spawnPoint;

    public GameObject visualObject;
    public Collider itemCollider;
    public XRGrabInteractable grabInteractable;

    private bool isAvailable = true;

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (!isAvailable || realItemPrefab == null || spawnPoint == null || grabInteractable == null)
            return;

        if (!(args.interactorObject is IXRSelectInteractor interactor))
            return;

        // Spawn the real usable item
        GameObject spawnedObject = Instantiate(realItemPrefab, spawnPoint.position, spawnPoint.rotation);

        XRGrabInteractable spawnedGrab = spawnedObject.GetComponent<XRGrabInteractable>();
        if (spawnedGrab == null)
        {
            Debug.LogWarning("Spawned real item is missing XRGrabInteractable.");
            return;
        }

        // Release dummy first
        if (grabInteractable.interactionManager != null)
        {
            grabInteractable.interactionManager.SelectExit(interactor, grabInteractable);
        }

        // Hide and disable dummy
        SetAvailable(false);

        // Transfer grab to spawned real item
        if (spawnedGrab.interactionManager != null)
        {
            spawnedGrab.interactionManager.SelectEnter(interactor, spawnedGrab);
        }
    }

    public void SetAvailable(bool available)
    {
        isAvailable = available;

        if (visualObject != null)
            visualObject.SetActive(available);

        if (itemCollider != null)
            itemCollider.enabled = available;

        if (grabInteractable != null)
            grabInteractable.enabled = available;
    }

    public void Restock()
    {
        SetAvailable(true);
    }

    public bool IsAvailable()
    {
        return isAvailable;
    }
}