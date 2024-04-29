using UnityEngine;

public class WallSlideState : State
{
    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("Effects")]
    [SerializeField]
    private GameObject      _slideEffect;

    private GameObject      obj;

    public override void Enter()
    {
        base.Enter();

        Animator.Play(animClip.name);
        
    }

    public override void Do()
    {
        base.Do();

        Body.velocity = new Vector2(Body.velocity.x, 0);

        if(Body.velocity.y < 0)
            SlideParticles();

        if(core.collisionSensors.IsGrounded || (!core.collisionSensors.IsWallLeft && !core.collisionSensors.IsWallRight))
        {
            IsComplete = true;
        }
    }

    public override void Exit()
    {
        base.Exit();

        Destroy(obj, 0f);
    }

    private void SlideParticles()
    {
        obj = Instantiate(
            _slideEffect,
            transform.position + (Vector3.right * 0.5f),
            Quaternion.Euler(0, 0, 0),
            gameObject.transform
            );

        // if(!core.collisionSensors.IsWallLeft && !core.collisionSensors.IsWallRight)
        // {
        //     Destroy(obj, 0.1f);
        // }
        Destroy(obj, 1f);
    }
}