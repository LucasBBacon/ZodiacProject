using UnityEngine;

public class FlyingBirdEnemy : EnemyEntity
{
    #region References

    [SerializeField] Transform attackTransform;
    [SerializeField] GameObject forceField;
    [SerializeField] SOWeaponData weaponData;

    #endregion


    #region States

    public FlyingBirdIdleState IdleState { get; private set;}
    public FlyingBirdMoveState MoveState { get; private set;}

    public FlyingBirdChargeState ChargeState { get; private set;}
    public FlyingBirdLookForPlayerState LookForPlayerState { get; private set;}
    public FlyingBirdPlayerDetectedState PlayerDetectedState { get; private set;}

    public FlyingBirdDamageState DamageState { get; private set;}

    #endregion
    

    #region Callback Functions

    public override void Awake()
    {
        base.Awake();

        IdleState = new FlyingBirdIdleState(this, stateMachine, this, "idle");
        MoveState = new FlyingBirdMoveState(this, stateMachine, this, "isFlying");

        ChargeState = new FlyingBirdChargeState(this, stateMachine, this, "isWalking");
        LookForPlayerState = new FlyingBirdLookForPlayerState(this, stateMachine, this, "idle");
        PlayerDetectedState = new FlyingBirdPlayerDetectedState(this, stateMachine, this, "playerDetected");

        DamageState = new FlyingBirdDamageState(this, stateMachine, this, "damage");
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
}