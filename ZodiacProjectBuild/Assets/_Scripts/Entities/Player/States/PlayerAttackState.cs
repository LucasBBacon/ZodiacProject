using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    public SOWeaponData Data;

    AnimationEventHandler EventHandler => player.EventHandler;

    bool _isGrounded;

    Vector2 _knockbackDirection;

    public bool IsAttacking;
    float _attackTimer = 0f;
    public Transform AttackPos;
    AttackDirections _attackDirection;

    bool _canInterrupt;
    bool _checkFlip;
    bool _isAbilityDone;
    bool _isAttackActive;

    public float AttackStartTime { get; set; }

    bool _input;
    bool _currentInput;
    bool _minHoldPassed;


    public List<IDamageable> DetectedDamageables = new List<IDamageable>();
    public List<IKnockbackable> DetectedIKnockbackables = new List<IKnockbackable>();


    public PlayerAttackState(
        Player player,
        PlayerStateMachine stateMachine,
        SOWeaponData weaponData,
        string animBoolName
        ) : base(player, stateMachine, animBoolName)
    {
        Data = weaponData;
    }

    #region Callback Functions

    private void OnDisable()
    {
        EventHandler.OnEnableInterrupt -= HandleEnableInterrupt;
        EventHandler.OnFinish -= HandleFinish;
        EventHandler.OnFlipSetActive -= HandleTurnCheckSetActive;
        EventHandler.OnAttackAction -= HandleAttackAction;

        EventHandler.OnUseInput -= HandleUseInput;

        EventHandler.OnMinHoldPassed -= HandleMinHoldPassed;

        EventHandler.OnStartMovement -= HandleStartMovement;
        EventHandler.OnStopMovement -= HandleStopMovement;
    }

    public override void StateEnter()
    {
        base.StateEnter();

        EventHandler.OnEnableInterrupt += HandleEnableInterrupt;
        EventHandler.OnFinish += HandleFinish;
        EventHandler.OnFlipSetActive += HandleTurnCheckSetActive;
        EventHandler.OnAttackAction += HandleAttackAction;

        EventHandler.OnUseInput += HandleUseInput;

        EventHandler.OnMinHoldPassed += HandleMinHoldPassed;

        EventHandler.OnStartMovement += HandleStartMovement;
        EventHandler.OnStopMovement += HandleStopMovement;

        _isAbilityDone = false;

        _checkFlip = true;
        _canInterrupt = false;

        CheckAttackDirection();

        AttackStartTime = Time.time;
        _isAttackActive = true;
        _minHoldPassed = false;

        Movement.SetVelocityZero();
    }

    public override void StateExit()
    {
        base.StateExit();

        _isAttackActive = false;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSensors)
        {
            _isGrounded = CollisionSensors.IsGrounded;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (_isAbilityDone)
        {
            if (_isGrounded && Movement.CurrentVelocity.y < 0.01f)
            {
                ChangeState(player.IdleState);
            }
            else
            {
                ChangeState(player.AirborneState);
            }
        }

        if (!_canInterrupt)
            return;

        if (InputManager.instance.MoveInput.x != 0 || InputManager.instance.AttackInput)
            _isAbilityDone = true;
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        player.Move(MoveData.GroundAcceleration, MoveData.GroundDeceleration, Vector2.zero);

        if (_checkFlip)
            Movement.TurnCheck(InputManager.instance.MoveInput);
    }

    #endregion


    #region Check Methods

    void CheckAttackDirection()
    {
        if (_isGrounded)
        {
            Animator.SetBool("groundedAttack", true);
            if (InputManager.instance.MoveInput.y > 0)
            {
                _attackDirection = AttackDirections.Up;
                _knockbackDirection = Vector2.zero;
            }

            if (InputManager.instance.MoveInput.y <= 0)
            {
                _attackDirection = AttackDirections.Side;
            }
        }

        else
        {
            Animator.SetBool("groundedAttack", false);
            if (InputManager.instance.MoveInput.y > 0)
            {
                _attackDirection = AttackDirections.Up;
                _knockbackDirection = Vector2.zero;
            }

            if (InputManager.instance.MoveInput.y == 0)
            {
                _attackDirection= AttackDirections.Side;
            }

            if (InputManager.instance.MoveInput.y < 0)
            {
                _attackDirection = AttackDirections.Down;
            }
        }

        switch (_attackDirection)
        {
            case AttackDirections.Up:
                AttackPos = player.AttackUpTransform;
                Animator.SetInteger("attackDirection", 0);
                break;
            case AttackDirections.Side:
                AttackPos = player.AttackTransform;
                Animator.SetInteger("attackDirection", 1);
                if (Movement.IsFacingRight)
                    _knockbackDirection = Vector2.left;
                else
                    _knockbackDirection = Vector2.right;
                break;
            case AttackDirections.Down:
                Animator.SetInteger("attackDirection", 2);
                AttackPos = player.AttackDownTransform;
                _knockbackDirection = Vector2.up;
                break;
            default:
                AttackPos = player.AttackTransform;
                Animator.SetInteger("attackDirection", 1);
                if (Movement.IsFacingRight)
                    _knockbackDirection = Vector2.left;
                else
                    _knockbackDirection = Vector2.right;
                break;
        }
    }

    public bool CanAttack()
    {
        if (_attackTimer >= Data.TimeBetweenAttacks)
        {
            _attackTimer = 0f;

            return true;
        }

        return false;
    }

    #endregion


    #region Animation Handlers

    void HandleAttackAction()
    {
        player.StartCoroutine(player.MeleeAttack());
    }

    public void AttackTimers()
    => _attackTimer += Time.deltaTime;

    void HandleTurnCheckSetActive(bool value)
    => _checkFlip = value;

    void HandleEnableInterrupt()
    => _canInterrupt = true;

    void HandleStartMovement()
    {
        
    }

    void HandleStopMovement()
    {
        
    }

    void HandleUseInput() { }

    void HandleFinish()
    {
        AnimationFinishedTrigger();
        IsAttacking = false;
        _isAbilityDone = true;
    }

    void HandleCurrentInputChange(bool newInput)
    {
        _input = newInput;

        SetAnimatorParameter();
    }

    void SetAnimatorParameter()
    {
        if (_input)
        {
            Animator.SetBool("hold", _input);
        }

        if (_minHoldPassed)
        {
            Animator.SetBool("hold", false);
        }
    }

    void HandleMinHoldPassed()
    {
        _minHoldPassed = true;

        SetAnimatorParameter();
    }

    #endregion


    
}

public enum AttackDirections
{
    Up,
    Down,
    Side
}