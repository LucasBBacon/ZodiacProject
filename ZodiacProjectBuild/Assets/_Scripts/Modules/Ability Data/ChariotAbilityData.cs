using UnityEngine;

[CreateAssetMenu(fileName = "newChariotData", menuName = "Data/Ability Data/Chariot Ability Data")]
public class ChariotAbilityData : AbilityData
{
    [Header("Chariot")]
    [Range(0, 5)] public int NumberOfChariots = 2;
    [Range(0f, 1f)] public float ChariotTime = 0.11f;
    [Range(1f, 200f)] public float ChariotSpeed = 40f;
    [Range(0f, 1f)] public float TimeBetweenChariotGround = 0.225f;
    public bool ResetChariotOnWallSlide = true;
    [Range(0f, 0.5f)] public float ChariotDiagonallyBias = 0.4f;

    [Header("Chariot Cancel Time")]
    [Range(0.01f, 5f)] public float ChariotGravityOnReleaseMultiplier = 1f;
    [Range(0.02f, 0.3f)] public float ChariotTimeForUpwardsCancel = 0.027f;

    public readonly Vector2[] ChariotDirections = new Vector2[]
    {
        new Vector2(0, 0), // nothing
        new Vector2(1, 0), // right
        new Vector2(1, 1).normalized, // top-right
        new Vector2(0, 1), // up
        new Vector2(-1, 1).normalized, // top left
        new Vector2(-1, 0), // left
        new Vector2(-1, -1).normalized, // bottom left
        new Vector2(0, -1), // down
        new Vector2(1, -1).normalized // bottom right
    };
}