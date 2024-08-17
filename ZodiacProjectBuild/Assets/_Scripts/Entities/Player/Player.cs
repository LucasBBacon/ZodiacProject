using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Player : Entity 
{

    #region References

    [Header("References")]
    public SOMovementData MoveData;
    // public Animator SwipeAnimator;


    [Header("Weapon Data")]
    public AnimationEventHandler EventHandler;
    public SOWeaponData WeaponData;
    public Transform AttackTransform;
    public Transform AttackUpTransform;
    public Transform AttackDownTransform;
    public CinemachineImpulseSource AttackShake;


    [Header("Ability Data")]
    [SerializeField] SOChariotAbilityData chariotAbilityData;
    [SerializeField] SOStarAbilityData starAbilityData;


    [Header("Effects")]
    public Transform ParticleSpawnTransform;
    public ParticleSystem JumpParticles;
    public GameObject JumpDustParticles;
    public float DustFormationPeriod = 0.04f;
    public GameObject LandParticles;
    public ParticleSystem DashParticles;
    public float DashParticleTime;
    public ParticleSystem SpeedParticles;
    public TrailRenderer TrailRenderer;
    public GameObject HitEffect;
    public GhostTrail GhostTrail;
    public ParticleSystem WallSlideParticles;

    [Header("Screen Shake")]
    public CinemachineImpulseSource CameraShakeHold;
    public CinemachineImpulseSource CollisionShake;

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

    #region Base States

    // state variables
    public PlayerIdleState IdleState;
    // public PlayerWalkState WalkState;
    public PlayerRunState RunState;
    
    public PlayerJumpState JumpState;
    public PlayerLandState LandState;
    public PlayerWallJumpState WallJumpState;
    
    public PlayerDashState DashState;
    public PlayerLedgeClimbState LedgeClimbState;
    public PlayerWallSlideState WallSlideState;

    public PlayerAirborneState AirborneState;

    public PlayerAttackState AttackState;
    public PlayerBlockState BlockState;
    public PlayerDeadState DeadState;

    #endregion


    #region Ability States

    // ability variables
    public PlayerAbilityState AbilityOneState;
    public PlayerAbilityState AbilityTwoState;
    public PlayerAbilityState AbilityThreeState;

    public PlayerEmperorState EmperorState;
    public PlayerHierophantState HierophantState;
    public PlayerLoversState LoversState;
    public PlayerChariotState ChariotState;
    public PlayerStrengthState StrengthState;
    public PlayerHermitState HermitState;
    public PlayerJusticeState JusticeState;
    public PlayerDeathState DeathState;
    public PlayerTemperanceState TemperanceState;
    public PlayerDevilState DevilState;
    public PlayerStarState StarState;
    public PlayerMoonState MoonState;
    public PlayerAbilityTest TestState;

    #endregion

    #endregion
    
    [HideInInspector] public bool IsOnPlatform;
    [HideInInspector] public Rigidbody2D PlatformBody;

    #region Base Abilities

    public bool DoubleJumpEnabled;
    public bool WallJumpEnabled;
    public bool DashEnabled;

    #endregion

    #region Card Abilities

    public int AbilityOne;
    public int AbilityTwo;
    public int AbilityThree;

    #endregion


    #region Unity Callback Methods


    private void Awake()
    {
        stateMachine = new PlayerStateMachine();

        IdleState = new PlayerIdleState(this, stateMachine, "idle");
        RunState = new PlayerRunState(this, stateMachine, "run");
        
        JumpState = new PlayerJumpState(this, stateMachine, "airborne");
        LandState = new PlayerLandState(this, stateMachine, "land");
        WallJumpState = new PlayerWallJumpState(this, stateMachine, "airborne");
        
        DashState = new PlayerDashState(this, stateMachine, "dash");
        LedgeClimbState = new PlayerLedgeClimbState(this, stateMachine, "ledgeClimbState");
        WallSlideState = new PlayerWallSlideState(this, stateMachine, "wallSlide");
        
        AirborneState = new PlayerAirborneState(this, stateMachine, "airborne");
        
        AttackState = new PlayerAttackState(this, stateMachine, WeaponData, "attack");
        BlockState = new PlayerBlockState(this, stateMachine, "block");

        DeadState = new PlayerDeadState(this, stateMachine, "death");

        // Ability States
        AbilityOneState = new PlayerAbilityState(this, stateMachine, "abilityOne", AbilityInputs.First);
        AbilityTwoState = new PlayerAbilityState(this, stateMachine, "abilityTwo", AbilityInputs.Second);
        AbilityThreeState = new PlayerAbilityState(this, stateMachine, "abilityThree", AbilityInputs.Third);

        EmperorState = new PlayerEmperorState(this, stateMachine, "emperor", AbilityInputs.First);
        HierophantState = new PlayerHierophantState(this, stateMachine, "hierophant", AbilityInputs.First);
        LoversState = new PlayerLoversState(this, stateMachine, "lovers", AbilityInputs.First);
        ChariotState = new PlayerChariotState(this, stateMachine, chariotAbilityData, "chariot", AbilityInputs.First);
        StrengthState = new PlayerStrengthState(this, stateMachine, "strength", AbilityInputs.First);
        HermitState = new PlayerHermitState(this, stateMachine, "hermit", AbilityInputs.First);
        JusticeState = new PlayerJusticeState(this, stateMachine, "justice", AbilityInputs.First);
        DeathState = new PlayerDeathState(this, stateMachine, "death", AbilityInputs.First);
        TemperanceState = new PlayerTemperanceState(this, stateMachine, "temperance", AbilityInputs.First);
        DevilState = new PlayerDevilState(this, stateMachine, "devil", AbilityInputs.First);
        StarState = new PlayerStarState(this, stateMachine, starAbilityData, "star", AbilityInputs.Second);
        MoonState = new PlayerMoonState(this, stateMachine, "moon", AbilityInputs.First);
        //TestState = new PlayerAbilityTest(this, stateMachine, "test", AbilityInputs.Second);
    }

    public override void Start()
    {
        base.Start();

        Initialize();

        AbilityOneState = ChariotState;
        AbilityTwoState = StarState;

        ChariotState.ResetData();
        DashState.ResetDashes();

        EventHandler = GetComponent<AnimationEventHandler>();
        
        GhostTrail = GetComponent<GhostTrail>();
        AttackShake = GetComponent<CinemachineImpulseSource>();

        WallSlideParticles.gameObject.SetActive(false);

        stateMachine.InitalizeState(IdleState);
    }

    protected virtual void Initialize() { }

    private void Update()
    {
        CurrentState.StateUpdate();
    }

    private void FixedUpdate()
    {
        CurrentState.StateFixedUpdate();
    }

    void AnimationTrigger() => CurrentState.AnimationTrigger();
    void AnimationFinishedTrigger() => CurrentState.AnimationFinishedTrigger();

    #endregion

    public void GenerateAbilityState(int cardAbility, int cardSlot)
    {
        PlayerAbilityState playerAbility = cardAbility switch
        {
            0 => EmperorState,
            1 => HierophantState,
            2 => LoversState,
            3 => ChariotState,
            4 => StrengthState,
            5 => HermitState,
            6 => JusticeState,
            7 => DeathState,
            8 => TemperanceState,
            9 => DevilState,
            10 => StarState,
            11 => MoonState,
            _ => null,
        };
        switch (cardSlot)
        {
            case 1:
                AbilityOneState = playerAbility;
                playerAbility.AbilityInput = AbilityOneState.AbilityInput;
                break;
            case 2:
                AbilityTwoState = playerAbility;
                playerAbility.AbilityInput = AbilityTwoState.AbilityInput;
                break;
            case 3:
                AbilityThreeState = playerAbility;
                playerAbility.AbilityInput = AbilityThreeState.AbilityInput;
                break;
            default:
                break;
        }
    }

    public void SpawnObject(GameObject objectToSpawn, Vector2 locationToSpawn)
    {
        Instantiate(objectToSpawn, locationToSpawn, Quaternion.identity);
    }

    #region Input Callbacks

    public void JumpInputChecks()
    {
        if (InputManager.instance.JumpJustPressed)
        {
            if (
                AirborneState.IsWallSlideFalling
                && WallJumpState.WallJumpPostBufferTimer >= 0f
                )
            {
                return;
            }

            else if (
                WallSlideState.IsWallSliding
                || (CollisionSensors.IsWall && !CollisionSensors.IsGrounded)
                )
            {
                return;
            }

            JumpWasPressed();
        }

        if (InputManager.instance.JumpReleased)
        {
            JumpWasReleased();
        }
    }

    void JumpWasPressed()
    {
        JumpState.JumpBufferTimer = MoveData.JumpBufferTime;
        JumpState.IsJumpCut = false;
    }

    void JumpWasReleased()
    {
        if (JumpState.JumpBufferTimer > 0f)
        {
            JumpState.IsJumpCut = true;
        }

        if (AirborneState.IsJumping && Movement.CurrentVelocity.y > 0f)
        {
            if (AirborneState.IsPastApexThreshold)
            {
                AirborneState.IsPastApexThreshold = false;
                AirborneState.IsFastFalling = true;
                AirborneState.FastFallTime = MoveData.TimeForUpwardsCancel;

                Movement.SetVelocityY(0f);
            }
            else
            {
                AirborneState.IsFastFalling = true;
                AirborneState.FastFallReleaseSpeed = Movement.CurrentVelocity.y;
            }
        }
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


    #region Movement

    public void Move(float acceleration, float decceleration, Vector2 moveInput)
    {
        float moveSpeed = MoveData.MaxRunSpeed;

        if (Mathf.Abs(moveInput.x) > MoveData.MoveThreshold)
        {
            Movement.TurnCheck(moveInput);

            float targetVelocity = moveInput.x * moveSpeed;

            if (!CollisionSensors.IsOnSlope)
            {
                Movement.SetVelocityX(Mathf.Lerp(Movement.CurrentVelocity.x, targetVelocity, acceleration * Time.fixedDeltaTime));
            }
            else if (CollisionSensors.IsOnSlope && CollisionSensors.CanWalkOnSlope)
            {
                Movement.SetVelocity(
                    Mathf.Lerp(Movement.CurrentVelocity.x, -targetVelocity * CollisionSensors.SlopeNormalPerp.x, acceleration * Time.fixedDeltaTime),
                    Mathf.Lerp(Movement.CurrentVelocity.y, -targetVelocity * CollisionSensors.SlopeNormalPerp.y, acceleration * Time.fixedDeltaTime)
                    );
            }
        }

        else
        {
            if (!CollisionSensors.IsOnSlope)
            {
                Movement.SetVelocityX(Mathf.Lerp(Movement.CurrentVelocity.x, 0f, decceleration * Time.deltaTime));
            }
            else if (CollisionSensors.IsOnSlope && CollisionSensors.CanWalkOnSlope)
            {
                Movement.SetVelocity(
                    Mathf.Lerp(Movement.CurrentVelocity.x, 0f, decceleration * Time.fixedDeltaTime),
                    Mathf.Lerp(Movement.CurrentVelocity.y, 0f, decceleration * Time.fixedDeltaTime)
                    );
            }
            
        }
    }

    public void ApplyVelocity()
    {
        if (!DashState.IsDashing)
        {
            Movement.SetVelocityY(Mathf.Clamp(Movement.CurrentVelocity.y, -MoveData.MaxFallSpeed, 50f));
        }
        else
        {
            Movement.SetVelocityY(Mathf.Clamp(Movement.CurrentVelocity.y, -50f, 50f));
        }
    }

    #endregion


/*
    #region Jump Visualition Tool

    private void OnDrawGizmos()
    {
        if (MoveData.ShowRunJumpArc)
        {
            DrawJumpArc(MoveData.MaxRunSpeed, Color.red);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(AttackTransform.position, WeaponData.AttackRange);
    }


    private void DrawJumpArc(float moveSpeed, Color gizmoColor)
    {
        Vector2 startPosition = new Vector2(CollisionSensors.GroundCheck.position.x, CollisionSensors.GroundCheck.position.y);
        Vector2 previousPosition = startPosition;
        float speed = 0f;
        if (MoveData.DrawRight)
        {
            speed = moveSpeed;
        }
        else { speed = -moveSpeed; }
        Vector2 velocity = new Vector2(speed, MoveData.InitialJumpVelocity);

        Gizmos.color = gizmoColor;

        float timeStep = 2 * MoveData.TimeTillJumpApex / MoveData.ArcResolution;

        for (int i = 0; i < MoveData.VisualizationSteps; i++)
        {
            float simulationTime = i * timeStep;
            Vector2 displacement;
            Vector2 drawPoint;
            float downTime = MoveData.Gravity * MoveData.GravityOnReleaseMultiplier;

            //ascending
            if (simulationTime < MoveData.TimeTillJumpApex)
            {
                displacement = velocity * simulationTime + 0.5f * new Vector2(0, MoveData.Gravity) * simulationTime * simulationTime;
            }

            //apex hang time
            else if (simulationTime < MoveData.TimeTillJumpApex + MoveData.ApexHangTime)
            {
                float apexTime = simulationTime - MoveData.TimeTillJumpApex;
                displacement = velocity * MoveData.TimeTillJumpApex + 0.5f * new Vector2(0, MoveData.Gravity) * MoveData.TimeTillJumpApex * MoveData.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * apexTime;
            }

            //descending
            else
            {
                float descendTime = simulationTime - (MoveData.TimeTillJumpApex + MoveData.ApexHangTime);
                displacement = velocity * MoveData.TimeTillJumpApex + 0.5f * new Vector2(0, MoveData.Gravity) * MoveData.TimeTillJumpApex * MoveData.TimeTillJumpApex;
                displacement += new Vector2(speed, 0) * MoveData.ApexHangTime;

                downTime *= descendTime * descendTime;


                displacement += new Vector2(speed, 0) * descendTime + 0.5f * new Vector2(0, downTime);
            }

            drawPoint = startPosition + displacement;

            if (MoveData.StopOnCollision)
            {
                RaycastHit2D hit = Physics2D.Raycast(previousPosition, drawPoint - previousPosition, Vector2.Distance(previousPosition, drawPoint), CollisionSensors.GroundMask);
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
    */


    public void Heal(int amount)
    {
        Stats.Health.Increase(amount);
        Debug.Log("Healed player for " + amount);
    }

    #region Functionality

    public IEnumerator MeleeAttack()
    {
        AttackState.IsAttacking = true;

        while (AttackState.IsAttacking)
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(
                AttackState.AttackPos.position,
                WeaponData.AttackRange,
                Vector2.right,
                0f,
                WeaponData.DetectableLayers
            );

            if (hits != null && hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    IDamageable damageable = hits[i].collider.gameObject.GetComponent<IDamageable>();
                    if (
                        damageable != null
                        && !damageable.HasTakenDamage
                        && !damageable.IsInvincible
                    )
                    {
                        damageable.Damage(
                            new DamageData(
                                WeaponData.DamageAmount,
                                gameObject
                            )
                        );

                        AttackState.DetectedDamageables.Add(damageable);
                    }

                    IKnockbackable knockbackable = hits[i].collider.gameObject.GetComponent<IKnockbackable>();
                    if (
                        knockbackable != null
                        && !knockbackable.HasKnockbacked
                        && !knockbackable.IsNotKnockbackable
                    )
                    {
                        knockbackable.Knockback(
                            new KnockbackData(
                                WeaponData.KnockbackAngle,
                                WeaponData.KnockbackStrength,
                                Movement.FacingDirection,
                                gameObject
                            )
                        );

                        AttackState.DetectedIKnockbackables.Add(knockbackable);
                    }
                }

            }
            yield return null;
        }

        ResetLists();
    }

    public void ResetLists()
    {
        foreach (IDamageable damaged in AttackState.DetectedDamageables)
        {
            damaged.HasTakenDamage = false;
        }

        AttackState.DetectedDamageables.Clear();
        AttackState.DetectedIKnockbackables.Clear();
    }

    public override void Die()
    {
        ChangeState(DeadState);
    }

    #endregion
}
