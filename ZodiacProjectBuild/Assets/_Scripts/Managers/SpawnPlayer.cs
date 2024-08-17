using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] SceneField _persistentGameplay;

    private void Awake()
    {
        SceneManager.LoadSceneAsync(_persistentGameplay, LoadSceneMode.Additive);
    }
}
