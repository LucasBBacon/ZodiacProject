using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensors : MonoBehaviour
{
    [Header("Check Colliders")]
    public  BoxCollider2D   groundCheck;
    public  BoxCollider2D   wallCheckLeft;
    public  BoxCollider2D   wallCheckRight;

    [Header("Layer Mask")]
    public  LayerMask       groundMask;

    public  bool            IsGrounded {
        get => Physics2D.OverlapAreaAll
        (
            groundCheck.bounds.min,
            groundCheck.bounds.max,
            groundMask
        ).Length > 0;
    }

    public  bool            IsWallLeft {
        get => Physics2D.OverlapAreaAll
        (
            wallCheckLeft.bounds.min,
            wallCheckLeft.bounds.max,
            groundMask
        ).Length > 0;
    }

    public  bool            IsWallRight {
        get => Physics2D.OverlapAreaAll
        (
            wallCheckRight.bounds.min,
            wallCheckRight.bounds.max,
            groundMask
        ).Length > 0;
    }
}