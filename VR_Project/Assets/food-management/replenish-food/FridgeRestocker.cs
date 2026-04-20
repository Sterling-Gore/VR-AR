using UnityEngine;

public class FridgeRestocker : MonoBehaviour
{
    public FridgeItemSlot[] slots;

    private bool leftDoorOpen = false;
    private bool rightDoorOpen = false;

    public void SetLeftDoorOpen(bool isOpen)
    {
        leftDoorOpen = isOpen;
        TryRestock();
    }

    public void SetRightDoorOpen(bool isOpen)
    {
        rightDoorOpen = isOpen;
        TryRestock();
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