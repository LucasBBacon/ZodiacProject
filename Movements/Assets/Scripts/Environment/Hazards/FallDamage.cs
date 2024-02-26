using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDamage : MonoBehaviour
{
    //private PlayerHealth playerHealth;

    //private SafeGroundSaver safeGround;
    private SafeGroundCheckPointSaver safeGroundCheckPointSaver;

    private void Start() 
    {
        // safeGround = GameObject.FindGameObjectWithTag("Player").GetComponent<SafeGroundSaver>();
        safeGroundCheckPointSaver = GameObject.FindGameObjectWithTag("Player").GetComponent<SafeGroundCheckPointSaver>();
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player"))
        {
            //playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            

            // damage the player
            //playerHealth.Damage(1f, Vector2.down);

            // warp player to safeground location
            safeGroundCheckPointSaver.WarpPlayerToSafeGround();
        }
    }
}
