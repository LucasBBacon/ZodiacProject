using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    [Header("Main Menu Objects")]
    [SerializeField] GameObject _loadingBarObj;
    [SerializeField] Image _loadingBar;
    [SerializeField] GameObject[] _objectsToHide;

    [Header("Scenes to Load")]
    [SerializeField] SceneField _persistentGameplay;
    [SerializeField] SceneField _levelScene;

    [Header("Menu Objects")]
    [SerializeField] private GameObject _mainMenuCanvasGO;
    [SerializeField] private GameObject _settingsMenuCanvasGO;
    // [SerializeField] private GameObject _keyboardSettingsMenuGO;
    // [SerializeField] private GameObject _gamepadSettingsMenuGO;
    // [SerializeField] private GameObject _soundMenuCanvasGO;

    [Space(20)]
    //[Header("Player Scripts to Deactivate on Pause")]
    //[SerializeField] private Player _player;
    //[SerializeField] private PlayerAttack _playerAttack;

    [Header("First Selected Options")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;
    // [SerializeField] private GameObject _keyboardSettingsMenuFirst;
    // [SerializeField] private GameObject _gamepadSettingsMenuFirst;
    // [SerializeField] private GameObject _soundMenuFirst;

    //public bool isPaused = false;
    private string _previousPage;

    List<AsyncOperation> _scenesToLoad = new List<AsyncOperation>();

    private void Start() 
    {
        _loadingBarObj.SetActive(false);
        _mainMenuCanvasGO.SetActive(true);
        _settingsMenuCanvasGO.SetActive(false);
        // _keyboardSettingsMenuGO.SetActive(false);
        // _gamepadSettingsMenuGO.SetActive(false);
        // _soundMenuCanvasGO.SetActive(false);
    }

    private void Update() 
    {
        if(InputManager.instance.ReturnPageInput)
        {
            switch(_previousPage)
            {
            case "main":
                OpenMainMenu();
                break;
            case "settings":
                OpenSettingsMenuHandle();
                break;
            default:
                OpenMainMenu();
                break;
            }
        }
        
    }

    #region Game Initiation

    public void StartGame()
    {
        HideMenu();

        _loadingBarObj.SetActive(true);

        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_persistentGameplay));
        _scenesToLoad.Add(SceneManager.LoadSceneAsync(_levelScene, LoadSceneMode.Additive));

        StartCoroutine(ProgressLoadingBar());
    }

    void HideMenu()
    {
        for (int i = 0; i < _objectsToHide.Length; i++)
        {
            _objectsToHide[i].SetActive(false);
        }
    }

    IEnumerator ProgressLoadingBar()
    {
        float loadProgress = 0f;
        for (int i = 0; i < _scenesToLoad.Count; i++)
        {
            while (!_scenesToLoad[i].isDone)
            {
                loadProgress += _scenesToLoad[i].progress;

                _loadingBar.fillAmount = loadProgress / _scenesToLoad.Count;

                yield return null;
            }
        }
    }

    #endregion


    #region Canvas Activations

    private void OpenMainMenu()
    {
        _previousPage = "game";
        _mainMenuCanvasGO.SetActive(true);
        _settingsMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_mainMenuFirst);
    }

    private void OpenSettingsMenuHandle()
    {
        _previousPage = "main";
        _settingsMenuCanvasGO.SetActive(true);
        _mainMenuCanvasGO.SetActive(false);
        // _keyboardSettingsMenuGO.SetActive(false);
        // _gamepadSettingsMenuGO.SetActive(false);
        // _soundMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }

    // private void OpenKeyboardControls()
    // {
    //     _previousPage = "settings";
    //     _keyboardSettingsMenuGO.SetActive(true);
    //     _settingsMenuCanvasGO.SetActive(false);

    //     EventSystem.current.SetSelectedGameObject(_keyboardSettingsMenuFirst);
    // }

    // private void OpenGamepadControls()
    // {
    //     _previousPage = "settings";
    //     _gamepadSettingsMenuGO.SetActive(true);
    //     _settingsMenuCanvasGO.SetActive(false);

    //     EventSystem.current.SetSelectedGameObject(_gamepadSettingsMenuFirst);
    // }

    // private void OpenSoundSettings()
    // {
    //     _previousPage = "settings";
    //     _soundMenuCanvasGO.SetActive(true);
    //     _settingsMenuCanvasGO.SetActive(false);

    //     EventSystem.current.SetSelectedGameObject(_soundMenuFirst);
    // }



    private void CloseAllMenus()
    {
        _previousPage = null;
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        // _keyboardSettingsMenuGO.SetActive(false);
        // _gamepadSettingsMenuGO.SetActive(false);
        // _soundMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    #endregion

    #region Main Menu Button Actions

    public void OnSettingsPress()
    {
        OpenSettingsMenuHandle();
    }

    #endregion

    #region Settings Menu Button Actions

    // public void OnSettingsBackPress()
    // {
    //     OpenMainMenu();
    // }

    // public void OnKeyboardControlsPress()
    // {
    //     OpenKeyboardControls();
    // }

    // public void OnGamepadControlsPress()
    // {
    //     OpenGamepadControls();
    // }

    // public void OnSoundSettingsPress()
    // {
    //     OpenSoundSettings();
    // }

    #endregion

}
