using System.Collections.Generic;
using UnityEngine;

public class Fryer : MonoBehaviour
{
    public Transform basketSnapPoint;

    private void OnTriggerEnter(Collider other) // when basket is placed in fryer
    {
        Basket basket = other.GetComponent<Basket>();

        if (basket != null)
        {
            basket.transform.position = basketSnapPoint.position; // snap basket into fryer
            basket.transform.rotation = basketSnapPoint.rotation;
            
            basket.InFryer = true;

            Debug.Log("Basket snapped into fryer");
        }
    }

    private void OnTriggerExit(Collider other) // when basket is removed from fryer
    {
        Basket basket = other.GetComponent<Basket>();

        if (basket != null)
        {
            basket.InFryer = false;
            Debug.Log("Basket removed from fryer");
        }
    }
}