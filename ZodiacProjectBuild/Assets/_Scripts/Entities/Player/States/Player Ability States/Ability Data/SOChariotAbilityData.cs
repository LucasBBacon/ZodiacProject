using UnityEngine;

[CreateAssetMenu(fileName = "newChariotData", menuName = "Data/Ability Data/Chariot Ability Data")]
public class SOChariotAbilityData : SOAbilityData
{
    [Header("Chariot")]
    [Range(0, 5)] public int NumberOfChariots = 1;

    [Header("Speeds")]
    [Range(1f, 200f)] public float ChariotSpeed = 40f;
    [Range(1f, 200f)] public float ChariotEndSpeed = 15f;

    [Header("Timers")]
    [Range(0f, 1f)] public float ChariotInputBufferTime = 0.125f;
    [Range(0f, 1f)] public float ChariotSleepTime = 0.04f;
    [Range(0f, 1f)] public float ChariotEndTime = 0.11f;
    public float ChariotMinTime = 0.14f;
    public float ChariotMaxExtraTime = 0.8f;
    [HideInInspector] public float[] ChariotBurstTimers = new float[3];

    [Header("Input")]
    public float InputHoldMaxTime = 4f;

    [Header("Rumble")]
    [Range(0f, 1f)] public float RumbleLowFreq = 0.1f;
    [Range(0f, 1f)] public float RumbleHighFreq = 0.2f;
    [Range(0f, 1f)] public float RumbleTime = 0.16f;


    private void OnValidate()
    {
        for (int i = 0; i < ChariotBurstTimers.Length; i++)
        {
            ChariotBurstTimers[i] = InputHoldMaxTime * (((float)i + 1) / ChariotBurstTimers.Length);
        }
    }
}