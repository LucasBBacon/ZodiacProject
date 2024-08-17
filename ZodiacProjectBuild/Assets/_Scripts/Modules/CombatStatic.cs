using System.Collections;
using UnityEngine;

public class CombatStatic : MonoBehaviour, IDamageable, IKnockbackable
{
    [field:SerializeField] public float InvincibilityTime { get; set; } = 0.3f;
    public bool IsInvincible { get; set; } = false;
    public bool HasTakenDamage { get; set; }

    [field:SerializeField] public float MaxKnockbackTime { get; set; } = 0.4f;
    public bool HasKnockbacked { get; set; }

    private bool _canSetVelocity = true;

    public bool IsNotKnockbackable { get; set; } = false;

    #region Entity References

    [SerializeField] Stats _stats;
    [SerializeField] GameObject _damageParticles;
    [SerializeField] ParticleManager _particleManager;
    [SerializeField] DamageFlash _damageFlash;

    Rigidbody2D _body;

    #endregion


    float _knockbackStartTime;

    #region Callback Functions

    private void Awake()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckKnockback();
    }

    private void OnEnable()
    {
        _stats.Health.OnCurrentValueZero += Die;
    }

    private void OnDisable()
    {
        _stats.Health.OnCurrentValueZero -= Die;    
    }

    #endregion


    #region Damage

    public void Damage(DamageData data)
    {
        if (data.Amount <= 0f)
        {
            return;
        }

        HasTakenDamage = true;
        Debug.Log("Damaged for: " + data.Amount);

        _stats.Health.Decrease(data.Amount);
        Debug.Log(_stats.Health.CurrentValue);
        _damageFlash.CallDamageFlash();

        //SpawnDamageParticles(data.Source.transform.position);

        IsInvincible = true;
        StartCoroutine(ResetInvicible());
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    #endregion


    #region Knockback

    public void Knockback(KnockbackData data)
    {
        HasKnockbacked = true;
        
        if (_canSetVelocity)
            _body.AddForce(
                new Vector2(
                    data.Strength * data.Angle.normalized.x * data.Direction,
                    data.Strength * data.Angle.normalized.y
                    ),
                ForceMode2D.Impulse
            );
        
        _canSetVelocity = false;
        
        IsNotKnockbackable = true;
        _knockbackStartTime = Time.time;
    }

    private void CheckKnockback()
    {
        if (
            HasKnockbacked
            && (Time.time >= _knockbackStartTime + MaxKnockbackTime)    
            )
        {  
            HasKnockbacked = false;
            
            _canSetVelocity = true;
        }
    }

    #endregion

    IEnumerator ResetInvicible()
    {
        yield return new WaitForSeconds(InvincibilityTime);

        IsInvincible = false;
        IsNotKnockbackable = false;
    }

    #region Effects

    void SpawnDamageParticles(Vector3 sourcePos)
    {
        Vector3 relativePos = gameObject.transform.InverseTransformDirection(sourcePos);
        Quaternion rotation = Quaternion.FromToRotation
            (
                Vector2.right,
                Vector2.right * (-Mathf.Sign(relativePos.x))
            );

        _particleManager.SpawnParticlesRelative
            (
                _damageParticles,
                new Vector2(0.2f, 0f),
                rotation
            );
    }

    #endregion
}