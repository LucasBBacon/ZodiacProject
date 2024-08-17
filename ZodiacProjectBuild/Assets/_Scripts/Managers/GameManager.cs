using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Player player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    public void SetPlayerAbility(int cardAbility, int cardSlot)
    {
        if (player != null)
            player.GenerateAbilityState(cardAbility, cardSlot);
    }

    public void GivePlayerAbility(BaseAblity abilityToGive)
    {
        switch (abilityToGive)
        {
            case BaseAblity.Dash:
                player.DashEnabled = true;
                break;
            case BaseAblity.WallJump:
                player.WallJumpEnabled = true;
                break;
            case BaseAblity.DoubleJump:
                player.DoubleJumpEnabled = true;
                break;
            case BaseAblity.AirDash:
                player.DashEnabled = true;
                break;
            default:
                break;
        }
    }

    #region Spawn Methods

    public void SpawnObject(GameObject objectToSpawn, Vector3 positionToSpawn)
    {
        Instantiate(objectToSpawn, positionToSpawn, Quaternion.identity);
    }

    public GameObject SpawnObjectOBJ(GameObject objectToSpawn, Vector3 positionToSpawn, Quaternion rotation)
    {
        return Instantiate(objectToSpawn, positionToSpawn, rotation);
    }

    #endregion
}
