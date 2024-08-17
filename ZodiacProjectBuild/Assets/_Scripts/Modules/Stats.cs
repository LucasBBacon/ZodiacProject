using UnityEngine;

public class Stats : MonoBehaviour
{
    [field: SerializeField] public Stat Health { get; private set; }
    [field: SerializeField] public Stat Poise { get; private set; }
    [field: SerializeField] public Stat Mana { get; private set; }

    [SerializeField] float PoiseRecoveryRate;
    [SerializeField] float ManaRecoveryRate;

    private void Awake() 
    {
        Health.Init();
        Poise.Init();
        Mana.Init();
    }

    private void Update()
    {
        if (!Poise.CurrentValue.Equals(Poise.MaxValue))
            Poise.Increase(PoiseRecoveryRate * Time.deltaTime);

        if (!Mana.CurrentValue.Equals(Mana.MaxValue))
            Mana.Increase(ManaRecoveryRate * Time.deltaTime);
    }
}
