using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    Transform particleContainer;
    Movement movement;

    private void Awake()
    {
        particleContainer = GameObject.FindGameObjectWithTag("ParticleContainer").transform;    
    }

    private void Start()
    {
        movement = GetComponentInChildren<Movement>();
        Debug.Log(movement);  
    }

    public void StartEffect
        (
            GameObject effectPrefab,
            Vector2 position,
            Quaternion rotation,
            float timeAlive
        )
    {
        GameObject obj = Instantiate(effectPrefab, position, rotation);
        Destroy(obj, timeAlive);
    }

    public GameObject StartParticles
        (
            GameObject particlePrefab,
            Vector2 position,
            Quaternion rotation
        )
        => Instantiate
            (
                particlePrefab,
                position,
                rotation,
                particleContainer
            );

    public GameObject StartParticles(GameObject particlePrefab)
    => StartParticles
        (
            particlePrefab,
            transform.position,
            Quaternion.identity
        );

    public GameObject StartWithRandomRotation(GameObject particlePrefab)
    => StartParticles
        (
            particlePrefab,
            transform.position,
            Quaternion.Euler(0f, 0f, Random.Range(0f, 360f))
        );

    public GameObject SpawnParticlesRelative
        (
            GameObject particlePrefab,
            Vector2 offset,
            Quaternion rotation
        )
        => StartParticles
            (
                particlePrefab,
                FindRelativePoint(offset),
                rotation
            );

    private Vector2 FindRelativePoint(Vector2 offset)
    => movement.FindRelativePoint(offset);
}