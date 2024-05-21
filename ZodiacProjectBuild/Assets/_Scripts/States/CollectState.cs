using System.Collections.Generic;
using UnityEngine;

public class CollectState : State
{
    public List<Transform> collectibles;
    public Transform target;
    public NavigateState navigateState;
    public IdleState idleState;

    public float collectRadius;
    public float vision = 1;

    public override void Enter()
    {
        base.Enter();

        navigateState.destination = target.position;

        Set(navigateState, true);
    }

    public override void Do()
    {
        base.Do();

        if(state == navigateState)
        {
            ChaseTarget();
        }
        else
        {
            EndPursuit();
        }

        if(target == null)
        {
            IsComplete = true;
            return;
        }
    }

    void ChaseTarget()
    {
        if(IsWithinReach(target.position))
        {
            Set(idleState, true);
            Body.velocity = new Vector2(0, Body.velocity.y);
            target.gameObject.SetActive(false);
        }

        else if(!IsInVision(target.position))
        {
            Set(idleState, true);
            Body.velocity = new Vector2(0, Body.velocity.y);
        }

        else
        {
            navigateState.destination = target.position;
            Set(navigateState, true);
        }
    }

    public bool IsWithinReach(Vector2 targetPos) => Vector2.Distance(core.transform.position, targetPos) < collectRadius;

    public bool IsInVision(Vector2 targetPos) => Vector2.Distance(core.transform.position, targetPos) < vision;

    public void CheckForTarget()
    {
        foreach(Transform collectible in collectibles)
        {
            if(IsInVision(collectible.position) && collectible.gameObject.activeSelf)
            {
                target = collectible;
                return;
            }
        }

        target = null;
    }

    void EndPursuit()
    {
        if(state.StateTime > 2)
            IsComplete = true;
    }
}