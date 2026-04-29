// IF YOU WANT TO TEST THE TV UNCOMMENT THIS CODE

using UnityEngine;

public class TVTester : MonoBehaviour
{
    public OrderDisplayManager displayManager;
    private OrderGenerator generator = new OrderGenerator();

    // void Update()
    // {
    //     // Press the 'T' key while the game is running to test the TV
    //     if (Input.GetKeyDown(KeyCode.T))
    //     {
    //         Debug.Log("Testing TV with a random Mode 3 order...");
    //         Order testOrder = generator.GenerateOrder(3, 101, Time.time);
    //         displayManager.UpdateDisplay(testOrder);
    //     }
    // }
}
