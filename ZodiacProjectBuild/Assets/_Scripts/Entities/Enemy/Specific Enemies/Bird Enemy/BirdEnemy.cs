using UnityEngine;

public class BirdEnemy : EnemyEntity
{
    #region References

    [SerializeField] Transform attackTransform;
    [SerializeField] GameObject forceField;
    [SerializeField] SOWeaponData weaponData;

    #endregion


    #region States

    public BirdIdleState IdleState { get; private set;}
    public BirdMoveState MoveState { get; private set;}

    public BirdChargeState ChargeState { get; private set;}
    public BirdLookForPlayerState LookForPlayerState { get; private set;}
    public BirdPlayerDetectedState PlayerDetectedState { get; private set;}

    public BirdMeleeAttackState MeleeAttackState { get; private set;}
    public BirdDamageState DamageState { get; private set;}
    public BirdDeadState DeadState { get; private set;}
    public BirdSpecialAttackState SpecialAttackState { get; private set; }

    #endregion
    

    #region Callback Functions

    public override void Awake()
    {
        base.Awake();

        IdleState = new BirdIdleState(this, stateMachine, this, "idle");
        MoveState = new BirdMoveState(this, stateMachine, this, "isWalking");

        ChargeState = new BirdChargeState(this, stateMachine, this, "isWalking");
        LookForPlayerState = new BirdLookForPlayerState(this, stateMachine, this, "idle");
        PlayerDetectedState = new BirdPlayerDetectedState(this, stateMachine, this, "playerDetected");

        MeleeAttackState = new BirdMeleeAttackState(this, stateMachine, "attack", attackTransform, this, weaponData);
        DamageState = new BirdDamageState(this, stateMachine, this, "damage");
        DeadState = new BirdDeadState(this, stateMachine, "death");
        SpecialAttackState = new BirdSpecialAttackState(this, stateMachine, "ability", attackTransform, this);
    }

    public override void Start()
    {
        base.Start();
        
        stateMachine.Initalize(MoveState);    
    }

    public override void Update()
    {
        base.Update();

        if (ChangeToDamageState)
        {
            ChangeState(DamageState);
        }    
    }

    public void DestroyProjectile(GameObject obj, float timeToDestroy)
    {
        Destroy(obj, timeToDestroy);
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTransform.position, EntityData.AttackRadius);
    }

    #endregion

    public override void Die()
    {
        base.Die();

        ChangeState(DeadState);
    }
}
