using UnityEngine;

public class RigidbodyCollisionDetection : MonoBehaviour
{
    [Header("RigidBody")]
    [SerializeField] private Rigidbody rb = null;

    [Header("Speed thresholds (m/s)")]
    [Tooltip("Switch to ContinuousDynamic when speed exceeds this.")]
    [SerializeField] private float enterCcdSpeed = 6f;
    [Tooltip("Switch back to Discrete when speed falls below this. Should be lower than Enter.")]
    [SerializeField] private float exitCcdSpeed = 4f;
    [Header("Performance")]
    [Tooltip("Check every N FixedUpdate calls. 1 = every FixedUpdate, 2 = every other, etc.")]
    [SerializeField] private int checkEveryNFixedSteps = 2;

    private int stepCounter;

//---------------------------------------------------------------//
/*                      Unity Functions                          */
   void Awake()
    {
        if(rb == null)
            rb = this.gameObject.GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
        stepCounter = 0;
    }
    private void FixedUpdate()
    {
        stepCounter ++;
        if(stepCounter < checkEveryNFixedSteps) return;
        stepCounter = 0;
        if(rb.IsSleeping()) return;

        float speed = Mathf.Sqrt(rb.linearVelocity.sqrMagnitude);
        if (speed >= enterCcdSpeed && rb.collisionDetectionMode == CollisionDetectionMode.Discrete)
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        else if (speed <= exitCcdSpeed && rb.collisionDetectionMode == CollisionDetectionMode.ContinuousDynamic)
            rb.collisionDetectionMode = CollisionDetectionMode.Discrete;

    }
}
