using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensors : MonoBehaviour
{
    [Header("Body Collider")]
    public BoxCollider2D   mainCollider;

    [Header("Check Colliders")]
    public BoxCollider2D   groundCheck;
    public BoxCollider2D   ceilingCheck;
    public BoxCollider2D   wallCheckLeft;
    public BoxCollider2D   wallCheckRight;
    public Vector2 topCheckOffset, topCheckSize;
    public Vector2 middleCheckOffset, middleCheckSize;
    public bool ground;

    [Header("Layer Mask")]
    public LayerMask groundMask;

    private void FixedUpdate() {
        ground = IsGrounded;
    }

    #region Checks

    public bool IsGrounded {
        get => Physics2D.OverlapAreaAll
        (
            groundCheck.bounds.min,
            groundCheck.bounds.max,
            groundMask
        ).Length > 0;
    }

    public  bool            IsCeiling {
        get => Physics2D.OverlapAreaAll
        (
            ceilingCheck.bounds.min,
            ceilingCheck.bounds.max,
            groundMask
        ).Length > 0;
    }

    public bool            IsWallLeft {
        get => Physics2D.OverlapAreaAll
        (
            wallCheckLeft.bounds.min,
            wallCheckLeft.bounds.max,
            groundMask
        ).Length > 0;
    }

    public bool IsWallRight {
        get => Physics2D.OverlapAreaAll
        (
            wallCheckRight.bounds.min,
            wallCheckRight.bounds.max,
            groundMask
        ).Length > 0;
    }

    public bool IsWall {
        get => IsWallRight || IsWallLeft;
    }

    public bool IsTouchingTop {
        get => Physics2D.OverlapBox
            (
                new Vector2
                    (
                        transform.position.x + (topCheckOffset.x * transform.localScale.x),
                        transform.position.y + topCheckOffset.y
                    ),
                topCheckSize,
                0f,
                groundMask
            );
    }

    public bool IsTouchingMiddle {
        get => Physics2D.OverlapBox
            (
                new Vector2
                    (
                        transform.position.x + (middleCheckOffset.x * transform.localScale.x),
                        transform.position.y + middleCheckOffset.y
                    ),
                middleCheckSize,
                0f,
                groundMask
            );
    }

    #endregion

    

    #region Collider Height Methods

    public void SetAllColliderHeight(float height)
    {
        SetColliderHeight(mainCollider, height);
        SetColliderHeight(wallCheckRight, height - 0.05f);
        SetColliderHeight(wallCheckLeft, height - 0.05f);
        //ceilingCheck.gameObject.transform.localPosition = new Vector3(0, height - 0.8f);
    }

    public void SetColliderHeight(BoxCollider2D collider, float height)
    {
        height -= 0.1f;
        Vector2 center  = collider.offset;
        
        Vector2 workspace = new Vector2(collider.size.x, height);

        center.y += (height - collider.size.y)/2;

        collider.size   = workspace;
        collider.offset = center;
    }

    #endregion


    #region Wall Position Methods

    public Vector2 FindWallPos(int facingDirection)
    {
        RaycastHit2D hit = Physics2D.Raycast
            (
                mainCollider.gameObject.transform.position,
                Vector2.right * facingDirection,
                0.5f,
                groundMask
            );

        if(hit.collider != null)
        {
            return hit.point;
        }
        else return new Vector2(0f, 0f);
    }

    #endregion


    #region Debug

    // private void OnDrawGizmos() {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireCube(new Vector2(transform.position.x + (topCheckOffset.x * transform.localScale.x), transform.position.y + topCheckOffset.y), topCheckSize);
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawWireCube(new Vector2(transform.position.x + (middleCheckOffset.x * transform.localScale.x), transform.position.y + middleCheckOffset.y), middleCheckSize);
        
    // }

    #endregion
}
