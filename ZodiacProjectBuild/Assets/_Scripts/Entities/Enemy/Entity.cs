using UnityEngine;

public class Entity : MonoBehaviour
{
    [Header("Entity References")]
    public Animator Animator;
    public CollisionSensors CollisionSensors;
    public Movement Movement;
    public Stats Stats;
    public DamageFlash DamageFlash;
    public ParticleManager ParticleManager;
    public GameObject DamageParticles;
    public bool ChangeToDamageState;

    public virtual void Start()
    {
        Stats = GetComponentInChildren<Stats>();
        ParticleManager = GetComponent<ParticleManager>();
        DamageFlash = GetComponent<DamageFlash>();

        Animator = GetComponentInChildren<Animator>();
    }
}