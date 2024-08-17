using UnityEngine;

public class AbilityPickupTrigger : MonoBehaviour
{
    [SerializeField] BaseAblity _abilityToGive;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Player"))
        {
            GameManager.Instance.GivePlayerAbility(_abilityToGive);
            Destroy(gameObject);
        }
    }
}

public enum BaseAblity
{
    Dash,
    WallJump,
    AirDash,
    DoubleJump
}
