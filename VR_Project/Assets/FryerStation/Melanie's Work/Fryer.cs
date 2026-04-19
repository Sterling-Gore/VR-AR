using System.Collections.Generic;
using UnityEngine;

public class Fryer : MonoBehaviour
{
    [Header("Basket Snap Points")]
    [SerializeField] private List<Transform> basketSnapPoints = new List<Transform>();

    private Dictionary<Basket, int> basketSlotAssignments = new Dictionary<Basket, int>();
    private Dictionary<int, Basket> slotOccupants = new Dictionary<int, Basket>();
    private Dictionary<Basket, int> basketOverlapCounts = new Dictionary<Basket, int>();

    private void OnTriggerEnter(Collider other) // when basket is placed in fryer
    {
        Basket basket = other.GetComponentInParent<Basket>();
        if (basket == null)
        {
            return;
        }

        int overlapCount = 0;
        basketOverlapCounts.TryGetValue(basket, out overlapCount);
        overlapCount += 1;
        basketOverlapCounts[basket] = overlapCount;

        if (overlapCount == 1 && !basketSlotAssignments.ContainsKey(basket))
        {
            TryAssignBasketToSlot(basket);
        }
    }

    private void OnTriggerExit(Collider other) // when basket is removed from fryer
    {
        Basket basket = other.GetComponentInParent<Basket>();
        if (basket == null)
        {
            return;
        }

        if (!basketOverlapCounts.TryGetValue(basket, out int overlapCount))
        {
            return;
        }

        overlapCount = Mathf.Max(0, overlapCount - 1);
        if (overlapCount == 0)
        {
            basketOverlapCounts.Remove(basket);
            RemoveBasketFromSlot(basket);
        }
        else
        {
            basketOverlapCounts[basket] = overlapCount;
        }
    }

    private void TryAssignBasketToSlot(Basket basket)
    {
        for (int i = 0; i < basketSnapPoints.Count; i++)
        {
            if (!slotOccupants.ContainsKey(i) || slotOccupants[i] == null)
            {
                AssignBasketToSlot(basket, i);
                return;
            }
        }

        Debug.LogWarning("Fryer: No available snap points for basket.");
    }

    private void AssignBasketToSlot(Basket basket, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= basketSnapPoints.Count)
        {
            Debug.LogError($"Fryer: Invalid slot index {slotIndex}");
            return;
        }

        Transform snapPoint = basketSnapPoints[slotIndex];
        if (snapPoint == null)
        {
            Debug.LogError($"Fryer: Snap point at index {slotIndex} is null");
            return;
        }

        basketSlotAssignments[basket] = slotIndex;
        slotOccupants[slotIndex] = basket;

        basket.transform.position = snapPoint.position;
        basket.transform.rotation = snapPoint.rotation;
        basket.InFryer = true;

        Debug.Log($"Basket snapped to snap point {slotIndex} in fryer");
    }

    private void RemoveBasketFromSlot(Basket basket)
    {
        if (!basketSlotAssignments.TryGetValue(basket, out int slotIndex))
        {
            return;
        }

        slotOccupants.Remove(slotIndex);
        basketSlotAssignments.Remove(basket);
        basket.InFryer = false;

        Debug.Log($"Basket removed from snap point {slotIndex}");
    }
}