using UnityEngine;

public class PlayerAbilityState : PlayerState
{
    // checks
    protected bool isAbilityDone;
    protected bool isGrounded;
    protected bool isWall;
    protected float manaCost;
    protected float cooldownTimer;

    // inputs
    public int AbilityInput;
    protected bool isAbilityHeld;
    protected float inputHoldTime;

    public PlayerAbilityState(
        Player player,
        PlayerStateMachine stateMachine,
        string animBoolName,
        AbilityInputs input
        ) : base(player, stateMachine, animBoolName)
    {
        AbilityInput = (int)input;
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        isAbilityDone = false;

        inputHoldTime = 0f;

        player.Stats.Mana.Decrease(manaCost);

        if (InputManager.instance.AbilityBeingHeld[AbilityInput])
        {
            isAbilityHeld = true;
            HeldBehaviour();
        }
    }

    public override void StateExit()
    {
        base.StateExit();

        isAbilityHeld = false;
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSensors)
        {
            isGrounded = CollisionSensors.IsGrounded;
            isWall = CollisionSensors.IsWall;
        }
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        //CurrentInput = InputManager.instance.AbilityOne;

        if (InputManager.instance.AbilityBeingHeld[AbilityInput])
        {
            
            HeldBehaviour();
        }

        if (InputManager.instance.AbilityReleased[AbilityInput])
        {
            isAbilityHeld = false;
            ReleaseBehaviour();
        }

        if (isAbilityDone)
        {
            if (isGrounded && Movement.CurrentVelocity.y < 0.01f)
            {
                ChangeState(player.IdleState);
            }
            else
            {
                ChangeState(player.AirborneState);
            }
        }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    public virtual bool CanCheck() 
    => cooldownTimer <= 0;

    public virtual bool CanAirCheck() => false;

    public virtual void ResetData() { }
    
    public virtual void ResetValues() { }

    public virtual void HeldBehaviour()
    {
        Animator.SetBool("abilityHold", true);
        inputHoldTime += Time.deltaTime;
        isAbilityDone = false;
    }

    public virtual void ReleaseBehaviour()
    {
        Animator.SetBool("abilityHold", false);
    }

    public void UpdateAbilityTimer()
    {
        if (cooldownTimer >= -0.1f)
            cooldownTimer -= Time.deltaTime;
    }

    #endregion
}
