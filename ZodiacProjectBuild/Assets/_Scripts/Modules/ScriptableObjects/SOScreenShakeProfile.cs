using UnityEngine;

[CreateAssetMenu(fileName = "ScreenShake", menuName = "Effects/Screen Shake")]
public class SOScreenShakeProfile : ScriptableObject
{
    [Header("Impulse Source Settings")]
    public float ImpulseTime = 0.2f;
    public float ImpulseForce = 1f;
    public Vector3 DefaultVelocity = new Vector3(0f, -1f, 0f);
    public AnimationCurve ImpulseCurve;

    [Header("Impulse Listener Settings")]
    public float ListenerAmplitude = 1f;
    public float ListenerFrequency = 1f;
    public float ListenerDuration = 1f;
}