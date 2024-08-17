using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionSensors : MonoBehaviour
{
    [SerializeField] Movement movement;
    [SerializeField] PhysicsMaterial2D noFriction;
    [SerializeField] PhysicsMaterial2D fullFriction;

    public bool IsOnSlope { get; private set; }
    public bool CanWalkOnSlope { get; private set; }
    public Vector2 SlopeNormalPerp { get; private set; } 
    public bool UseLedgeCheck { get; set; }
    public bool UseGroundChecks { get; set; } = true;

    public bool IsGrounded {
        get => Physics2D.OverlapCircle(
            GroundCheck.position,
            _groundCheckRadius,
            _groundMask
            );
    }
    public bool IsCeiling { 
        get => Physics2D.OverlapCircle(
            CeilingCheck.position,
            _groundCheckRadius,
            _groundMask
            ); 
    }
    public bool IsWall;

    public bool IsWallBird
    {
        get => Physics2D.Raycast(
            WallCheck.position,
            Vector2.right * movement.FacingDirection,
            _wallCheckDistance,
            _groundMask
            );
    }
    public float WallDetectionRayLength = 0.125f;
    public float WallDetectionRayHeightMultiplier = 0.9f;
    public RaycastHit2D WallHit;
    public RaycastHit2D LastWallHit;
    public bool IsWallLedge {
        get => Physics2D.Raycast(
            WallCheck.position,
            Vector2.right * movement.FacingDirection,
            _wallCheckDistance,
            _ledgeMask
            );
    }
    public bool IsWallBack {
        get => Physics2D.Raycast(
            WallCheck.position,
            Vector2.right * movement.FacingDirection,
            _wallCheckDistance,
            _groundMask
            );
    }
    public bool IsLedgeHorizontal {
        get => Physics2D.Raycast(
            LedgeCheckHorizontal.position,
            Vector2.right * movement.FacingDirection,
            _wallCheckDistance,
            _ledgeMask
            );
    }
    public bool IsLedgeVertical {
        get => Physics2D.Raycast(
            LedgeCheckVertical.position,
            Vector2.down,
            _wallCheckDistance,
            _groundMask
            );
    }

    public float slopeSideAngle;
    public float slopeDownAngle;
    float lastSlopeAngle;


    #region Checks    

    public Transform GroundCheck {
        get => GenericNotImplementedError<Transform>.TryGet(_groundCheck, transform.parent.name);
        private set => _groundCheck = value;
    }

    public Transform CeilingCheck {
        get => GenericNotImplementedError<Transform>.TryGet(_ceilingCheck, transform.parent.name);
        private set => _ceilingCheck = value;
    }

    public Transform WallCheck {
        get => GenericNotImplementedError<Transform>.TryGet(_wallCheck, transform.parent.name);
        private set => _wallCheck = value;
    }

    public Transform LedgeCheckHorizontal {
        get => GenericNotImplementedError<Transform>.TryGet(_ledgeCheckHorizontal, transform.parent.name);
        private set => _ledgeCheckHorizontal = value;
    }

    public Transform LedgeCheckVertical {
        get => GenericNotImplementedError<Transform>.TryGet(_ledgeCheckVertical, transform.parent.name);
        private set => _ledgeCheckVertical = value;
    }

    public float GroundCheckRadius { get => _groundCheckRadius; set => _groundCheckRadius = value; }
    public float WallCheckDistance { get => _wallCheckDistance; set => _wallCheckDistance = value; }
    public LayerMask GroundMask { get => _groundMask;  set => _groundMask = value; }
    public LayerMask LedgeMask { get => _ledgeMask; set => _ledgeMask = value; }

    #endregion

    
    #region Check Transforms

    [Header("Check Colliders")]
    public CapsuleCollider2D BodyColl;
    [SerializeField] Transform _groundCheck;
    [SerializeField] Transform _ceilingCheck;
    [SerializeField] Transform _wallCheck;
    [SerializeField] Transform _ledgeCheckHorizontal;
    [SerializeField] Transform _ledgeCheckVertical;
    [Space(10)]
    [SerializeField] float _groundCheckRadius;
    [SerializeField] float _wallCheckDistance;
    [SerializeField] LayerMask _groundMask;
    [SerializeField] LayerMask _ledgeMask;


    [Header("Slope Check")]
    [SerializeField] float _slopeCheckDistance;
    [SerializeField] float _maxSlopeAngle;

    #endregion


    #region Slope


    public void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)new Vector2(0.0f, 1.59889f / 2f);

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(
            checkPos,
            transform.right,
            _slopeCheckDistance,
            _groundMask
            );
        RaycastHit2D slopeHitBack = Physics2D.Raycast(
            checkPos,
            -transform.right,
            _slopeCheckDistance,
            _groundMask
            );

        if (movement.IsOnPlatform)
        {
            slopeSideAngle = 0.0f;
            IsOnSlope = false;
        }
        else if (slopeHitFront)
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

        RaycastHit2D hit = Physics2D.Raycast(
            checkPos,
            Vector2.down,
            _slopeCheckDistance,
            _groundMask
            );

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

        // if (slopeDownAngle > MaxSlopeAngle)
        if ((slopeDownAngle > _maxSlopeAngle || slopeSideAngle > _maxSlopeAngle) && !Mathf.Approximately(Mathf.Abs(slopeSideAngle), 90f))
        {
            CanWalkOnSlope = false;
        }
        else
        {
            CanWalkOnSlope = true;
        }

        if (
            IsOnSlope
            && CanWalkOnSlope
            && InputManager.instance.MoveInput.x == 0.0f
            )
        {
            //Debug.Log("On Slope");
            movement.Body.sharedMaterial = fullFriction;
        }
        else
        {
            //Debug.Log("Not On Slope");
            movement.Body.sharedMaterial = noFriction;
        }
    }

    #endregion
    
    /*
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
    */

    #region Collision Checks

    public void CollisionChecks()
    {
        CheckForTouchingWall();
    }

    void CheckForGrounded()
    {
        //Vector2 boxCastOrigin = new Vector2(FeetColl.bounds.center.x, FeetColl.bounds.min.y);

    }

    void CheckForTouchingWall()
    {
        float originEndPoint = 0f;

        if (movement.IsFacingRight)
            originEndPoint = BodyColl.bounds.max.x;
        else
            originEndPoint = BodyColl.bounds.min.x;

        float adjustedHeight = BodyColl.bounds.size.y * WallDetectionRayHeightMultiplier;
        Vector2 boxCastOrigin = new Vector2(originEndPoint, BodyColl.bounds.center.y);
        Vector2 boxCastSize = new Vector2(WallDetectionRayLength, adjustedHeight);

        WallHit = Physics2D.BoxCast(
            boxCastOrigin,
            boxCastSize,
            0f,
            transform.right,
            WallDetectionRayLength,
            GroundMask
            );

        if (WallHit.collider != null)
        {
            LastWallHit = WallHit;
            IsWall = true;
        }
        else
        {
            IsWall = false;
        }

        #region Debug Visualization

        Color rayColor;
        if (IsWall)
        {
            rayColor = Color.green;
        }
        else { rayColor = Color.red; }

        Vector2 boxBottomLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
        Vector2 boxBottomRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y - boxCastSize.y / 2);
        Vector2 boxTopLeft = new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);
        Vector2 boxTopRight = new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y + boxCastSize.y / 2);

        Debug.DrawLine(boxBottomLeft, boxBottomRight, rayColor);
        Debug.DrawLine(boxBottomRight, boxTopRight, rayColor);
        Debug.DrawLine(boxTopRight, boxTopLeft, rayColor);
        Debug.DrawLine(boxTopLeft, boxBottomLeft, rayColor);

        #endregion
    }

    #endregion

    private void OnDrawGizmos() {
        if (IsGrounded) Gizmos.color = Color.green;
        else if (!IsGrounded) Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);

        if (IsWall) Gizmos.color = Color.green;
        else if (!IsWall) Gizmos.color = Color.red;
        Gizmos.DrawLine(_wallCheck.position, _wallCheck.position + (Vector3.right * movement.FacingDirection * _wallCheckDistance));
    }
}
