using UnityEngine;

public class DashState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("Effects")]
    [SerializeField]
    private GameObject      _dashEffect;

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
}