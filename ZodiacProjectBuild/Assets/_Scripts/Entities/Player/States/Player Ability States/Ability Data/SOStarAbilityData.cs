using Cinemachine;
using UnityEngine;

[CreateAssetMenu(fileName = "StarData", menuName = "Data/Ability Data/Test/Star Ability Data")]
public class SOStarAbilityData : SOAbilityData
{
    [Header("Light Shaft Appearance Settings")]
    public GameObject LightPrefab;
    public GameObject InitialLightPrefab;
    public GameObject FloorLightPrefab;
    [Range(0f, 360f)] public float LightMaxInnerAngle = 17.607f;
    [Range(0f, 360f)] public float LightMaxOuterAngle = 30.11f;
    [Range(0f, 50f)] public float LightMaxOuterRadius = 11.82147f;

    [Header("Light Shaft Timers")]
    [Range(1, 20)] public int HealingBeamHealFactor = 3;
    [Range(1f, 100f)] public float HealingBeamActiveTime = 20f;
    [Range(0.1f, 10f)] public float HealingBeamInterHealCooldown = 2f;
    [Range(0.01f, 5f)] public float LightShaftFlashTimer = 0.2f;

    [Header("Input")]
    [Range(0f, 10f)] public float MinHoldTime = 2f;

    [Header("Rumble")]
    [Range(0f, 1f)] public float RumbleConstLowFreq = 0.2f;
    [Range(0f, 1f)] public float RumbleConstHighFreq = 0.4f;
    [Range(0f, 1f)] public float RumbleBurstLowFreq = 0.8f;
    [Range(0f, 1f)] public float RumbleBurstHighFreq = 0.9f;
    [Range(0f, 1f)] public float RumbleBurstTime = 0.08f;
}
