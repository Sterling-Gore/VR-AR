using UnityEngine;

public class FridgeRestocker : MonoBehaviour
{
    public FridgeItemSlot[] slots;

    public void RestockAll()
    {
        foreach (FridgeItemSlot slot in slots)
        {
            if (slot != null)
            {
                slot.Restock();
            }
        }
    }
}