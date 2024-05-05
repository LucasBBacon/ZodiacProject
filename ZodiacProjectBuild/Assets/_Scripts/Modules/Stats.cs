using UnityEngine;

public class Stats : Core
{
    private EntityStats _entityStats;

    public int currentHealth;

    private void Awake() 
    {
        currentHealth = _entityStats.maxHealth;
    }
}