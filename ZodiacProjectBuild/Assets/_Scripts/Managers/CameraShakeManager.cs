using Cinemachine;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instance;
    [SerializeField] float globalShakeForce;

    CinemachineImpulseDefinition _definition;
    CinemachineImpulseListener _impulseListener;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        _impulseListener = Camera.main.GetComponent<CinemachineImpulseListener>();    
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }

    public void CameraShakeFromProfile(SOScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        // apply settings
        SetUpCameraShakeSettings(profile, impulseSource);

        // screenshake
        impulseSource.GenerateImpulseWithForce(profile.ImpulseForce);
    }

    void SetUpCameraShakeSettings(SOScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
    {
        _definition = impulseSource.m_ImpulseDefinition;

        // change impulse source settings
        _definition.m_ImpulseDuration = profile.ImpulseTime;
        impulseSource.m_DefaultVelocity = profile.DefaultVelocity;
        _definition.m_CustomImpulseShape = profile.ImpulseCurve;

        // change impulse listener settings
        _impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.ListenerAmplitude;
        _impulseListener.m_ReactionSettings.m_FrequencyGain = profile.ListenerFrequency;
        _impulseListener.m_ReactionSettings.m_Duration = profile.ListenerDuration;
    }
}