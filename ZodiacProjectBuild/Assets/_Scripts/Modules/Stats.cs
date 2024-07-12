using UnityEngine;

public class Stats : MonoBehaviour
{
    [field: SerializeField] public Stat Health { get; private set; }
    [field: SerializeField] public Stat Poise { get; private set; }

    [SerializeField] float PoiseRecoveryRate;

    private void Awake() 
    {
        Health.Init();
        Poise.Init();
    }

    private void Update()
    {
        if (Poise.CurrentValue.Equals(Poise.MaxValue))
            return;

        Poise.Increase(PoiseRecoveryRate * Time.deltaTime);    
    }
}
