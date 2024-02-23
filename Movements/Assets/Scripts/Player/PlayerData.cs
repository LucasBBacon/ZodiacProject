using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("Gravity")]
    // Downwards force (gravity) needed for the desired jumpHeight and JumpTimeToApex
    [HideInInspector] public float  gravityStrength;
    // Strength of the player's gravity as a multiplier of gravity (set in the project settings)
    [HideInInspector] public float  gravityScale;


    [Space(5)]
    // Multiplier to the player's gravityScale when falling
    public float                    fallGravityMult;
    // Maximum fall speed (terminal velocity) of the player when falling
    public float                    maxFallSpeed;
    [Space(5)]
    // larger multiplier to the player's gravityScale when falling
    public float                    fastFallGravityMult;
    // maximum fall speed (terminal velocity) of the player when performing a faster fall
    public float                    maxFastFallSpeed;


    [Space(20)]


    [Header("Run")]
    // Target speed the player should reach
    public float                    runMaxSpeed;
    // Speed at which the player accelerates to max speed, can be set to runMaxSpeed for instant accellartion down to 0 for none at all
    public float                    runAcceleration;
    // Actual force (multiplied with speedDiff) applied to the player
    [HideInInspector] public float  runAccelAmount;
    // Speed at which the player decelerates from the current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
    public float                    runDecceleration;
    // Actual froce (multiplied with speedDiff) applied to the player
    [HideInInspector] public float  runDeccelAmount;
    [Space(5)]
    // Multipliers applied to acceleartaion rate when airborne
    [Range(0f, 1f)] public float    accelInAir;
    [Range(0f, 1f)] public float    deccelInAir;
    [Space(5)]
    public bool                     doConserveMomentum = true;


    [Space(20)]


    [Header("Jump")]
    // height of a player's jump
    public float                    jumpHeight;
    // time ebween applying the jump force and reaching the desired jump height. Thus value also controls the player's gravity and jump force
    public float                    jumpTimeToApex;
    // atcual force applied (upwards) to the player when they jump
    [HideInInspector] public float  jumpForce;


    [Header("Both Jumps")]
    // multiplier to increase the gravity of the player release the jump button while still jumping
    public float                    jumpCutGravityMult;
    // reduces gravity while close to the apex (desired max height) of the jump
    [Range(0f, 1f)] public float    jumpHangGravityMult;
    // speeds (close ot 0) where the player will experience extra "jump hang", the player''s velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
    public float                    jumpHangTimeThreshold;
    [Space(0.5f)]
    public float                    jumpHangAccelerationMult;
    public float                    jumpHangMaxSpeedMult;


    [Header("Wall Jump")]
    // force (set by user) applied ot the player when wall jumping
    public Vector2                  wallJumpForce;
    [Space(0.5f)]
    // reduces the effect of player's movement while wall jumping
    [Range(0f, 1f)] public float    wallJumpRunLerp;
    // time after wall jumping the player's movement is slowed for
    [Range(0f, 1.5f)] public float  wallJumpTime;
    // player will rotate to face wall jumping direction
    public bool                     doTurnOnWallJump;


    [Space(20)]


    [Header("Slide")]
    public float                    slideSpeed;
    public float                    slideAccel;


    [Space(20)]


    [Header("Dash")]
    public int                      dashAmount;
    public float                    dashSpeed;
    // Duration for which the game freezes when dash is pressed but before the direction input is read and a force is applied
    public float                    dashSleepTime;
    [Space(5)]
    public float                    dashAttackTime;
    [Space(5)]
    // time after the initial drag phase is finished, smoothing the transition back to idle or any other standard state
    public float                    dashEndTime;
    // slows down player, makes dash feel more responsive (like celeste)
    public Vector2                  dashEndSpeed;
    // slows the effect of player movement while dashing
    [Range(0f, 1f)] public float    dashEndRunLerp;
    [Space(5)]
    public float                    dashRefillTime;
    [Space(5)]
    [Range(0.01f, 0.5f)] public float   dashInputBufferTime;

    [Header("Assists")]
    // Grace period after falling off a platform, where player can still jump
    [Range(0.01f, 0.5f)] public float   coyoteTime;
    // grace period after pressing jump where a jump will be automatically performed once the requirements (eg being grounded) are met
    [Range(0.01f, 0.5f)] public float   jumpInputBufferTime;



    /// <summary>
    /// Unity Callback, called when the inspector updates
    /// </summary>
    private void OnValidate() 
    {
        Calculate();
    }

    public void Calculate()
    {
        // calculate gravity strength using the formula gravity = 2 * jumpHeight / timeToJumpApex^2
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        
        // calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value)
        gravityScale    = gravityStrength / Physics2D.gravity.y;

        // calculate trun acceleration and decelration forces using formula: amount = ((1 / fixed time) * acceleration) / runMaxSpeed
        runAccelAmount  = (50 * runAcceleration) / runMaxSpeed;
        runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

        // calculate jumpFoce using the formula: initialJumpVelocity = gravity * timeToJumpApex
        jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;
        
        #region Variable Ranges
        runAcceleration  = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);
        #endregion
    }
}