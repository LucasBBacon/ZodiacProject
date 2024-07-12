public interface IKnockbackable
{
    public void Knockback(KnockbackData data);
    public float MaxKnockbackTime { get; set; }
    public bool HasKnockbacked { get; set; }
    public bool IsNotKnockbackable { get; set; }
}
