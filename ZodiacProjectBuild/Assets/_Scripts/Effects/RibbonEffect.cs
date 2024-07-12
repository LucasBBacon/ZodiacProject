using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RibbonEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] int length;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Vector3[] segmentPoses;
    Vector3[] segmentVelocities;

    [Header("Parameters")]
    public float targetRotation;
    [SerializeField] Transform targetDirection;
    [SerializeField] float targetDistance;
    [SerializeField] float smoothSpeed;
    [SerializeField] float trailSpeed;

    [Header("Wiggle")]
    [SerializeField] Transform wiggleDirection;
    [SerializeField] float wiggleSpeed;
    [SerializeField] float wiggleMagnitude;

    private void Start()
    {
        lineRenderer.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentVelocities = new Vector3[length];
    }

    private void Update()
    {
        // targetDirection.localRotation = new Quaternion
        //     (
        //         0,
        //         0,
        //         targetRotation,
        //         0
        //     );

        wiggleDirection.localRotation = Quaternion.Euler
            (
                0,
                0,
                Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude
            );

        segmentPoses[0] = targetDirection.position;

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = Vector3.SmoothDamp
                (
                    segmentPoses[i],
                    segmentPoses[i - 1] + targetDirection.right * targetDistance,
                    ref segmentVelocities[i],
                    smoothSpeed + i / trailSpeed
                );
        }

        lineRenderer.SetPositions(segmentPoses);
    }
}
