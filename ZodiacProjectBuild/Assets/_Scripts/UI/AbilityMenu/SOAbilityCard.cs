using UnityEngine;

[CreateAssetMenu(fileName = "newAbilityCardData", menuName = "Data/Ability Data/Card Details")]
public class SOAbilityCard : ScriptableObject
{
    public Sprite CardImage;
    public string CardName;
    public int CardIndex;
}
