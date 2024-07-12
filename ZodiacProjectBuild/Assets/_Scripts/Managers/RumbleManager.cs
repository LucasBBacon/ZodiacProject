using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager Instance { get; private set;}
    Gamepad pad;
    string currentControlScheme;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

    }

    private void Start()
    {
        InputManager.PlayerInput.onControlsChanged += SwitchControls;
    }

    private void OnDisable()
    {
        InputManager.PlayerInput.onControlsChanged -= SwitchControls;
    }

    public void RumblePulse(float lowFrequency, float highFrequency, float duration)
    {
        if (currentControlScheme == "Gamepad")
        {
            // gets reference to gamepad
            pad = Gamepad.current;

            // if there is a gamepad
            if (pad != null)
            {
                // start rumble
                pad.SetMotorSpeeds(lowFrequency, highFrequency);

                // stop rumble after a certain amount of time
                StartCoroutine(StopRumble(pad, duration));
            }
        }

        
    }

    IEnumerator StopRumble(Gamepad pad, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // once duration is over
        pad.SetMotorSpeeds(0f, 0f);
    }

    void SwitchControls(PlayerInput input)
    {
        Debug.Log("device is now: " + input.currentControlScheme);
        currentControlScheme = input.currentControlScheme;
    }
}
