using System.Collections;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    #region Blackboard Variables

    public Vector2 DashDirection;

    public bool IsDashing;

    bool _isAbilityDone;

    bool _isGrounded;
    bool _isSlope;
    
    float _lastPressedDashTime;
    int _numberOfDashesLeft;
    bool _isDashAttacking;
    bool _dashRefilling;
    Vector2 _lastDashDirection;

    #endregion

    public PlayerDashState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        _isAbilityDone = false;

        player.Sleep(MoveData.DashSleepTime);

        if (InputManager.instance.MoveInput != Vector2.zero)
            _lastDashDirection = InputManager.instance.MoveInput;
        
        else
            _lastDashDirection = Movement.IsFacingRight ? Vector2.right : Vector2.left;

        IsDashing = true;

        player.StartCoroutine(StartDash(_lastDashDirection));
    }

    public override void StateExit()
    {
        base.StateExit();
    }

    public override void DoChecks()
    {
        base.DoChecks();

        if (CollisionSensors)
        {
            _isGrounded = CollisionSensors.IsGrounded;
            _isSlope = CollisionSensors.IsOnSlope;
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
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();
    }

    #endregion


    #region Check Functions

    public bool CanDash()
    {
        if (
            !IsDashing
            && _numberOfDashesLeft < MoveData.NumberOfDashes
            && _isGrounded
            && !_dashRefilling
            )
            player.StartCoroutine(RefillDash(1));
        
        Debug.Log(_numberOfDashesLeft > 0);
        return _numberOfDashesLeft > 0;   
    }

    #endregion


    #region Functionality

    public void ResetDashes()
    => _numberOfDashesLeft = MoveData.NumberOfDashes;

    IEnumerator StartDash(Vector2 direction)
    {
        _lastPressedDashTime = 0;

        float startTime = Time.time;

        _numberOfDashesLeft--;
        _isDashAttacking = true;

        Movement.SetVelocityY(0f);

        while (Time.time - startTime <= MoveData.DashAttackTime)
        {
            Movement.SetVelocity(
                MoveData.DashSpeed * CollisionSensors.SlopeNormalPerp.x * -direction.normalized.x,
                MoveData.DashSpeed * CollisionSensors.SlopeNormalPerp.y * -direction.normalized.x
                );

            yield return null;
        }

        startTime = Time.time;

        _isDashAttacking = false;

        // Body.gravityScale = MoveData.GravityScale;

        if (!_isSlope)
        {
            Movement.SetVelocity(MoveData.DashEndSpeed, direction.normalized);
        }
        else if (_isSlope && CollisionSensors.CanWalkOnSlope)
        {
            Movement.SetVelocity(
                MoveData.DashEndSpeed * CollisionSensors.SlopeNormalPerp.x * -direction.normalized.x,
                MoveData.DashEndSpeed * CollisionSensors.SlopeNormalPerp.y * -direction.normalized.x
                );
        }

        Movement.SetVelocityY(Mathf.Clamp(Movement.CurrentVelocity.y, -MoveData.MaxFallSpeed, 50f));

        while (Time.time - startTime <= MoveData.DashEndTime)
        {
            yield return null;
        }

        player.StartCoroutine(RefillDash(1));

        _isAbilityDone = true;
    }

    IEnumerator RefillDash(int amount)
    {
        _dashRefilling = true;

        yield return new WaitForSeconds(MoveData.DashRefillTime);

        _dashRefilling = false;
        _numberOfDashesLeft = Mathf.Min(MoveData.NumberOfDashes, _numberOfDashesLeft + amount);
    }

    #endregion


    #region Effects

    void DashParticles()
    {
        float counter = 0f + Time.deltaTime;
        Debug.Log(counter);

        if (counter >= player.DashParticleTime)
        {
            player.DashParticles.Play();
            counter = 0f;
        }
    }

    #endregion
}
