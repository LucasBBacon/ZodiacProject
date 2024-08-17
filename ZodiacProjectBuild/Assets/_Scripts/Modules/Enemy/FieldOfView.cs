using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [SerializeField] Vector2 offset;
    [Range(0, 360f)] public float viewAngle;
    
    public float minAgroDist;
    public float meeleDist;

    [HideInInspector] public Vector3 headPosition;
    public Transform target;
    public float distToTarget;

    public LayerMask obstacleMask;

    [SerializeField] EnemyEntity entity;

    Movement Movement => entity.Movement;
    SOEntityData EntityData => entity.EntityData;
    CollisionSensors CollisionSensors => entity.CollisionSensors;

    private void Start()
    {
        viewRadius = EntityData.MaxAgroDistance;
        minAgroDist = EntityData.MinAgroDistance;
        meeleDist = EntityData.CloseRangeActionDistance;
    }

    public bool FindVisibleTargets(float detectionRange)
    {
        headPosition = transform.position + (Vector3)offset;

        Collider2D targetInViewRadius = Physics2D.OverlapCircle(
            headPosition,
            viewRadius,
            EntityData.PlayerMask
            );

        if (targetInViewRadius == null)
        {
            target = null;
            return false;
        }
        
        Transform tempTarget = targetInViewRadius.transform;

        Vector3 dirToTarget = (tempTarget.position - headPosition).normalized;
        if (Vector3.Angle(
            transform.right,
            dirToTarget
            ) < viewAngle / 2)
        {
            distToTarget = Vector3.Distance(
                headPosition,
                tempTarget.position
                );

            if (!Physics2D.Raycast(
                headPosition,
                dirToTarget,
                distToTarget,
                CollisionSensors.GroundMask
                )
            )
            {
                
                //visibleTargets.Add(target);
                // if (distToTarget <= maxAngroDist && distToTarget > minAgroDist)
                //     return 1;
                // else if (distToTarget <= minAgroDist && distToTarget > meeleDist)
                //     return 2;
                // else if (distToTarget <= meeleDist)
                //     return 3;
                if (distToTarget <= detectionRange)
                {
                    target = tempTarget;
                    return true;
                }
            }
            else
            {
                target = null;
                return false;
            }
        }

        target = null;
        return false;
    }

    public Vector2 DirFromAngle(float angleDeg, bool angleIsGlobal)
    {
        angleDeg = (angleDeg + 90f) * Movement.FacingDirection;
        if (!angleIsGlobal)
        {
            angleDeg -= transform.eulerAngles.z;
        }
        return new Vector2(
            Mathf.Sin(angleDeg * Mathf.Deg2Rad),
            Mathf.Cos(angleDeg * Mathf.Deg2Rad)
            );
    }
}