using UnityEngine;

[CreateAssetMenu(fileName = "newMovementData", menuName = "Data/Entity Data/Movement Data")]
public class SOMovementData : ScriptableObject
{
    // [Range(0.25f, 50f)] public float AirAcceleration = 5f;
    // [Range(0.25f, 50f)] public float AirDeceleration = 5f;

    [Header("Run")]
    [Range(0f, 1f)] public float MoveThreshold = 0.25f;
    [Range(1f, 100f)] public float MaxRunSpeed = 12.5f;
    [Range(0.25f, 50f)] public float GroundAcceleration = 5f;
    [Range(0.25f, 50f)] public float GroundDeceleration = 20f;
    [Range(0.25f, 50f)] public float AirAcceleration = 5f;
    [Range(0.25f, 50f)] public float AirDeceleration = 5f;
    [Range(0.25f, 50f)] public float WallJumpMoveAcceleration = 5f;
    [Range(0.25f, 50f)] public float WallJumpMoveDeceleration = 5f;


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


    [Header("Stun")]
    public float StunTime = 2f;

    [Header("Wall Jump")]
    public Vector2 WallJumpDirection = new Vector2(-10f, 6.5f);
    [Range(0f, 1f)] public float WallJumpPostBufferTime = 0.125f;
    [Range(0.01f, 5f)] public float WallJumpGravityOnReleaseMultiplier = 1f;


    [Header("Wall Slide")]
    public float WallSlideVelocity = 3f;
    public float WallSlideDecelerationSpeed = 50f;
    public bool ResetJumpsOnWallSlide = false;


    [Header("Ledge Climb")]
    public Vector2 StartOffset = new(0.4f, 0.65f);
    public Vector2 EndOffset = new(0.5f, 1f);


    [Header("Dash")]
    [Range(0, 5)] public int NumberOfDashes = 2;
    [Range(1f, 200f)] public float DashSpeed = 40f;
    [Range(1f, 200f)] public float DashEndSpeed = 20f;
    [Range(0f, 1f)] public float DashAttackTime = 0.11f;
    [Range(0f, 1f)] public float DashEndTime = 0.05f;
    [Range(0f, 0.5f)] public float DashSleepTime = 0.04f;
    [Range(0f, 5f)] public float DashRefillTime = 0.225f;

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

        AdjustedWallJumpHeight = WallJumpDirection.y * JumpHeightCompensationFactor;
        WallJumpGravity = -(2f * AdjustedWallJumpHeight) / Mathf.Pow(TimeTillJumpApex, 2f);
        InitialWallJumpVelocity = Mathf.Abs(WallJumpGravity) * TimeTillJumpApex;
    }
}