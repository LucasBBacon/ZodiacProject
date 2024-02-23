using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public bool IsPaused { get; private set; }

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public void PauseGame()
    {
        IsPaused  = true;
        UserInput.PlayerInput.SwitchCurrentActionMap("UI");
        Time.timeScale = 0f;

        // UserInput.PlayerInput.actions.FindActionMap("InGame").Disable();
        // UserInput.PlayerInput.actions.FindActionMap("UI").Enable();
        
    }

    public void UnpauseGame()
    {
        IsPaused = false;
        UserInput.PlayerInput.SwitchCurrentActionMap("InGame");
        Time.timeScale = 1f;

        // UserInput.PlayerInput.actions.FindActionMap("UI").Disable();
        // UserInput.PlayerInput.actions.FindActionMap("InGame").Enable();
    }
}