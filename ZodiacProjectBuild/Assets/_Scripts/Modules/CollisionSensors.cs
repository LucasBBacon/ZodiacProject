using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensors : MonoBehaviour
{
    public  BoxCollider2D   groundCheck;
    public  LayerMask       groundMask;

    public  bool            IsGrounded {
        get => Physics2D.OverlapAreaAll
        (
            groundCheck.bounds.min,
            groundCheck.bounds.max,
            groundMask
        ).Length > 0;
    }

    // public bool WallFront {
    //     get => Physics2D.OverlapAreaAll
    //     (

    //     )
    // }
}