using UnityEngine;

public class LandState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("State")]
    [SerializeField] GroundedState groundedState;
    [SerializeField] AirState airState;

    [Header("Effects")]
    [SerializeField]
    private GameObject      _landEffects;

    #region Callback Functions

    public override void Enter()
    {
        base.Enter();

        airState.SetIsJumping(false);

        Animator.Play(animClip.name);
        LandParticles();
    }

    public override void Do()
    {
        base.Do();

        if (
            Time.deltaTime - startTime >= animClip.length
            )
        {
            IsComplete = true;
            return;
        }
    }

    #endregion


    #region Effects

    private void LandParticles()
    {
        GameObject obj = Instantiate(
            _landEffects,
            transform.position - (Vector3.up * 0.8f),
            Quaternion.Euler(-90, 0, 0)
            );
        Destroy(obj, 1);  
    }

    #endregion
}