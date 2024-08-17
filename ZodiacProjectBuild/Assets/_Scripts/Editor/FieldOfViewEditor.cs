using UnityEditor;
using UnityEngine;

[CustomEditor (typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fov = (FieldOfView)target;

        Handles.color = Color.white;
        Handles.DrawWireArc(
            fov.headPosition,
            Vector3.forward,
            Vector3.right,
            360f,
            fov.viewRadius
        );

        Handles.color = Color.yellow;
        Handles.DrawWireArc(
            fov.headPosition,
            Vector3.forward,
            Vector3.right,
            360f,
            fov.minAgroDist
        );

        Handles.color = Color.red;
        Handles.DrawWireArc(
            fov.headPosition,
            Vector3.forward,
            Vector3.right,
            360f,
            fov.meeleDist
        );

        Handles.color = Color.white;
        Vector3 viewAngleA = fov.DirFromAngle(-fov.viewAngle / 2, false);
        Vector3 viewAngleB = fov.DirFromAngle(fov.viewAngle / 2, false);

        Handles.DrawLine(
            fov.headPosition,
            fov.headPosition + viewAngleA * fov.viewRadius
            );
        Handles.DrawLine(
            fov.headPosition,
            fov.headPosition + viewAngleB * fov.viewRadius
            );

        if (fov.distToTarget <= fov.viewRadius && fov.distToTarget > fov.minAgroDist)
            Handles.color = Color.green;
        else if (fov.distToTarget <= fov.minAgroDist && fov.distToTarget > fov.meeleDist)
            Handles.color = Color.yellow;
        else if (fov.distToTarget <= fov.meeleDist)
            Handles.color = Color.red;
        
        if (fov.target != null)
            Handles.DrawLine(
                fov.headPosition,
                fov.target.position
                );
        
    }
}
