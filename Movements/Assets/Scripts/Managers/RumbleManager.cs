using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.iOS;

public class RumbleManager : MonoBehaviour
{
    public static RumbleManager instance;

    private Gamepad pad;

    private Coroutine stopRumbleAfterTimeCoroutine;

    private string currentControlScheme;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    private void Start() 
    {
        UserInput.PlayerInput.onControlsChanged += SwitchControls;
        
    }

    public void RumblePulse(float lowFreq, float highFreq, float duration)
    {
        // is the current control scheme gamepad
        if(currentControlScheme == "Gamepad")
        {
            // get reference to gamepad
            pad = Gamepad.current;

            // check for reference of gamepad
            if(pad != null)
            {
                // start rumble
                pad.SetMotorSpeeds(lowFreq, highFreq);

                // stop rumble after duration
                stopRumbleAfterTimeCoroutine = StartCoroutine(StopRumble(duration, pad));
            }
        }
    }

    private IEnumerator StopRumble(float duration, Gamepad pad)
    {
        float elapsedTime = 0f;

        while(elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // once duration is over
        pad.SetMotorSpeeds(0f, 0f);
    }

    private void SwitchControls(PlayerInput input)
    {
        Debug.Log("Device is now: " + input.currentControlScheme);
        currentControlScheme = input.currentControlScheme;
    }

    private void OnDisable() 
    {
        UserInput.PlayerInput.onControlsChanged -= SwitchControls;
    }
}
