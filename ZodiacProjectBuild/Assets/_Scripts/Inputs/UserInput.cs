using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UserInput : MonoBehaviour
{
    public static UserInput     instance;
    public static PlayerInput   playerInput;

    #region Outputs Values

    public Vector2  MoveInput       { get; private set; }
    public bool     JumpJustPressed { get; private set; }
    public bool     JumpBeingHeld   { get; private set; }
    public bool     JumpReleased    { get; private set; }

    #endregion

    #region Input Actions

    private InputAction _moveAction;
    private InputAction _jumpAction;

    #endregion

    private void Awake()
    {
        if(instance == null)
            instance = this;    
        else
            Destroy(instance);

        playerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }
    
    private void Update()
    {
        UpdateInputs();
    }

    private void SetupInputActions()
    {
        _moveAction = playerInput.actions["Move"];
        _jumpAction = playerInput.actions["Jump"];
    }

    private void UpdateInputs()
    {
        MoveInput       = _moveAction.ReadValue<Vector2>();

        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        JumpBeingHeld   = _jumpAction.IsPressed();
        JumpReleased    = _jumpAction.WasReleasedThisFrame();
    }
}
