using System.Collections;
using UnityEngine;

public class DashState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;


    #region States

    [Header("States")]
    [SerializeField] RunState runState;
    [SerializeField] AirState airState;
    [SerializeField] WallControlState wallControlState;

    #endregion


    [Header("Effects")]
    [SerializeField] GameObject _dashEffect;

    #region Blackboard Variables

    [HideInInspector] public bool IsDashing { get; private set; }
    [HideInInspector] public int DashesLeft { get; private set; } = 2;
    [HideInInspector] public bool IsDashRefilling { get; private set; }
    [HideInInspector] public bool IsDashAttacking { get; private set; }

    //[HideInInspector] 
    public Vector2 LastDashDir;

    [HideInInspector] Player _player;

    #endregion


    #region Callback Functions

    private void Awake()
    {
        _player = GetComponentInParent<Player>();
    }

    public override void Enter()
    {
        base.Enter();

        Animator.Play(animClip.name);

        
    }

    public override void Do()
    {
        base.Do();

        StartCoroutine(PerformSleep(Data.dashSleepTime));

        if(UserInput.instance.MoveInput != Vector2.zero)
            LastDashDir = UserInput.instance.MoveInput;
        else
            LastDashDir = _player.IsFacingRight ? Vector2.right : Vector2.left;

        IsDashing = true;

        airState.SetIsJumping(false);
        airState.SetIsJumpCut(false);

        wallControlState.SetWallJumping(false);

        StartCoroutine(StartDash(LastDashDir));
        // start dash method, check for direction

        if(core.collisionSensors.IsGrounded)
            DashParticles();

        IsComplete = true;
    }

    public override void FixedDo()
    {
        base.FixedDo();

        if (IsDashAttacking)
            runState.Run(Data.dashEndRunLerp);
    }

    #endregion


    #region Checks

    public bool CanDash()
    {
        if
        (
            !IsDashing &&
            !IsDashRefilling &&
            Data.TimeLastOnGround > 0 &&
            DashesLeft < Data.dashAmount
        )
            StartCoroutine(DashRefill(1));

        return DashesLeft > 0;
    }

    #endregion


    #region Functionality

    public IEnumerator StartDash(Vector2 direction)
    {
        Data.TimeLastOnGround = 0;
        Data.TimeLastPressedDash = 0;

        float stateTime = Time.time;

        DashesLeft--;
        IsDashAttacking = true;

        Body.gravityScale = 0;

        while (Time.time - stateTime <= Data.dashAttackTime)
        {
            Body.velocity = Data.dashSpeed * direction.normalized;

            yield return null;
        }

        startTime = Time.time;

        IsDashAttacking = false;

        Body.gravityScale = Data.gravityScale;

        Body.velocity = Data.dashEndSpeed * direction.normalized;

        while (Time.time - startTime <= Data.dashEndTime)
            yield return null;

        IsDashing = false;
    }

    public IEnumerator DashRefill(int amount)
    {
        IsDashRefilling = true;

        yield return new WaitForSeconds(Data.dashRefillTime);

        IsDashRefilling = false;
        DashesLeft = Mathf.Min(Data.dashAmount, DashesLeft + amount);
    }

    #endregion


    #region Effects

    private void DashParticles()
    {
        GameObject obj = Instantiate(
            _dashEffect,
            transform.position - (Vector3.up * 0.8f),
            Quaternion.Euler(-90, 0, 0),
            gameObject.transform
            );
        Destroy(obj, 0.4f);
    }

    #endregion

    #region Helper Methods

    public IEnumerator PerformSleep(float duration)
        {
            Time.timeScale = 0;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1;
        }

    #endregion
}