using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{   
    [Header("Enemy Data")]
    public EntityData EntityData;
    public AnimationToStateMachine AnimationToStateMachine;
    [SerializeField] Transform playerCheck;


    #region State Machine References

    public EntityStateMachine stateMachine;
    public State CurrentState => stateMachine.CurrentState;
    public State PreviousState => stateMachine.PreviousState;

    protected void ChangeState(State newState, bool forceReset = false)
    => stateMachine.ChangeState(newState, forceReset);

    #endregion


    #region Entity Variables

    float currentHealth;
    float currentStunResitance;
    float lastDamageTime;
    public bool CanSetVelocity { get; set; }

    protected bool IsStunned;
    protected bool IsDead;

    #endregion


    #region Callback Functions

    public virtual void Awake()
    {
        stateMachine = new EntityStateMachine();
        AnimationToStateMachine = GetComponentInChildren<AnimationToStateMachine>();
    }

    public override void Start()
    {
        base.Start();
    }

    public virtual void Update()
    {
        
        CurrentState.StateUpdate();

        if (Time.time >= lastDamageTime + EntityData.StunRecoveryTime)
            ResetStunResistance();
    }

    public virtual void FixedUpdate()
    {
        CollisionSensors.CollisionChecks();
        CurrentState.StateFixedUpdate();
    }
    
    #endregion


    #region Check Functions

    public virtual bool CheckPlayerInMinAgroRange()
    => Physics2D.Raycast
        (
            playerCheck.position,
            transform.right,
            EntityData.MinAgroDistance,
            EntityData.PlayerMask
        );

    public virtual bool CheckPlayerInMaxAgroRange()
    => Physics2D.Raycast
        (
            playerCheck.position,
            transform.right,
            EntityData.MaxAgroDistance,
            EntityData.PlayerMask
        );

    public virtual bool CheckPlayerInCloseRangeAction()
    => Physics2D.Raycast
        (
            playerCheck.position,
            transform.right,
            EntityData.CloseRangeActionDistance,
            EntityData.PlayerMask
        );

    #endregion

    public virtual void ResetStunResistance()
    {
        IsStunned = false;
        currentStunResitance = EntityData.StunResistance;
    }

    public virtual void OnDrawGizmos() {
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * EntityData.CloseRangeActionDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * EntityData.MinAgroDistance), 0.2f);
        Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(Vector2.right * EntityData.MaxAgroDistance), 0.2f);
	}
}
