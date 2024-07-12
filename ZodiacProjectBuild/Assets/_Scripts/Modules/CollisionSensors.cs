using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensors : MonoBehaviour
{
    [Header("Entity Data")]
    public MovementData MoveData;
    
    [Header("Main Collider")]
    public CapsuleCollider2D mainCollider;

    [Header("Check Colliders")]
    public BoxCollider2D wallCheckLeft;
    public BoxCollider2D wallCheckRight;
    [Space(10)]
    public Collider2D FeetColl;
    public Collider2D HeadColl;
    [Space(10)]
    public GameObject LedgeCheck;
    public float LedgeCheckRadius;
    public Transform LedgeCheckVertical;

    [Header("Layer Masks")]
    public LayerMask groundMask;


    [Header("Slope Check")]
    public float slopeCheckDistance;
    public float MaxSlopeAngle;

    public bool IsOnSlope { get; private set; }
    public bool CanWalkOnSlope { get; private set; }
    public Vector2 SlopeNormalPerp { get; private set; } 
    
    public bool UseLedgeCheck { get; set; }
    public bool UseGroundChecks { get; set; } = true;
    public bool IsGrounded { get; private set; }
    public bool BumpedHead { get; private set; }

    RaycastHit2D _groundHit;
    RaycastHit2D _headHit;
    public RaycastHit2D LastWallHit { get; private set; }
    RaycastHit2D WallHit;

    float slopeSideAngle;
    float slopeDownAngle;
    float lastSlopeAngle;


    #region Checks    

    public bool IsWallBack {
        get => Physics2D.OverlapAreaAll
        (
            wallCheckLeft.bounds.min,
            wallCheckLeft.bounds.max,
            groundMask
        ).Length > 0;
    }

    public bool IsWallFront {
        get => Physics2D.OverlapAreaAll
        (
            wallCheckRight.bounds.min,
            wallCheckRight.bounds.max,
            groundMask
        ).Length > 0;
    }

    public bool IsWall { get; private set; }

    public bool IsLedge {
        get => Physics2D.OverlapCircleAll
            (
                LedgeCheck.transform.position,
                LedgeCheckRadius,
                groundMask
            ).Length > 0 && 
            UseLedgeCheck;
    }

    public bool IsLedgeVertical {
        get => Physics2D.Raycast
            (
                LedgeCheckVertical.position,
                Vector2.down,
                MoveData.LedgeVerticalDistance,
                groundMask
            );
    }

    #endregion


    #region Slope


    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, 2.281606f / 2f));

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundMask);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundMask);

        if (slopeHitFront)
        {
            IsOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
        }
        else if (slopeHitBack)
        {
            IsOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            IsOnSlope = false;
        }

    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {

        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundMask);

        if (hit)
        {

            SlopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;            

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if(slopeDownAngle != lastSlopeAngle || slopeDownAngle != 0)
            {
                IsOnSlope = true;
            }                       

            lastSlopeAngle = slopeDownAngle;
           
            Debug.DrawRay(hit.point, SlopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

        }

        //if (slopeDownAngle > MaxSlopeAngle || slopeSideAngle > MaxSlopeAngle)
        if (slopeDownAngle > MaxSlopeAngle)
        {
            CanWalkOnSlope = false;
        }
        else
        {
            CanWalkOnSlope = true;
        }

        // if (IsOnSlope && CanWalkOnSlope && InputManager.MoveInput.x == 0.0f)
        // {
        //     //Debug.Log("On Slope");
            
        // }
        // else
        // {
        //     //Debug.Log("Not On Slope");
            
        // }
    }

    #endregion


    #region New Collision Checks

    public void CollisionChecks()
    {
        CheckForGrounded();
        CheckForBumpedHead();
        CheckForTouchingWall();
        if (gameObject.transform.parent.CompareTag("Player"))
            SlopeCheck();
    }

    private void CheckForGrounded()
    {
        Vector2 boxCastOrigin = new Vector2
            (
                FeetColl.bounds.center.x,
                FeetColl.bounds.min.y
            );
        Vector2 boxCastSize = new Vector2
            (
                FeetColl.bounds.size.x,
                MoveData.GroundDetectionRayLength
            );

        _groundHit = Physics2D.BoxCast
            (
                boxCastOrigin,
                boxCastSize,
                0f,
                Vector2.down,
                MoveData.GroundDetectionRayLength,
                MoveData.GroundLayer
            );
        if (UseGroundChecks && _groundHit.collider != null)
            IsGrounded = true;
        else
            IsGrounded = false;

        #region Debug Visualization
        if (MoveData.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (IsGrounded)
                rayColor = Color.green;
            
            else 
                rayColor = Color.red; 

            Debug.DrawRay
                (
                    new Vector2
                        (
                            boxCastOrigin.x - boxCastSize.x / 2,
                            boxCastOrigin.y
                        ),
                    Vector2.down * MoveData.GroundDetectionRayLength,
                    rayColor
                );
            Debug.DrawRay
                (
                    new Vector2
                        (
                            boxCastOrigin.x + boxCastSize.x / 2,
                            boxCastOrigin.y
                        ),
                    Vector2.down * MoveData.GroundDetectionRayLength,
                    rayColor
                );
            Debug.DrawRay
                (
                    new Vector2
                        (
                            boxCastOrigin.x - boxCastSize.x / 2,
                            boxCastOrigin.y - MoveData.GroundDetectionRayLength
                        ),
                    Vector2.right * boxCastSize.x,
                    rayColor
                );
        }
        #endregion
    }

    private void CheckForBumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2
            (
                FeetColl.bounds.center.x,
                HeadColl.bounds.max.y
            );
        Vector2 boxCastSize = new Vector2
            (
                FeetColl.bounds.size.x * MoveData.HeadWidth,
                MoveData.HeadDetectionRayLength
            );

        _headHit = Physics2D.BoxCast
            (
                boxCastOrigin,
                boxCastSize,
                0f,
                Vector2.up,
                MoveData.HeadDetectionRayLength,
                MoveData.GroundLayer
            );
        if (UseGroundChecks && _headHit.collider != null)
            BumpedHead = true;
        else 
            BumpedHead = false;

        #region Debug Visualization

        if (MoveData.DebugShowHeadBumpBox)
        {
            float headWidth = MoveData.HeadWidth;

            Color rayColor;
            if (BumpedHead)
                rayColor = Color.green;
            else
                rayColor = Color.red;

            Debug.DrawRay
                (
                    new Vector2
                        (
                            boxCastOrigin.x - boxCastSize.x / 2 * headWidth,
                            boxCastOrigin.y
                        ),
                    Vector2.up * MoveData.HeadDetectionRayLength,
                    rayColor
                );
            Debug.DrawRay
                (
                    new Vector2
                        (
                            boxCastOrigin.x + boxCastSize.x / 2 * headWidth,
                            boxCastOrigin.y
                        ),
                    Vector2.up * MoveData.HeadDetectionRayLength,
                    rayColor
                );
            Debug.DrawRay
                (
                    new Vector2
                        (
                            boxCastOrigin.x - boxCastSize.x / 2 * headWidth,
                            boxCastOrigin.y + MoveData.HeadDetectionRayLength
                        ),
                    Vector2.right * boxCastSize.x * headWidth,
                    rayColor
                );
        }

        #endregion
    }


    private void CheckForTouchingWall()
    {
        float originEndPoint = 0f;
        originEndPoint = mainCollider.bounds.max.x;

        float adjustedHeight = mainCollider.bounds.size.y * 0.9f;
        Vector2 boxCastOrigin = new Vector2(originEndPoint, mainCollider.bounds.center.y);
        Vector2 boxCastSize = new Vector2(0.45f, adjustedHeight);

        WallHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, transform.right, 0.125f, groundMask);
        if (WallHit.collider != null)
        {
            LastWallHit = WallHit;
            IsWall = true;
        }
        else
        {
            IsWall = false;
        }

        #region Debug Visualisation

        Color rayColor;
        if (IsWall)
            rayColor = Color.green;
        else
            rayColor = Color.red;

        Vector2 boxBottomLeft = new(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y  / 2);
        Vector2 boxBottomRight = new(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y  / 2);
        Vector2 boxTopLeft = new(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y  / 2);
        Vector2 boxTopRight = new(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y  / 2);

        Debug.DrawLine(boxBottomLeft, boxBottomRight, rayColor);
        Debug.DrawLine(boxBottomRight, boxTopRight, rayColor);
        Debug.DrawLine(boxTopRight, boxTopLeft, rayColor);
        Debug.DrawLine(boxTopLeft, boxBottomLeft, rayColor);

        #endregion
    }

    #endregion
    

    #region Collider Height Methods

    public void SetAllColliderHeight(float height)
    {
        // SetColliderHeight(mainCollider, height);
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
}
