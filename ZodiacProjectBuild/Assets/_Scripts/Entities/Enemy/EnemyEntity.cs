using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : Entity
{   
    [Header("Enemy Data")]
    public SOEntityData EntityData;
    public AnimationToStateMachine AnimationToStateMachine;
    [SerializeField] Transform playerCheck;
    FieldOfView fieldOfView;


    #region State Machine References

    public EntityStateMachine stateMachine;
    public State CurrentState => stateMachine.CurrentState;
    public State PreviousState => stateMachine.PreviousState;

    protected void ChangeState(State newState, bool forceReset = false)
    => stateMachine.ChangeState(newState, forceReset);

    #endregion


    #region Entity Variables

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
        fieldOfView = GetComponent<FieldOfView>();
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
        CurrentState.StateFixedUpdate();
    }
    
    #endregion


    #region Check Functions

    // public virtual bool CheckPlayerInMinAgroRange()
    // => Physics2D.Raycast
    //     (
    //         playerCheck.position,
    //         transform.right,
    //         EntityData.MinAgroDistance,
    //         EntityData.PlayerMask
    //     );

    // public virtual bool CheckPlayerInMaxAgroRange()
    // => Physics2D.Raycast
    //     (
    //         playerCheck.position,
    //         transform.right,
    //         EntityData.MaxAgroDistance,
    //         EntityData.PlayerMask
    //     );

    // public virtual bool CheckPlayerInCloseRangeAction()
    // => Physics2D.Raycast
    //     (
    //         playerCheck.position,
    //         transform.right,
    //         EntityData.CloseRangeActionDistance,
    //         EntityData.PlayerMask
    //     );

    public virtual bool CheckPlayerInMinAgroRange()
    => fieldOfView.FindVisibleTargets(EntityData.MinAgroDistance);

    public virtual bool CheckPlayerInMaxAgroRange()
    => fieldOfView.FindVisibleTargets(EntityData.MaxAgroDistance);

    public virtual bool CheckPlayerInCloseRangeAction()
    => fieldOfView.FindVisibleTargets(EntityData.CloseRangeActionDistance);

    #endregion

    public virtual void ResetStunResistance()
    {
        IsStunned = false;
        currentStunResitance = EntityData.StunResistance;
    }

    public virtual void OnDrawGizmos() {
        //Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(EntityData.CloseRangeActionDistance * Movement.FacingDirection * Vector2.right), 0.2f);
        //Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(EntityData.MinAgroDistance * Movement.FacingDirection * Vector2.right), 0.2f);
       // Gizmos.DrawWireSphere(playerCheck.position + (Vector3)(EntityData.MaxAgroDistance * Movement.FacingDirection * Vector2.right), 0.2f);
	}
}
