using System.Collections.Generic;

public class Modifiers<TModifierType, TValueType> where TModifierType: Modifier<TValueType>
{
    readonly List<TModifierType> _modifierList = new List<TModifierType>();

    public TValueType ApplyAllModifiers(TValueType initialValue)
    {
        var modifiedValue = initialValue;

        foreach (var modifier in _modifierList)
        {
            modifiedValue = modifier.ModifyValue(modifiedValue);
        }

        return modifiedValue;
    }

    public void AddModifier(TModifierType modifier)
    => _modifierList.Add(modifier);

    public void RemoveModifier(TModifierType modifier)
    => _modifierList.Remove(modifier);
}
