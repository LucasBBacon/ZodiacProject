using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State
{
    public AnimationClip animClip;

    [SerializeField]
    private GameObject _jumpEffect;

    public override void Enter()
    {
        base.Enter();

        Animator.Play(animClip.name);
        JumpParticles();
    }

    public override void Do()
    {
        base.Do();

        IsComplete = true;
    }

    // public void ResetAmountOfJumpsLeft()    => amountOfJumpsLeft = playerData.jumpAmount;
    // public void DecreaseAmountOfJumpsLeft() => amountOfJumpsLeft--;

    private void JumpParticles()
    {
        GameObject obj = Instantiate(
            _jumpEffect,
            transform.position - (Vector3.up * transform.localScale.y / 2f),
            Quaternion.Euler(-90, 0, 0)
            );
        Destroy(obj, 1);  
    }
}