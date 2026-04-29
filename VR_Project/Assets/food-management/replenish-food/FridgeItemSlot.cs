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

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    private bool isAvailable = true;
    private bool wasTaken = false;

    private void Start()
    {
        if (audioManager == null)
            audioManager = FindFirstObjectByType<AudioManager>();
    }

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

        GameObject spawnedObject = Instantiate(realItemPrefab, spawnPoint.position, spawnPoint.rotation);

        XRGrabInteractable spawnedGrab = spawnedObject.GetComponent<XRGrabInteractable>();
        if (spawnedGrab == null)
        {
            Debug.LogWarning("Spawned real item is missing XRGrabInteractable.");
            Destroy(spawnedObject);
            return;
        }

        if (grabInteractable.interactionManager != null)
        {
            grabInteractable.interactionManager.SelectExit(interactor, grabInteractable);
        }

        SetAvailable(false);
        wasTaken = true;

        if (audioManager != null)
        {
            audioManager.PlayStackConnect();
        }

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
        if (!wasTaken)
            return;

        SetAvailable(true);
        wasTaken = false;
    }

    public bool IsAvailable()
    {
        return isAvailable;
    }

    public bool WasTaken()
    {
        return wasTaken;
    }
}