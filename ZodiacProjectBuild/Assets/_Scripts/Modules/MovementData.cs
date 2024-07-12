using UnityEngine;

[CreateAssetMenu(fileName = "newMovementData", menuName = "Data/Entity Data/Movement Data")]
public class MovementData : ScriptableObject
{
    [Header("Walk")]
    [Range(1f, 100f)]   public float MaxWalkSpeed = 12.5f;
    [Range(0.25f, 50f)] public float GroundAcceleration = 5f;
    [Range(0.25f, 50f)] public float GroundDeceleration = 20f;
    [Range(0.25f, 50f)] public float AirAcceleration = 5f;
    [Range(0.25f, 50f)] public float AirDeceleration = 5f;


    [Header("Run")]
    [Range(1f, 100f)]   public float MaxRunSpeed = 20f;


    [Header("Jump")]
    public float JumpHeight = 6.5f;
    [Range(1f, 1.1f)]
    public float JumpHeightCompensationFactor = 1.054f;
    public float TimeTillJumpApex = 0.35f;
    [Range(0.01f, 5f)]
    public float GravityOnReleaseMultiplier = 2f;
    public float MaxFallSpeed = 26f;
    [Range(1, 5)]
    public int NumberOfJumpsAllowed = 2;

    [Header("Jump Cut")]
    [Range(0.02f, 0.3f)] public float TimeForUpwardsCancel = 0.027f;

    [Header("Jump Apex")]
    [Range(0.5f, 1f)] public float ApexThreshold = 0.97f;
    [Range(0.01f, 1f)] public float ApexHangTime = 0.075f;

    [Header("Jump Time Buffer")]
    [Range(0f, 1f)] public float JumpBufferTime = 0.125f;

    [Header("Jump Coyote Time")]
    [Range(0f, 1f)] public float JumpCoyoteTime = 0.1f;


    [Header("Wall Jump")]
    public float WallJumpVelocity = 20f;
    public float WallJumpTime = 0.4f;
    public Vector2 WallJumpAngle = new Vector2(1, 2);
    public Vector2 WallJumpDirection = new Vector2(-20f, 6.5f);
    [Range(0f, 1f)] public float WallJumpPostBufferTime = 0.125f;
    [Range(0.01f, 5f)] public float WallJumpGravityOnReleaseMultiplier = 1f;


    [Header("Wall Slide")]
    public float WallSlideVelocity = 5f;
    public float WallSlideDecelerationSpeed = 50f;
    public bool ResetJumpsOnWallSlide = false;


    [Header("Ledge Climb")]
    public Vector2 StartOffset;
    public Vector2 EndOffset;


    [Header("Dash")]
    public int DashAmount = 2;
    [Range(0.01f, 0.5f)] public float DashInputBufferTime = 0.1f;
    [Range(0f, 1f)] public float DashTime = 0.11f;
    [Range(1f, 200f)] public float DashSpeed = 40f;
    [Range(0f, 1f)] public float TimeBetweenDashesGround = 0.225f;
    public bool ResetDashOnWallSlide = true;
    [Range(0, 5)] public int NumberOfDashes = 2;
    [Range(0f, 0.5f)] public float DashDiagonallyBias = 0.4f;

    [Header("Dash Cancel Time")]
    [Range(0.01f, 5f)] public float DashGravityOnReleaseMultiplier = 1f;
    [Range(0.02f, 0.3f)] public float DashTimeForUpwardsCancel = 0.027f;

    public readonly Vector2[] DashDirections = new Vector2[]
    {
        new Vector2(0, 0), // nothing
        new Vector2(1, 0), // right
        new Vector2(1, 1).normalized, // top-right
        new Vector2(0, 1), // up
        new Vector2(-1, 1).normalized, // top left
        new Vector2(-1, 0), // left
        new Vector2(-1, -1).normalized, // bottom left
        new Vector2(0, -1), // down
        new Vector2(1, -1).normalized // bottom right
    };


    [Header("Grounded/Collision Checks")]
    public LayerMask GroundLayer;
    public float GroundDetectionRayLength = 0.02f;
    public float HeadDetectionRayLength = 0.02f;
    public float LedgeVerticalDistance = 0.5f;
    [Range(0f, 1f)] public float HeadWidth = 0.75f;


    [Header("Debug")]
    public bool DebugShowIsGroundedBox = true;
    public bool DebugShowHeadBumpBox = true;
    [Range(0f, 1f)] public float TimeScale = 1f;


    [Header("Jump Visualization Tool")]
    public bool ShowWalkJumpArc = false;
    public bool ShowRunJumpArc = false;
    public bool StopOnCollision = true;
    public bool DrawRight = true;
    [Range(5, 180)] public int ArcResolution = 28;
    [Range(0, 580)] public int VisualizationSteps = 90;

    public float Gravity { get; private set; }
    public float InitialJumpVelocity { get; private set; }
    public float AdjustedJumpHeight { get; private set; }

    public float WallJumpGravity { get; private set; }
    public float InitialWallJumpVelocity { get; private set; }
    public float AdjustedWallJumpHeight { get; private set; }

    private void OnValidate() 
    {
        CalculateValues();

        Time.timeScale = TimeScale;
    }

    private void OnEnable() 
    {
        CalculateValues();
    }

    private void CalculateValues()
    {
        AdjustedJumpHeight = JumpHeight * JumpHeightCompensationFactor;
        Gravity = -(2f * AdjustedJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialJumpVelocity = Mathf.Abs(Gravity) * TimeTillJumpApex;

        AdjustedJumpHeight = WallJumpDirection.y * JumpHeightCompensationFactor;
        WallJumpGravity = -(2f * AdjustedWallJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialWallJumpVelocity = Mathf.Abs(WallJumpGravity) * TimeTillJumpApex;
    }
}