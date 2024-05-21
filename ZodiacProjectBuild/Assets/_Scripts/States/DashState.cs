using System.Collections;
using UnityEngine;

public class DashState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("Effects")]
    [SerializeField]
    private GameObject      _dashEffect;

    [HideInInspector] public bool IsDashing;
    private int DashesLeft;
    private bool IsDashRefilling;
    private bool IsDashAttacking;

    #region Callback Functions

    public override void Enter()
    {
        base.Enter();

        Animator.Play(animClip.name);

        if(core.collisionSensors.IsGrounded)
            DashParticles();
    }

    public override void Do()
    {
        base.Do();

        IsComplete = true;
    }

    #endregion


    #region Checks

    public bool CanDash()
    {
        if
        (
            !IsDashing &&
            !IsDashRefilling &&
            core.collisionSensors.IsGrounded &&
            DashesLeft < Data.dashAmount
        )
            StartCoroutine(DashRefill(1));

        return DashesLeft > 0;
    }

    #endregion


    #region Functionality

    public IEnumerator StartDash(Vector2 direction)
    {
        float startTime = Time.time;

        DashesLeft--;
        IsDashAttacking = true;

        //Set gravity

        while (Time.time - startTime <= Data.dashAttackTime)
        {
            Body.velocity = direction.normalized * Data.dashSpeed;

            yield return null;
        }

        startTime = Time.time;

        IsDashAttacking = false;

        // Set gravity

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
}