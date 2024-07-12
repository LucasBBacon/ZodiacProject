using System.Collections;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    Rigidbody2D effectedBody;
    float forceAmount;

    private void OnEnable()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            effectedBody = collision.GetComponent<Rigidbody2D>();
        }
    }

    // private IEnumerator Push()
    // {
    //     effectedBody.AddForce(Vector2.right * forceAmount, ForceMode2D.Force);
    // }
}