using UnityEngine;

[CreateAssetMenu(fileName = "newAbilityData", menuName = "Data/Entity Data/Ability Data")]
public class SOAbilityData : ScriptableObject
{
    [Header("Cooldown")]
    public float AbilityCooldown = 0.5f;
    public float ManaCost = 10f;
}
