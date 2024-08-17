using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerInteractBase : MonoBehaviour, IInteractable
{
    public GameObject Player { get; set; }
    public bool CanInteract { get; set; }

    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        if (CanInteract)
        {
            if (InputManager.instance.InteractJustPressed)
            {
                Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == Player)
            CanInteract = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == Player)
            CanInteract = false;
    }

    public virtual void Interact() { }
}

public class SceneLoadTrigger : MonoBehaviour
{
    [SerializeField] SceneField[] _scenesToLoad;
    [SerializeField] SceneField[] _scenesToUnload;

    private GameObject _player;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == _player)
        {
            // load and unload the needed scenes
            LoadScenes();
            UnloadScenes();
        }
    }

    void LoadScenes()
    {
        for (int i = 0; i < _scenesToLoad.Length; i++)
        {
            bool isSceneLoaded = false;
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == _scenesToLoad[i].SceneName)
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            if (!isSceneLoaded)
            {
                SceneManager.LoadSceneAsync(_scenesToLoad[i], LoadSceneMode.Additive);    
            }
        }

        
    }

    void UnloadScenes()
    {
        for (int i = 0; i < _scenesToLoad.Length; i++)
        {
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == _scenesToLoad[i].SceneName)
                {
                    SceneManager.UnloadSceneAsync(_scenesToUnload[i]);
                }
            }
        }
    }
}