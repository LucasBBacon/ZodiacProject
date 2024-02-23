using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    [Header("Menu Objects")]
    [SerializeField] private GameObject _mainMenuCanvasGO;
    [SerializeField] private GameObject _settingsMenuCanvasGO;
    [SerializeField] private GameObject _keyboardSettingsMenuGO;
    [SerializeField] private GameObject _gamepadSettingsMenuGO;
    [SerializeField] private GameObject _soundMenuCanvasGO;

    [Space(5)]

    [SerializeField] private GameObject _variableMenuCavasGO;
    [SerializeField] private GameObject _gravityVariableMenuGO;
    [SerializeField] private GameObject _runVariableMenuGO;
    [SerializeField] private GameObject _jumpVariableMenuGO;
    [SerializeField] private GameObject _wallJumpVariableMenuGO;
    [SerializeField] private GameObject _slideVariableMenuGO;
    [SerializeField] private GameObject _dashVariableMenuGO;

    [Space(20)]
    //[Header("Player Scripts to Deactivate on Pause")]
    //[SerializeField] private Player _player;
    //[SerializeField] private PlayerAttack _playerAttack;

    [Header("First Selected Options")]
    [SerializeField] private GameObject _mainMenuFirst;
    [SerializeField] private GameObject _settingsMenuFirst;
    [SerializeField] private GameObject _keyboardSettingsMenuFirst;
    [SerializeField] private GameObject _gamepadSettingsMenuFirst;
    [SerializeField] private GameObject _soundMenuFirst;
    [SerializeField] private GameObject _variableMenuFirst;
    [SerializeField] private GameObject _gravityMenuFirst;
    [SerializeField] private GameObject _runMenuFirst;
    [SerializeField] private GameObject _jumpMenuFirst;
    [SerializeField] private GameObject _wallJumpMenuFirst;
    [SerializeField] private GameObject _slideMenuFirst;
    [SerializeField] private GameObject _dashMenuFirst;

    //public bool isPaused = false;
    private string _previousPage;

    private void Start() 
    {
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardSettingsMenuGO.SetActive(false);
        _gamepadSettingsMenuGO.SetActive(false);
        _soundMenuCanvasGO.SetActive(false);
        
        _variableMenuCavasGO.SetActive(false);
        _gravityVariableMenuGO.SetActive(false);
        _runVariableMenuGO.SetActive(false);
        _jumpVariableMenuGO.SetActive(false);
        _wallJumpVariableMenuGO.SetActive(false);
        _slideVariableMenuGO.SetActive(false);
        _dashVariableMenuGO.SetActive(false);
    }

    private void Update() 
    {
        if(UserInput.instance.MenuOpenInput)
        {
            if(!PauseManager.instance.IsPaused)
            {
                Pause();
            }
        }
        else if(UserInput.instance.UIMenuCloseInput)
        {
            if(PauseManager.instance.IsPaused)
            {
                Unpause();
            }
        }

        if(UserInput.instance.ReturnPageInput)
        {
            switch(_previousPage)
            {
            case "game":
                Unpause();
                break;
            case "main":
                OpenMainMenu();
                break;
            case "settings":
                OpenSettingsMenuHandle();
                break;
            case "variables":
                OpenVariableMenu();
                break;
            default:
                Unpause();
                break;
            }
        }
        
    }

    #region Pause/Unpause Functions

    public void Pause()
    {
        PauseManager.instance.PauseGame();

        OpenMainMenu();
    }

    public void Unpause()
    {
        PauseManager.instance.UnpauseGame();

        CloseAllMenus();
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
        _keyboardSettingsMenuGO.SetActive(false);
        _gamepadSettingsMenuGO.SetActive(false);
        _soundMenuCanvasGO.SetActive(false);
        _variableMenuCavasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_settingsMenuFirst);
    }

    private void OpenKeyboardControls()
    {
        _previousPage = "settings";
        _keyboardSettingsMenuGO.SetActive(true);
        _settingsMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_keyboardSettingsMenuFirst);
    }

    private void OpenGamepadControls()
    {
        _previousPage = "settings";
        _gamepadSettingsMenuGO.SetActive(true);
        _settingsMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_gamepadSettingsMenuFirst);
    }

    private void OpenSoundSettings()
    {
        _previousPage = "settings";
        _soundMenuCanvasGO.SetActive(true);
        _settingsMenuCanvasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_soundMenuFirst);
    }



    #region Variables Menu

    private void OpenVariableMenu()
    {
        _previousPage = "settings";
        _variableMenuCavasGO.SetActive(true);
        _settingsMenuCanvasGO.SetActive(false);
        _gravityVariableMenuGO.SetActive(false);
        _runVariableMenuGO.SetActive(false);
        _jumpVariableMenuGO.SetActive(false);
        _wallJumpVariableMenuGO.SetActive(false);
        _slideVariableMenuGO.SetActive(false);
        _dashVariableMenuGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_variableMenuFirst);
    }

    private void OpenGravityMenu()
    {
        _previousPage = "variables";
        _gravityVariableMenuGO.SetActive(true);
        _variableMenuCavasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_gravityMenuFirst);
    }

    private void OpenRunMenu()
    {
        _previousPage = "variables";
        _runVariableMenuGO.SetActive(true);
        _variableMenuCavasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_runMenuFirst);
    }

    private void OpenJumpMenu()
    {
        _previousPage = "variables";
        _jumpVariableMenuGO.SetActive(true);
        _variableMenuCavasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_jumpMenuFirst);
    }

    private void OpenWallJumpMenu()
    {
        _previousPage = "variables";
        _wallJumpVariableMenuGO.SetActive(true);
        _variableMenuCavasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_wallJumpMenuFirst);
    }

    private void OpenSlideMenu()
    {
        _previousPage = "variables";
        _slideVariableMenuGO.SetActive(true);
        _variableMenuCavasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_slideMenuFirst);
    }

    private void OpenDashMenu()
    {
        _previousPage = "variables";
        _dashVariableMenuGO.SetActive(true);
        _variableMenuCavasGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(_dashMenuFirst);
    }

    #endregion



    private void CloseAllMenus()
    {
        _previousPage = null;
        _mainMenuCanvasGO.SetActive(false);
        _settingsMenuCanvasGO.SetActive(false);
        _keyboardSettingsMenuGO.SetActive(false);
        _gamepadSettingsMenuGO.SetActive(false);
        _soundMenuCanvasGO.SetActive(false);
        
        _variableMenuCavasGO.SetActive(false);
        _gravityVariableMenuGO.SetActive(false);
        _runVariableMenuGO.SetActive(false);
        _jumpVariableMenuGO.SetActive(false);
        _wallJumpVariableMenuGO.SetActive(false);
        _slideVariableMenuGO.SetActive(false);
        _dashVariableMenuGO.SetActive(false);

        EventSystem.current.SetSelectedGameObject(null);
    }

    #endregion

    #region Main Menu Button Actions

    public void OnSettingsPress()
    {
        OpenSettingsMenuHandle();
    }

    public void OnResumePress()
    {
        Unpause();
    }

    #endregion

    #region Settings Menu Button Actions

    public void OnSettingsBackPress()
    {
        OpenMainMenu();
    }

    public void OnKeyboardControlsPress()
    {
        OpenKeyboardControls();
    }

    public void OnGamepadControlsPress()
    {
        OpenGamepadControls();
    }

    public void OnSoundSettingsPress()
    {
        OpenSoundSettings();
    }

    public void OnVariableMenuPress()
    {
        OpenVariableMenu();
    }

    #endregion

    #region Varible Menu Button Actions

    public void OnGravityMenuPress()
    {
        OpenGravityMenu();
    }

    public void OnRunMenuPress()
    {
        OpenRunMenu();
    }

    public void OnJumpMenuPress()
    {
        OpenJumpMenu();
    }

    public void OnWallJumpMenuPress()
    {
        OpenWallJumpMenu();
    }

    public void OnSlideMenuPress()
    {
        OpenSlideMenu();
    }

    public void OnDashMenuPress()
    {
        OpenDashMenu();
    }

    #endregion

}
