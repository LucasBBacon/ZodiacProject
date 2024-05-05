using UnityEngine;

[CreateAssetMenu(fileName = "Default Player", menuName = "SO Data/Player Data")]
public class PlayerData : EntityData
{
    #region Run Parameters

    [Header("Run")]
    /// <summary>
    /// Target speed for acceleration to reach.
    /// </summary>
    public float   runMaxSpeed                      = 9.5f;
    /// <summary>
    /// If momentum carries over to RigidBody2D.
    /// </summary>
    public bool    doConserveMomentum               = true;
    /// <summary>
    /// Acceleration (multiplied with <c>speedDiff</c>) that is applied to the RigidBody2D.
    /// </summary>
    public float   runAcceleration                  = 9.5f;
    /// <summary>
    /// Decceleration (multiplied with <c>speedDiff</c>) that is applied to the RigidBody2D.
    /// </summary>
    public float   runDecceleration                 = 9.5f;
    [Space(5)]
    /// <summary>
    /// Multiplier applied to acceleration while in air.
    /// </summary>
    [Range(0f, 1f)]
    public float   airAcceleration                  = 1f;
    /// <summary>
    /// Multiplier applied to deccelration while in air.
    /// </summary>
    [Range(0f, 1f)]
    public float   airDecceleration                 = 1f;
    [HideInInspector]
    public float   runAccelAmount;
    [HideInInspector]
    public float   runDeccelAmount;

    #endregion
    

    [Space(20)]


    #region Jump Parameters

    [Header("Jump")]
    /// <summary>
    /// Number of jumps that can be done before touching ground.
    /// </summary>
    public  int     jumpAmount                      = 1;
    /// <summary>
    /// Height the RigidBody2D reaches at apex (max height).
    /// </summary>
    public  float   jumpHeight                      = 6.5f;
    /// <summary>
    /// Time taken from start of jump to reaching jump apex.
    /// </summary>
    public  float   jumpTimeToApex                  = 0.5f;
    [HideInInspector]
    public  float   jumpForce;
    
    #endregion


    [Space(20)]


    #region Walljump Parameters

    [Header("Walljump")]
    public  Vector2 wallJumpForce                   = new Vector2(8.5f, 20f);
    [Space(5)]
    
    [Range(0f, 1f)]
    public  float   wallJumpRunLerp                 = 0.075f;
    [Range(0f, 1.5f)]
    public  float   wallJumpTime                    = 0.3f;
    [Space(5)]
    public  bool    wallJumpDoTurn                  = false;

    #endregion


    [Space(20)]


    #region Slide Parameters

    [Header("Slide")]
    public float slideSpeed                         = -12f;
    public float slideAcceleration                  = 12f;

    #endregion


    [Space(20)]


    #region Jump Dynamic Parameters

    [Header("Jump Dynamics")]
    ///<summary>
    /// Multiplier to reduce gravity by at apex of jump.
    ///</summary>
    [Range(0f, 1f)]
    public  float   jumpHangGravityMultiplier       = 1f;
    /// <summary>
    /// Threshold speed where "Jump Hang" will be experienced, usually close to 0 at the apex of jump.
    /// </summary>
    public  float   jumpHangTimeThreshold           = 0f;
    /// <summary>
    /// Multiplier for RigidBody2D acceleration during "Jump Hang".
    /// </summary>
    public  float   jumpHangAccelerationMultiplier  = 1f;
    /// <summary>
    /// Maximum speed player can reach during "Jump Hang".
    /// </summary>
    public  float   jumpHangMaxSpeedMultiplier      = 1f;
    [Space(5)]
    /// <summary>
    /// Multiplier increase in RigidBody2D gravity when jump button is released.
    /// </summary>
    public  float   jumpCutGravityMultiplier        = 3.5f;

    #endregion


    [Space(20)]


    #region Dash Parameters

    [Header("Dash")]
    public  int     dashAmount                      = 1;
    public  float   dashSpeed                       = 20f;
    public  float   dashSleepTime                   = 0.05f;
    public  float   dashAttackTime                  = 0.15f;
    public  float   dashEndTime                     = 0.15f;
    public  Vector2 dashEndSpeed                    = new Vector2(15f, 15f);
    [Range(0f, 1f)]
    public  float   dashEndRunLerp                  = 0.5f;
    public  float   dashRefillTime                  = 0.1f;

    #endregion


    [Space(20)]


    #region Assist Parameters

    [Header("Assists")]
    /// <summary>
    /// Time window in which jump input will be recognised before touching the ground.
    /// </summary>
    public float    jumpInputBufferTime             = 0.2f;
    [Range(0.01f, 0.5f)]
    public  float   dashInputBufferTime             = 0.1f;
    public  float   grabInputBufferTime             = 0.1f;
    /// <summary>
    /// Time window in which jump input will be recognised after leaving ground.
    /// </summary>
    public float    coyoteTime                      = 0.2f;

    #endregion

    
    [Space(20)]

    
    #region Gravity Parameters
    
    [Header("Gravity")]
    /// <summary>
    /// Multiplier increase in gravity while free falling.
    /// </summary>
    public float    fallGravityMultiplier           = 2;
    /// <summary>
    /// Maximum speed while free falling (terminal velocity).
    /// </summary>
    public float    fallMaxSpeed                    = 18;
    /// <summary>
    /// Multiplier increase in gravity while fast falling.
    /// </summary>
    public float    fallGravityMultiplierFast       = 1;
    /// <summary>
    /// Maximum speed while fast falling (fast terminal velocity).
    /// </summary>
    public float    fallMaxSpeedFast                = 20;
    [HideInInspector]
    public float    gravityStrength;
    [HideInInspector]
    public float    gravityScale;

    #endregion

    #region Variable Calculations

    /// <summary>
    /// Unity callback function, called to calculate certain variables.
    /// </summary>
    private void OnValidate()
    {
        Calculate();
    }

    /// <summary>
    /// Calculates certain variables based on inputs.
    /// </summary>
    public void Calculate()
    {
        // Calculation for desired gravity strength based on the desired jump height and time to apex.
        gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
        // Caculate the RigidBody2D gravity needed to reach desired gravity strength.
        gravityScale    = gravityStrength / Physics2D.gravity.y;

        // Calculate turn acceleration and deceleration forces using: amount = ((1 / fixed time) * acceleration) / runMaxSpeed
        runAccelAmount  = 50 * runAcceleration / runMaxSpeed;
        runDeccelAmount = 50 * runDecceleration / runMaxSpeed;

        runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
        runDecceleration= Mathf.Clamp(runDecceleration, 0.1f, runMaxSpeed);

        jumpForce       = Mathf.Abs(gravityStrength) * jumpTimeToApex;
    }

    #endregion
}