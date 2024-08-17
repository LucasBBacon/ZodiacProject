using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager instance;

    public bool IsPaused { get; private set; }

    private void Awake() {
        if (instance == null)
            instance = this;
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        InputManager.PlayerInput.SwitchCurrentActionMap("UI");
    }

    public void UnpauseGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        InputManager.PlayerInput.SwitchCurrentActionMap("InGame");
    }
}
