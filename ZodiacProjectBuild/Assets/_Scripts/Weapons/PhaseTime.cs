using System;
using UnityEngine;

[Serializable]
public struct PhaseTime
{
    public float Duration { get; private set; }
    public AttackPhases Phase { get; private set; }

    public bool TryGetTriggerTime(AttackPhases phase, out float triggerTime)
    {
        triggerTime = Time.time + Duration;
        return phase == Phase;
    }
}
