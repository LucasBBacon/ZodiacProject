using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField] private EntityData _entityData;

    public int currentHealth;

    private void Awake() 
    {
        currentHealth = _entityData.maxHealth;
    }
}