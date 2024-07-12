using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Entity 
{

    [Header("References")]
    public MovementData MoveStats;
    public Animator SwipeAnimator;


    [Header("Weapon Data")]
    public WeaponData WeaponData;
    public Transform AttackTransform;
    public Transform AttackUpTransform;
    public Transform AttackDownTransform;
    public CinemachineImpulseSource AttackShake;


    [Header("Ability Data")]
    [SerializeField] ChariotAbilityData chariotAbilityData;


    [Header("Effects")]
    public Transform ParticleSpawnTransform;
    public GameObject JumpParticles;
    public GameObject LandParticles;
    public GameObject StopParticles;
    public ParticleSystem DashParticles;
    public float DashParticleTime;
    public ParticleSystem SpeedParticles;
    public TrailRenderer TrailRenderer;
    public GameObject HitEffect;
    public GameObject SwipeEffect;
    public GhostTrail GhostTrail;


    #region Animation Variables

    public const string IS_WALKING = "isWalking";
    public const string JUMP = "jump";
    public const string LAND = "land";
    public const string FALL = "fall";
    public const string IS_CHARIOT = "isChariot";
    public const string IS_AIR_CHARIOT_FALLING = "isChariotFalling";

    public Utilities.TimeNotifier AttackCounterResetTimeNotifier;

    #endregion


    #region State Machine References

    /// <summary>
    /// Current StateMachine.
    /// </summary>
    public PlayerStateMachine stateMachine;
    /// <summary>
    /// Wrappers to avoid having to call machine.state and its functions
    /// </summary>
    public PlayerState CurrentState => stateMachine.CurrentState;
    public PlayerState PreviousState => stateMachine.PreviousState;
    

    protected void ChangeState(PlayerState newState, bool forceReset = false) 
    => stateMachine.ChangeState(newState, forceReset);


    // state variables
    public PlayerIdleState IdleState;
    // public PlayerWalkState WalkState;
    public PlayerRunState RunState;
    
    public PlayerJumpState JumpState;
    public PlayerWallJumpState WallJumpState;
    
    public PlayerDashState DashState;
    public PlayerLedgeClimbState LedgeClimbState;
    public PlayerWallSlideState WallSlideState;

    public PlayerAirborneState AirborneState;

    public PlayerAttackState AttackState;


    // ability variables
    public AbilityState AbilityOneState;
    public AbilityState AbilityTwoState;
    public AbilityState AbilityThreeState;

    public PlayerChariotState ChariotState;

    #endregion
    


    #region Unity Callback Methods


    private void Awake()
    {
        //SwipeEffect.SetActive(false);
        AttackCounterResetTimeNotifier = new Utilities.TimeNotifier();

        stateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, stateMachine);
        RunState = new PlayerRunState(this, stateMachine);
        
        JumpState = new PlayerJumpState(this, stateMachine);
        WallJumpState = new PlayerWallJumpState(this, stateMachine);
        
        DashState = new PlayerDashState(this, stateMachine);
        LedgeClimbState = new PlayerLedgeClimbState(this, stateMachine);
        WallSlideState = new PlayerWallSlideState(this, stateMachine);
        
        AirborneState = new PlayerAirborneState(this, stateMachine);
        
        AttackState = new PlayerAttackState(this, stateMachine, WeaponData);

        // Ability States
        AbilityOneState = new AbilityState(this, stateMachine);
        AbilityTwoState = new AbilityState(this, stateMachine);
        AbilityThreeState = new AbilityState(this, stateMachine);

        ChariotState = new PlayerChariotState(this, stateMachine, chariotAbilityData);
    }

    public override void Start()
    {
        base.Start();

        Initialize();

        AbilityOneState = ChariotState;
        
        DashState.NumberOfDashesLeft = MoveStats.DashAmount;
        GhostTrail = GetComponent<GhostTrail>();
        AttackShake = GetComponent<CinemachineImpulseSource>();

        stateMachine.InitalizeState(AirborneState);
    }

    protected virtual void Initialize() { }

    private void Update()
    {
        CurrentState.StateUpdate();

        AttackCounterResetTimeNotifier.Tick();
    }

    private void FixedUpdate()
    {
        CurrentState.StateFixedUpdate();
    }

    #endregion


    #region Input Handler

    /// <summary>
    /// Fetches the inputs from <c>UserInput</c>.
    /// </summary>
    public void CheckInput()
    {            

        if(InputManager.JumpJustPressed) OnJumpPressed();
        if(InputManager.JumpReleased) OnJumpReleasedInput();

        if(InputManager.DashInput) OnDashInput();

        // if(UserInput.AbilityOne) OnAbilityOneInput();

        // if(UserInput.GrabInput)        OnGrabInput();

        //if(UserInput.instance.AttackInput)      OnAttackInput();
    }

    #endregion


    #region Timers

    public void Sleep(float duration)
    {
        StartCoroutine(PerformSleep(duration));
    }

    IEnumerator PerformSleep(float duration)
    {
        Time.timeScale = 0;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = 1;
    }

    #endregion


    #region Particles

    public void SpawnParticles(GameObject particleToTransform)
    {
        Instantiate
            (
                particleToTransform,
                ParticleSpawnTransform.position,
                Quaternion.identity
            );
    }

    public void SpawnParticles(GameObject particleToTransform, Quaternion rotation)
    {
        Instantiate
            (
                particleToTransform,
                ParticleSpawnTransform.position,
                rotation
            );
    }

    #endregion


    


    #region Input Callbacks


    /// <summary>
    /// Method called when jump button is pressed.
    /// </summary>
    public void OnJumpPressed()
    {
        JumpState.JumpBufferTimer = MoveStats.JumpBufferTime;
        JumpState.JumpReleasedDuringBuffer = false;
    }

    /// <summary>
    /// Method called when jump button is released.
    /// </summary>
    public void OnJumpReleasedInput()
    {
        if (JumpState.JumpBufferTimer > 0f)
        {
            JumpState.JumpReleasedDuringBuffer = true;
        }

        if (
            AirborneState.IsJumping &&
            Movement.VerticalVelocity > 0f
            )
        {
            if (AirborneState.IsPastApexThreshold)
            {
                AirborneState.IsPastApexThreshold = false;
                AirborneState.IsFastFalling = true;
                AirborneState.FastFallTime = MoveStats.TimeForUpwardsCancel;

                Movement.SetVerticalVelocity(0f);
            }
            else
            {
                AirborneState.IsFastFalling = true;
                AirborneState.FastFallReleaseSpeed = Movement.VerticalVelocity;
            }
        }
    }

    public void OnDashInput()
    {
        DashState.DashBufferTimer = MoveStats.DashInputBufferTime;
    }

    // public void OnAbilityOneInput()
    // {
    //     if (CollisionSensors.IsGrounded && AbilityOneState.CanCheck())
    //     {
    //         ChangeState(AbilityOneState);
    //     }
    //     else if (!CollisionSensors.IsGrounded && AbilityOneState.CanAirCheck())
    //     {
    //         ChangeState(AbilityOneState);
    //     }
    // }

    #endregion


    #region Check Methods

    public void CheckForFalling()
    {
        if (
            !CollisionSensors.IsGrounded &&
            !AirborneState.IsJumping &&
            !AirborneState.IsFalling &&
            !CollisionSensors.IsWall &&
            !AirborneState.IsWallJumping &&
            !DashState.IsDashing &&
            !DashState.IsDashFastFalling &&
            !ChariotState.IsChariot &&
            !ChariotState.IsChariotFastFalling
            )
        {
            if (!AirborneState.IsFalling)
            {
                AirborneState.IsFalling = true;
                Animator.ResetTrigger(LAND);
                Animator.SetTrigger(FALL);
            }
            
            ChangeState(AirborneState);
                
        }
    }

    #endregion


    #region Helper Methods

    public void NotLedgeFalling() => LedgeClimbState.IsLedgeFalling = false;

    #endregion


    #region Jump Visualition Tool

    private void OnDrawGizmos()
    {
        if (MoveStats.ShowWalkJumpArc)
        {
            DrawJumpArc(MoveStats.MaxWalkSpeed, Color.white);
        }

        if (MoveStats.ShowRunJumpArc)
        {
            DrawJumpArc(MoveStats.MaxRunSpeed, Color.red);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackTransform.position, WeaponData.AttackRange);
    }

    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(CollisionSensors.FeetColl.bounds.center.x, CollisionSensors.FeetColl.bounds.min.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;
        if (MoveStats.DrawRight)
        {
            speed = moveSpeed;
        }
        else { speed = -moveSpeed; }
        Vector2 velocity = new Vector2(speed, MoveStats.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * MoveStats.TimeTillJumpApex / MoveStats.ArcResolution;

        for (int i = 0; i < MoveStats.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;
            float downTime = MoveStats.Gravity * MoveStats.GravityOnReleaseMultiplier;

            //ascending
            if (simulationTime < MoveStats.TimeTillJumpApex)
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, MoveStats.Gravity) * simulationTime * simulationTime;
            }

            //apex hang time
            else if (simulationTime < MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime)
            {
                float apexTime = simulationTime - MoveStats.TimeTillJumpApex;
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime;
            }

            //descending
            else
            {
                float descendTime = simulationTime - (MoveStats.TimeTillJumpApex + MoveStats.ApexHangTime);
                displacement = velocity * MoveStats.TimeTillJumpApex + 0.5f * new Vector2(0, MoveStats.Gravity) * MoveStats.TimeTillJumpApex * MoveStats.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * MoveStats.ApexHangTime;

                downTime *= descendTime * descendTime;


                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, downTime);
            }

            drawPoint = startPosition + displacement;

            if (MoveStats.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), MoveStats.GroundLayer);
                if (hit.collider != null)
                {
                    // If a hit is detected, stop drawing the arc at the hit point
                    Gizmos.DrawLine(previousPosition, hit.point);
                    break;
                }
            }

            Gizmos.DrawLine(previousPosition, drawPoint);
            previousPosition = drawPoint;
        }
    }
    
    #endregion
}
