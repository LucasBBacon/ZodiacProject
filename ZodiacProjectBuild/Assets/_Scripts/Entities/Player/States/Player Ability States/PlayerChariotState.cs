using System.Collections;
using Cinemachine;
using UnityEngine;

public class PlayerChariotState : PlayerAbilityState
{
    #region Blackboard Variables

    public bool IsChariot { get; private set; }
    public bool IsChariotAttacking { get; private set; }
    public bool IsChariotFastFalling { get; private set; }
    public float ChariotOnGroundTimer { get; private set; }
    public float LastPressedChariotTime { get; private set; }

    float _cameraLensSize;

    int _chariotsLeft;
    bool _chariotRefilling;
    float _chariotTime;
    bool[] _shouldBurst = new bool[3];

    Vector2 _lastChariotDirection;

    SOChariotAbilityData _abilityData;

    #endregion

    public PlayerChariotState(
        Player player,
        PlayerStateMachine stateMachine,
        SOChariotAbilityData abilityData,
        string animBoolName,
        AbilityInputs input
        ) : base(player, stateMachine, animBoolName, input)
    {
        this._abilityData = abilityData;
    }

    #region Callback Functions

    public override void StateEnter()
    {
        manaCost = _abilityData.ManaCost;
        cooldownTimer = _abilityData.AbilityCooldown;

        base.StateEnter();

        _chariotTime = 0f;

        for (int i = 0; i < _shouldBurst.Length; i++)
        {
            _shouldBurst[i] = true;
        }

        _cameraLensSize = CameraManager.instance.CurrentCamera.m_Lens.OrthographicSize;

        if (InputManager.instance.MoveInput != Vector2.zero)
            _lastChariotDirection = InputManager.instance.MoveInput;
        
        else
            _lastChariotDirection = Movement.IsFacingRight ? Vector2.right : Vector2.left;
        

        if (!isAbilityHeld)
        {
            player.Sleep(_abilityData.ChariotSleepTime);
            
            _chariotTime = _abilityData.ChariotMinTime;
            IsChariot = true;
            player.StartCoroutine(StartChariot(_lastChariotDirection));
        }
    }

    public override void StateExit()
    {
        base.StateExit();

        Animator.SetBool("chariotEnd", false);

        IsChariot = false;
    }

    public override void StateUpdate()
    {
        base.StateUpdate();
            

        if (IsChariot && isWall)
        {
            CameraShakeManager.instance.CameraShake(player.CollisionShake);
            if (isGrounded)
            {
                ChangeState(player.AirborneState);
            }
            else
            {
                ChangeState(player.IdleState);
            }
        }

        else if (
            !IsChariot
            && !isGrounded 
            && !IsExitingState
            )
        {
            ChangeState(player.AirborneState);
        }

        else if (
            !IsChariot 
            && isGrounded 
            && !IsExitingState 
            && !isAbilityHeld)
        {
            ChangeState(player.IdleState);
        }

        else if (player.JumpState.LastPressedJumpTime > 0)
        {
            if (player.JumpState.CanJump())
            {
                ChangeState(player.JumpState);
            }
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        if (!IsChariotAttacking)
        {
            player.Move(MoveData.GroundAcceleration, MoveData.GroundDeceleration, Vector2.zero);
        }
    }

    public override void HeldBehaviour()
    {
        base.HeldBehaviour();

        // clamp hold time
        if (inputHoldTime >= _abilityData.InputHoldMaxTime + 0.1f)
            inputHoldTime = _abilityData.InputHoldMaxTime + 0.1f;

        // effects
        else
        {
            CameraManager.instance.CurrentCamera.m_Lens.OrthographicSize -= Time.deltaTime / 10;
        }

        CameraManager.instance.CurrentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = Utilities.MappingUtil.Map(
            inputHoldTime,
            0f, _abilityData.InputHoldMaxTime + 0.1f,
            0f, 1f,
            true
        );

        for (int i = 0; i < _abilityData.ChariotBurstTimers.Length; i++)
        {
            if (inputHoldTime >= _abilityData.ChariotBurstTimers[i] && _shouldBurst[i])
            {
                RumbleManager.Instance.RumblePulse(
                    _abilityData.RumbleLowFreq * (i + 1),
                    _abilityData.RumbleHighFreq * (i + 1),
                    _abilityData.RumbleTime
                    );
                _shouldBurst[i] = false;
            }
        }
    }

    public override void ReleaseBehaviour()
    {
        base.ReleaseBehaviour();

        CameraManager.instance.CurrentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0f;
        CameraManager.instance.CurrentCamera.m_Lens.OrthographicSize = _cameraLensSize;

        float extraTime = Utilities.MappingUtil.Map(
            inputHoldTime,
            0f, _abilityData.InputHoldMaxTime,
            0f, _abilityData.ChariotMaxExtraTime,
            true
            );

        //Debug.Log(extraTime);

        _chariotTime = _abilityData.ChariotMinTime + extraTime;

        IsChariot = true;
        player.StartCoroutine(StartChariot(_lastChariotDirection));
    }

    #endregion


    #region Checks

    public override bool CanCheck()
    => base.CanCheck()
        && player.Stats.Mana.CurrentValue >= manaCost
        && !_chariotRefilling
        && CollisionSensors.IsGrounded
        && !IsChariot;

    #endregion


    #region Timers

    public void ChariotTimers()
    {
        LastPressedChariotTime -= Time.deltaTime;
    }

    #endregion


    #region Functionality

    public override void ResetData()
    => _chariotsLeft = _abilityData.NumberOfChariots;

    public override void ResetValues()
    {
        IsChariotFastFalling = false;
        ChariotOnGroundTimer = -0.01f;
    }

    
    IEnumerator RefillChariot(int amount)
    {
        _chariotRefilling = true;

        yield return new WaitForSeconds(MoveData.DashRefillTime);

        _chariotRefilling = false;
        _chariotsLeft = Mathf.Min(_abilityData.NumberOfChariots, _chariotsLeft + amount);
    }

    IEnumerator StartChariot(Vector2 dir)
    {
        LastPressedChariotTime = 0;

        float startTime = Time.time;

        _chariotsLeft--;
        IsChariotAttacking = true;

        Movement.SetVelocityY(0f);

        while (Time.time - startTime <= _chariotTime)
        {
            Movement.SetVelocityX(dir.normalized.x * _abilityData.ChariotSpeed);

            yield return null;
        }

        startTime = Time.time;

        IsChariotAttacking = false;

        Movement.SetVelocityX(dir.normalized.x * _abilityData.ChariotEndSpeed);
        Movement.SetVelocityY(Mathf.Clamp(Movement.CurrentVelocity.y, -MoveData.MaxFallSpeed, 50f));
        Animator.SetBool("chariotEnd", true);

        while (Time.time - startTime <= _abilityData.ChariotEndTime)
        {
            yield return null;
        }

        player.StartCoroutine(RefillChariot(1));

        isAbilityDone = true;
    }

    #endregion
}