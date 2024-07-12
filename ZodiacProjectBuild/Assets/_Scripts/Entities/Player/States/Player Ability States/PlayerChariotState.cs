using UnityEngine;

public class PlayerChariotState : AbilityState
{
    #region Blackboard Variables

    public bool IsChariot { get; private set; }
    public float ChariotTimer { get; private set; }
    public bool IsAirChariot { get; private set; }
    
    public int NumberOfChariotsUsed { get; private set; }
    
    public Vector2 ChariotDirection { get; private set; }
    public float ChariotDirectionMult { get; private set; }

    public bool IsChariotFalling { get; private set; }
    public bool IsChariotFastFalling { get; private set; }
    public float ChariotFastFallReleaseSpeed { get; private set; }
    public float ChariotFastFallTime { get; private set; }
    
    public float ChariotOnGroundTimer { get; private set; }
    public float WallJumpPostBufferTimer { get; private set; }

    ChariotAbilityData AbilityData;

    #endregion

    public PlayerChariotState(Player player, PlayerStateMachine stateMachine, ChariotAbilityData abilityData) : base(player, stateMachine)
    {
        this.AbilityData = abilityData;
    }

    #region Callback Functions

    public override void StateEnter()
    {
        base.StateEnter();

        InitiateChariot();

        Animator.SetBool("isChariot", true);
    }

    public override void StateExit()
    {
        base.StateExit();

        IsChariot = false;
        Animator.SetBool("isChariot", false);
    }

    public override void StateUpdate()
    {
        base.StateUpdate();

        if (_player.WallSlideState.ShouldWallSlide())
        {
            ChangeState(_player.AirborneState);
        }

        else if (!IsChariot && !CollisionSensors.IsGrounded && !IsExitingState)
        {
            ChangeState(_player.AirborneState);
        }

        else if (!IsChariot && CollisionSensors.IsGrounded && !IsExitingState)
        {
            ChangeState(_player.IdleState);
        }

        else if (InputManager.JumpJustPressed)
        {
            if (_player.JumpState.CanJump())
            {
                _player.SpawnParticles(_player.JumpParticles);
                ChangeState(_player.JumpState);
            }
        }

        // if (!IsExitingState && Time.time >= startTime + AbilityData.ChariotTime)
        // {
        //     IsAbilityDone = true;
        // }
    }

    public override void StateFixedUpdate()
    {
        base.StateFixedUpdate();

        ChariotPhysics();
    }

    #endregion


    #region Checks

    public override bool CanCheck()
    {
        Debug.Log("Check: " + CollisionSensors.IsGrounded + ", " + (ChariotOnGroundTimer < 0) + ", " + !IsChariot);
        return CollisionSensors.IsGrounded && ChariotOnGroundTimer < 0 && !IsChariot;
    } 
        

    public override bool CanAirCheck()
    {
        Debug.Log("Air Check: " + !CollisionSensors.IsGrounded + ", " + !IsChariot + ", " + (NumberOfChariotsUsed < AbilityData.NumberOfChariots));
        
        if (
            !CollisionSensors.IsGrounded &&
            !IsChariot &&
            NumberOfChariotsUsed < AbilityData.NumberOfChariots
            )
        {
            IsAirChariot = true;

            if (WallJumpPostBufferTimer > 0f)
            {
                _player.JumpState.NumberOfJumpsUsed--;

                if (_player.JumpState.NumberOfJumpsUsed < 0)
                {
                    _player.JumpState.NumberOfJumpsUsed = 0;
                }
            }

            return true;
        }

        return false;
    }

    #endregion


    #region Functionality

    public override void ResetData()
    => NumberOfChariotsUsed = 0;

    public override void ResetValues()
    {
        IsChariotFastFalling = false;
        ChariotOnGroundTimer = -0.01f;
    }

    public void ChariotTimers()
    {
        if (CollisionSensors.IsGrounded)
        {
            ChariotOnGroundTimer -= Time.deltaTime;
        }
    }


    public void InitiateChariot()
    {
        ChariotDirection = InputManager.MoveInput;

        Vector2 closestDirection = Vector2.zero;
        float minDistance = Vector2.Distance(ChariotDirection, AbilityData.ChariotDirections[0]);

        for (int i = 0; i < AbilityData.ChariotDirections.Length; i++)
        {
            if (ChariotDirection == AbilityData.ChariotDirections[i])
            {
                closestDirection = ChariotDirection;
                break;
            }

            float distance = Vector2.Distance(ChariotDirection, AbilityData.ChariotDirections[i]);

            bool isDiagonal = 
                Mathf.Abs(AbilityData.ChariotDirections[i].x) == 1 &&
                Mathf.Abs(AbilityData.ChariotDirections[i].y) == 1;
            if (isDiagonal)
            {
                distance -= AbilityData.ChariotDiagonallyBias;
            }

            else if (distance < minDistance)
            {
                minDistance = distance;
                closestDirection = AbilityData.ChariotDirections[i];
            }
        }

        // handle direction with no input
        if (closestDirection == Vector2.zero)
        {
            if (Movement.IsFacingRight)
            {
                closestDirection = Vector2.right;
            }
            else
            {
                closestDirection = Vector2.left;
            }
        }

        ChariotDirectionMult = 1;
        ChariotDirection = new Vector2
            (
                closestDirection.x * ChariotDirectionMult,
                closestDirection.y * ChariotDirectionMult
            );
        
        NumberOfChariotsUsed++;
        IsChariot = true;
        ChariotTimer = 0f;
        ChariotOnGroundTimer = AbilityData.TimeBetweenChariotGround;

        Animator.SetBool("isChariot", true);
        _player.GhostTrail.LeaveGhostTrail(AbilityData.ChariotTime * 1.75f);

        _player.AirborneState.ResetJumpValues();
        _player.AirborneState.ResetWallJumpValues();
        _player.WallSlideState.StopWallSliding();
    }

    public void ChariotPhysics()
    {
        if (IsChariot)
        {
            //stop the dash after the timer
            ChariotTimer += Time.fixedDeltaTime;
            if (ChariotTimer >= AbilityData.ChariotTime)
            {
                if (CollisionSensors.IsGrounded)
                {
                    ResetData();
                }
                else
                {
                    Animator.SetBool(Player.IS_AIR_CHARIOT_FALLING, true);
                }


                IsAirChariot = false;
                IsChariot = false;
                // Debug.Log("shut down");

                Animator.SetBool(Player.IS_CHARIOT, false);

                //start the time for upwards cancel
                if (!_player.AirborneState.IsJumping && !_player.AirborneState.IsWallJumping)
                {
                    ChariotFastFallTime = 0f;
                    ChariotFastFallReleaseSpeed = Movement.VerticalVelocity;

                    if (!CollisionSensors.IsGrounded)
                        IsChariotFastFalling = true;
                }

                return;
            }

            Movement.HorizontalVelocity = AbilityData.ChariotSpeed * ChariotDirection.x;

            if (ChariotDirection.y != 0f || IsAirChariot)
                Movement?.SetVerticalVelocity(AbilityData.ChariotSpeed * ChariotDirection.y);
        }

        //HANDLE DASH CUT TIME
        else if (IsChariotFastFalling)
        {
            //new
            if (Movement?.VerticalVelocity > 0f)
            {
                if (ChariotFastFallTime < AbilityData.ChariotTimeForUpwardsCancel)
                {
                    Movement?.SetVerticalVelocity(Mathf.Lerp(ChariotFastFallReleaseSpeed, 0f, (ChariotFastFallTime / AbilityData.ChariotTimeForUpwardsCancel)));
                }
                else if (ChariotFastFallTime >= AbilityData.ChariotTimeForUpwardsCancel)
                {
                    Movement?.IncrementVerticalVelocity(MoveStats.Gravity * AbilityData.ChariotGravityOnReleaseMultiplier * Time.fixedDeltaTime);
                }

                ChariotFastFallTime += Time.fixedDeltaTime;
            }
            else
            {
                Movement?.IncrementVerticalVelocity(MoveStats.Gravity * AbilityData.ChariotGravityOnReleaseMultiplier * Time.fixedDeltaTime);
            }
        }
    }

    #endregion
}
