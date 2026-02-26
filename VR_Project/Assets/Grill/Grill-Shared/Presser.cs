using System.Runtime.CompilerServices;
using System.Security;
using UnityEngine;

public class Presser : MonoBehaviour {
    private Patty cookingPatty;

    private void PresserPattyTouch(Collider other) {
        Patty patty = other.GetComponent<Patty>(); 

        if (patty != null) {
            cookingPatty = patty; 
            cookingPatty.ApplyHeat(Time.deltaTime); 
        }
    }

    private void PresserPattyNotTouch(Collider other) {
        cookingPatty = null;
    }
}
