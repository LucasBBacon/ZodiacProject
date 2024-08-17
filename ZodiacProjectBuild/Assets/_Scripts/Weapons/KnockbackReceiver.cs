using UnityEngine;

public class KnockbackReceiver : IKnockbackable
{
    public Modifiers<Modifier<KnockbackData>, KnockbackData> Modifiers { get; } = new();

    public float MaxKnockbackTime { get; set; }
    public bool HasKnockbacked { get; set; }
    public bool IsNotKnockbackable { get; set; }

    [SerializeField] float _maxKnockbackTime = 0.2f;

    bool _isKnockbackActive;
    float _knockbackStartTime;

    Movement _movement;
    CollisionSensors _collisionSensors;

    private void Update()
    {
        CheckKnockback();
    }

    void CheckKnockback()
    {
        if (
            _isKnockbackActive
            && ((_movement.CurrentVelocity.y <= 0.01f && _collisionSensors.IsGrounded)
            || Time.time >= _knockbackStartTime)
        )
        {
            _isKnockbackActive = false;
            _movement.CanSetVelocity = true;
        }
    }

    public void Knockback(KnockbackData data)
    {
        data = Modifiers.ApplyAllModifiers(data);

        _movement.SetVelocity(data.Strength, data.Angle, data.Direction);
        _movement.CanSetVelocity = false;
        _isKnockbackActive = true;
        _knockbackStartTime = Time.time;
    }
}

