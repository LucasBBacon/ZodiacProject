using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager instance;

    public bool IsInteracting { get; private set; }

    private void Awake() 
    {
        if(instance == null)
        {
            instance = this;
        }   
    }

    public void Interact()
    {
        IsInteracting = true;
        UserInput.PlayerInput.SwitchCurrentActionMap("InteractiveMovementObjects");
    }

    public void StopInteract()
    {
        IsInteracting = false;
        UserInput.PlayerInput.SwitchCurrentActionMap("InGame");
    }
}