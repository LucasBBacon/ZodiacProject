using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    [SerializeField] GameObject _fullSprite;
    [SerializeField] GameObject _brokenSprite;
    [SerializeField] GameObject _particlesToSpawn;

    public bool HasTakenDamage { get; set; }
    public float InvincibilityTime { get; set; }
    public bool IsInvincible { get; set; }

    void Start()
    {
        _fullSprite.SetActive(true);
        _brokenSprite.SetActive(false);
    }

    public void Damage(DamageData data)
    {
        if (data.Amount > 0)
        {
            HasTakenDamage = true;
            IsInvincible = true;
            Die();
        }
    }

    public void Die()
    {
        _fullSprite.SetActive(false);
        _brokenSprite.SetActive(true);

        GameObject.Instantiate(_particlesToSpawn, gameObject.transform.position, _particlesToSpawn.transform.rotation);
    }
}
