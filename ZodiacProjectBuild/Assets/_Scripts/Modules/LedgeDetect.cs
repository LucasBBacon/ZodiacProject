using UnityEngine;

public class LedgeDetect : MonoBehaviour
{
    [SerializeField] CollisionSensors collisionSensors;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            collisionSensors.UseLedgeCheck = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            collisionSensors.UseLedgeCheck = true;
        }   
    }

    private void OnDrawGizmos() {
        // Gizmos.DrawWireSphere(transform.position, collisionSensors.LedgeCheckRadius);
    }
}
