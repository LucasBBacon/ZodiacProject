using UnityEngine;

public class LandState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("Effects")]
    [SerializeField]
    private GameObject      _landEffects;

    public override void Enter()
    {
        base.Enter();

        Animator.Play(animClip.name);
        LandParticles();
    }

    public override void Do()
    {
        base.Do();

        if(!core.collisionSensors.IsGrounded)
            IsComplete = true;
    }

    private void LandParticles()
    {
        GameObject obj = Instantiate(
            _landEffects,
            transform.position - (Vector3.up * 0.8f),
            Quaternion.Euler(-90, 0, 0)
            );
        Destroy(obj, 1);  
    }
}