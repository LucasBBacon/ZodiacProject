public class PoiseDamageReceiver : IPoiseDamageable
{
    public Modifiers<Modifier<PoiseDamageData>, PoiseDamageData> Modifiers { get; } = new();

    Stats _stats;

    public void DamagePoise(PoiseDamageData data)
    {
        data = Modifiers.ApplyAllModifiers(data);

        _stats.Poise.Decrease(data.Amount);
    }
}

