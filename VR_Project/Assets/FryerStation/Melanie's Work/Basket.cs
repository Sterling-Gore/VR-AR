using UnityEngine;

public class Basket : MonoBehaviour
{
    public bool InFryer = false;

    void Update()
    {
        if (InFryer)
        {
            Debug.Log("Basket is frying!");
        }
    }
}