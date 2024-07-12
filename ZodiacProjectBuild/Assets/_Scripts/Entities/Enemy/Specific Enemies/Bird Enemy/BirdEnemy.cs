using UnityEngine;

public class BirdEnemy : EnemyEntity
{
    #region References

    [SerializeField] Transform attackTransform;
    [SerializeField] GameObject forceField;
    [SerializeField] WeaponData weaponData;

    #endregion


    #region States

    public BirdIdleState IdleState { get; private set;}
    public BirdMoveState MoveState { get; private set;}

    public BirdChargeState ChargeState { get; private set;}
    public BirdLookForPlayerState LookForPlayerState { get; private set;}
    public BirdPlayerDetectedState PlayerDetectedState { get; private set;}

    public BirdMeleeAttackState MeleeAttackState { get; private set;}
    public BirdDamageState DamageState { get; private set;}

    #endregion

    public const string IS_WALKING = "isWalking";
    public const string IDLE = "idle";
    public const string LOOK_FOR_PLAYER = "lookForPlayer";
    public const string PLAYER_DETECTED = "playerDetected";
    public const string ATTACK = "attack";
    public const string LAND = "land";
    public const string FALL = "fall";
    public const string DAMAGE = "damage";

    #region Callback Functions

    public override void Awake()
    {
        base.Awake();

        IdleState = new BirdIdleState(this, stateMachine, this);
        MoveState = new BirdMoveState(this, stateMachine, this);

        ChargeState = new BirdChargeState(this, stateMachine, this);
        LookForPlayerState = new BirdLookForPlayerState(this, stateMachine, this);
        PlayerDetectedState = new BirdPlayerDetectedState(this, stateMachine, this);

        MeleeAttackState = new BirdMeleeAttackState(this, stateMachine, attackTransform, this, weaponData);
        DamageState = new BirdDamageState(this, stateMachine, this);
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

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTransform.position, weaponData.AttackRange);
    }

    #endregion
}
