using UnityEngine;

public class FridgeRestocker : MonoBehaviour
{
    public FridgeItemSlot[] slots;

    [Header("Audio")]
    [SerializeField] private AudioManager audioManager;

    private bool leftDoorOpen = false;
    private bool rightDoorOpen = false;

    public void SetLeftDoorOpen(bool isOpen)
    {
        if (leftDoorOpen == isOpen)
            return;

        leftDoorOpen = isOpen;
        UpdateFridgeAudio(isOpen);
        TryRestock();
    }

    public void SetRightDoorOpen(bool isOpen)
    {
        if (rightDoorOpen == isOpen)
            return;

        rightDoorOpen = isOpen;
        UpdateFridgeAudio(isOpen);
        TryRestock();
    }

    public void PlayFridgeOpenCreak()
    {
        if (audioManager == null)
        {
            Debug.LogWarning("FridgeRestocker is missing AudioManager reference.");
            return;
        }

        audioManager.PlaySFX(audioManager.fridgeOpenCreak);
        Debug.Log("Fridge open creak played.");
    }

    private void UpdateFridgeAudio(bool doorIsNowOpen)
    {
        if (audioManager == null)
        {
            Debug.LogWarning("FridgeRestocker is missing AudioManager reference.");
            return;
        }

        bool anyDoorOpen = leftDoorOpen || rightDoorOpen;

        if (doorIsNowOpen)
        {
            audioManager.PlayFridgeHumLoop();
            Debug.Log("Fridge hum loop started.");
        }
        else
        {
            audioManager.PlaySFX(audioManager.fridgeClose);
            Debug.Log("Fridge close sound played.");

            if (!anyDoorOpen)
            {
                audioManager.StopFridgeHumLoop();
                Debug.Log("Fridge hum loop stopped.");
            }
        }
    }

    private void TryRestock()
    {
        if (leftDoorOpen || rightDoorOpen)
            return;

        RestockAll();
    }

    public void RestockAll()
    {
        foreach (FridgeItemSlot slot in slots)
        {
            if (slot != null)
                slot.Restock();
        }
    }
}