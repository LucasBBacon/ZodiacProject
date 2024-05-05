using UnityEngine;

public class WallSlideState : State
{
    public Player player;

    [Header("Animation Clip")]
    public  AnimationClip   animClip;

    [Header("Effects")]
    [SerializeField]
    private GameObject      _slideEffect;

    private GameObject      obj;
    private float timer;

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

        if(Body.velocity.y < 0 && timer >= 0.15f)
        {
            SlideParticles();
            timer = 0f;
        }

        if(core.collisionSensors.IsGrounded || (!core.collisionSensors.IsWallLeft && !core.collisionSensors.IsWallRight))
        {
            Destroy(obj, 0f);
            IsComplete = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        timer = 0f;
        Destroy(obj, 0f);
    }

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
}