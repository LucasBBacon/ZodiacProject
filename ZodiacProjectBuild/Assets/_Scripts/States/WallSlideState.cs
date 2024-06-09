using UnityEngine;

public class WallSlideState : State
{
    public Player player;

    [Header("Animation Clip")]
    public  AnimationClip   animClip;


    #region States

    [Header("States")]
    [SerializeField] WallControlState wallControlState;
    [SerializeField] AirState airState;

    #endregion


    [Header("Effects")]
    [SerializeField] GameObject _slideEffect;

    private GameObject      obj;
    private float timer;

    #region Callback Functions

    public override void Enter()
    {
        base.Enter();

        if(Body.velocity.y > 0)
        {
            Body.velocity = new Vector2(Body.velocity.x, 0);
        }

        timer = 0f;

        Animator.Play(animClip.name);
        
    }

    public override void Do()
    {
        base.Do();

        

        timer += Time.deltaTime;

        if (
            Body.velocity.y < 0 &&
            timer >= 0.15f
            )
        {
            SlideParticles();
            timer = 0f;
        }

        if (
            Data.TimeLastOnWall > 0 ||
            core.collisionSensors.IsGrounded
            )
        {
            Destroy(obj, 0f);
            IsComplete = true;
        }
    }

    public override void FixedDo()
    {
        base.FixedDo();

        Slide();
    }

    public override void Exit()
    {
        base.Exit();
        timer = 0f;
        Destroy(obj, 0f);
        wallControlState.SetWallSliding(false);
    }

    #endregion


    #region Checks

    /// <summary>
    /// Checks if the player is able to wall slide.
    /// </summary>
    /// <returns>True if the player can wall slide.</returns>
    public bool CanSlide()
    {
        if (
            Data.TimeLastOnWall > 0 &&
            Data.TimeLastOnGround <= 0 &&
            !airState.IsJumping &&
            !wallControlState.IsWallJumping
        )
            return true;
        else
            return false;
    }

    #endregion


    #region Functionality

    /// <summary>
    /// Counteracts gravity on the player's <c>RigidBody2D</c> component while input is held.
    /// </summary>
    private void Slide()
    {
        // works the same as the run, but only in the y-axis
        float speedDiff = Data.slideSpeed - Body.velocity.y;
        float movement = speedDiff * Data.slideAcceleration;

        // the movement is clamped here to prevent any over corrections (not that noticable in the run, so not implemented there)
        // the force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called
        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDiff) * (1 / Time.fixedDeltaTime));

        Body.AddForce(movement * Vector2.up);
    }

    #endregion


    #region Effects

    private void SlideParticles()
    {
        obj = Instantiate(
            _slideEffect,
            transform.position + (Vector3.right * 0.5f * player.FacingDirection) + (Vector3.up * 0.8f),
            Quaternion.Euler(-90, 0, 0),
            gameObject.transform
            );

        // if(!core.collisionSensors.IsWallLeft && !core.collisionSensors.IsWallRight)
        // {
        //     Destroy(obj, 0.1f);
        // }
        Destroy(obj, 1f);
    }

    #endregion
}