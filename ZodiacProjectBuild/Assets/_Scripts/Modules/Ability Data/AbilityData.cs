using UnityEngine;

[CreateAssetMenu(fileName = "newAbilityData", menuName = "Data/Entity Data/Ability Data")]
public class AbilityData : ScriptableObject
{
    [Header("Cooldown")]
    public float AbilityCooldown = 0.5f;
}
