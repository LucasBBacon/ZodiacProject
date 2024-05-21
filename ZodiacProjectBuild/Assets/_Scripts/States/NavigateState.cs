using UnityEngine;

public class NavigateState : State
{
    public Vector2 destination;
    public float speed = 1;
    public float threshold = 0.1f;
    public State animation;

    public override void Enter()
    {
        base.Enter();

        Set(animation, true);
    }

    public override void Do()
    {
        base.Do();

        if(Vector2.Distance(core.transform.position, destination) < threshold)
        {
            IsComplete = true;
        }

        FaceDestination();
    }

    public override void FixedDo()
    {
        base.FixedDo();

        Vector2 direction = (destination - (Vector2)core.transform.position).normalized;

        Body.velocity = new Vector2(direction.x * speed, Body.velocity.y);
    }

    void FaceDestination()
    {
        core.transform.localScale = new Vector3(Mathf.Sign(Body.velocity.x), 1, 1);
    }
}