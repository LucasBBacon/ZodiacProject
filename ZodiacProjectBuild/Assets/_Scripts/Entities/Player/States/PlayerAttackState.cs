using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    readonly WeaponData weaponData;

    public int AttackCounter 
    {
        get => attackCounter;
        private set => attackCounter = value >= _player.WeaponData.NumberOfAttacks ? 0 : value;
    }
    public float AttackTimer;

    bool isAbilityDone;
    bool meleeAttack;
    
    bool isDamageActive;
    
    // attack animations
    int attackCounter;
    Utilities.TimeNotifier AttackCounterResetTimeNotifier => _player.AttackCounterResetTimeNotifier;

    // damage lists
    RaycastHit2D[] hits;
    readonly List<IDamageable> iDamaged = new List<IDamageable>();
    readonly List<IKnockbackable> iKnockbacked = new List<IKnockbackable>();

    // attack direction
    bool collided;
    readonly bool downwardStrike;
    Vector2 direction;
    public Vector2 attackPos;

    // player recoil
    float upwardsForce = 5000;
    float defaultForce = 300;

    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, WeaponData weaponData) : base(player, stateMachine)
    {
        this.weaponData = weaponData;
    }

    #region Callback Functions

    private void OnEnable()
    {
        AttackCounterResetTimeNotifier.OnNotify += ResetAttackCounter;    
    }

    private void OnDisable()
    {
        AttackCounterResetTimeNotifier.OnNotify -= ResetAttackCounter;    
    }

    public override void StateEnter()
    {
        base.StateEnter();

        _player.StartCoroutine(AttackDamage());

        Animator.SetInteger("attackCounter", AttackCounter);

        isAbilityDone = false;
        CheckMeleeInput();
    }

    public override void StateExit()
    {
        base.StateExit();
        AttackCounterResetTimeNotifier.Init(_player.WeaponData.AttackCounterResetCooldown);
        
        if (CollisionSensors.IsGrounded)
        {
            _player.AirborneState.ResetJumpValues();
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (IsAnimationFinished)
        {
            if (CollisionSensors.IsGrounded && Movement.VerticalVelocity < 0.01f)
            {
                ChangeState(_player.IdleState);
            }
            else
            {
                ChangeState(_player.AirborneState);
            }
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        HandleMovement();

        _player.AirborneState.JumpPhysics();

        if (CollisionSensors.IsGrounded)
        {
            Movement?.SetHorizontalVelocity(0f);
        }
        else
        {
            Movement.Move(InputManager.MoveInput, MoveStats.MaxRunSpeed);
        }
        
    }

    #endregion


    #region Triggers

    public override void AnimationTrigger()
    {
        base.AnimationTrigger();

        //_player.SwipeEffect.SetActive(true);
        AttackCounterResetTimeNotifier.Disable();
        isDamageActive = true;
    }

    public override void AnimationFinishedTrigger()
    {
        base.AnimationFinishedTrigger();

        //_player.SwipeEffect.SetActive(false);
        isDamageActive = false;
        AttackCounter++;
    }

    #endregion


    #region Checks

    public bool CanAttack()
    {
        if (AttackTimer >= weaponData.TimeBetweenAttacks)
        {
            AttackTimer = 0f;

            return true;
        }
        return false;
    }

    void CheckMeleeInput()
    {
        if (InputManager.AttackInput)
        {
            meleeAttack = true;
        }
        else
        {
            meleeAttack = false;
        }

        if (CollisionSensors.IsGrounded)
        {
            if (meleeAttack && InputManager.MoveInput.y > 0)
            {
                Animator.SetTrigger("isAttackingUp");
                _player.SwipeAnimator.SetTrigger("UpwardMeleeSwipe");

                attackPos = _player.AttackUpTransform.position;

                direction = Vector2.zero;
            }

            if (meleeAttack && InputManager.MoveInput.y <= 0)
            {
                Animator.SetTrigger("isAttacking");
                _player.SwipeAnimator.SetTrigger("MeleeSwipe");
                
                attackPos = _player.AttackTransform.position;

                if (Movement.IsFacingRight)
                {
                    direction = Vector2.left;
                }
                else
                {
                    direction = Vector2.right;
                }
            }
        }
        else
        {
            if (meleeAttack && InputManager.MoveInput.y > 0)
            {
                Animator.SetTrigger("isAirAttackingUp");
                _player.SwipeAnimator.SetTrigger("UpwardMeleeSwipe");

                attackPos = _player.AttackUpTransform.position;

                direction = Vector2.down;
            }   
            if (meleeAttack && InputManager.MoveInput.y == 0)
            {
                Animator.SetTrigger("isAirAttacking");
                _player.SwipeAnimator.SetTrigger("MeleeSwipe");

                attackPos = _player.AttackTransform.position;

                if (Movement.IsFacingRight)
                {
                    direction = Vector2.left;
                }
                else
                {
                    direction = Vector2.right;
                }
            }
            if (meleeAttack && InputManager.MoveInput.y < 0)
            {
                Animator.SetTrigger("isAirAttackingDown");
                _player.SwipeAnimator.SetTrigger("DownwardMeleeSwipe");

                attackPos = _player.AttackDownTransform.position;

                direction = Vector2.up;
            }
        }
    }

    #endregion


    #region Functionality

    public void AttackTimers()
    => AttackTimer += Time.deltaTime;

    public IEnumerator AttackDamage()
    {
        isDamageActive = true;

        while (isDamageActive)
        {
            hits = Physics2D.CircleCastAll
                (
                    attackPos,
                    weaponData.AttackRange,
                    Vector2.right,
                    0f,
                    weaponData.AttackLayer
                );
            
            if (hits.Length > 0)
            {
                // Debug.Log("hit objects");
                for (int i = 0; i < hits.Length; i++)
                {
                    IDamageable iDamageable = hits[i].collider.gameObject.GetComponent<IDamageable>();
                    if (
                        iDamageable != null &&
                        !iDamageable.HasTakenDamage &&
                        !iDamageable.IsInvincible
                        )
                    {
                        iDamageable.Damage
                            (
                                new DamageData
                                    (
                                        weaponData.DamageAmount,
                                        _player.gameObject
                                    )
                            );
                        iDamaged.Add(iDamageable);

                        _player.ParticleManager.StartEffect(_player.HitEffect, hits[i].point, Quaternion.identity, 0.6f);
                        _player.Sleep(0.1f);
                        CameraShakeManager.instance.CameraShake(_player.AttackShake);
                        
                        RumbleManager.Instance.RumblePulse(0.15f, 0.43f, 0.25f);
                        
                        collided = true;
                    }

                    IKnockbackable iKnockbackable = hits[i].collider.gameObject.GetComponent<IKnockbackable>();
                    if (
                        iKnockbackable != null &&
                        !iKnockbackable.HasKnockbacked &&
                        !iKnockbackable.IsNotKnockbackable
                        )
                    {
                        iKnockbackable.Knockback
                            (
                                new KnockbackData
                                    (
                                        weaponData.KnockbackAngle,
                                        weaponData.KnockbackStrength,
                                        Movement.FacingDirection,
                                        _player.gameObject
                                    )
                            );
                        iKnockbacked.Add(iKnockbackable);
                    }
                }
            }

            yield return null;
        }

        ResetLists();
    }

    private void ResetLists()
    {
        foreach (IDamageable damaged in iDamaged)
        {
            damaged.HasTakenDamage = false;
        }

        collided = false;

        iDamaged.Clear();
        iKnockbacked.Clear();
    }

    private void ResetAttackCounter()
    {
        AttackCounter = 0;
    }

    private void HandleMovement()
    {
        if (collided)
        {
            if (downwardStrike)
            {
                Body.AddForce(direction * upwardsForce, ForceMode2D.Impulse);
            }
            else
            {
                Body.AddForce(direction * defaultForce);
            }
        }
    }

    #endregion
}