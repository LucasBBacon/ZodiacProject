using UnityEngine;

public class WindTrigger : MonoBehaviour
{
    [SerializeField] GameObject windObject;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            windObject.SetActive(true);
            Destroy(gameObject);
        }
    }
}