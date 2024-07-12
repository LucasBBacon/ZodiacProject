using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //public static UserInput Instance { get; set; }
    public static PlayerInput PlayerInput;

    #region Outputs Values

    public static Vector2 MoveInput { get; private set; }

    public static bool JumpJustPressed { get; private set; }
    public static bool JumpBeingHeld { get; private set; }
    public static bool JumpReleased { get; private set; }

    public static bool RunIsHeld { get; private set; }

    public static bool DashInput { get; private set; }

    public static bool GrabInput { get; private set; }
    public static bool GrabBeingHeld { get; private set; }
    public static bool GrabReleased { get; private set; }

    public static bool AttackInput { get; private set; }

    public static bool AbilityOne { get; private set; }

    #endregion

    #region Input Actions

    private InputAction _moveAction;

    private InputAction _jumpAction;
    private InputAction _runAction;
    private InputAction _dashAction;
    private InputAction _grabAction;

    private InputAction _attackAction;

    private InputAction _abilityOne;

    #endregion

    private void Awake()
    {
        // if (Instance == null)
        // {
        //     Instance = this;
        // }

        PlayerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }
    
    private void Update()
    {
        UpdateInputs();
    }

    private void SetupInputActions()
    {
        _moveAction = PlayerInput.actions["Move"];
        _jumpAction = PlayerInput.actions["Jump"];
        _runAction = PlayerInput.actions["Run"];
        _dashAction = PlayerInput.actions["Dash"];
        _grabAction = PlayerInput.actions["Grab"];
        _attackAction = PlayerInput.actions["Attack"];
        _abilityOne = PlayerInput.actions["Ability1"];
    }

    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>();

        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        JumpBeingHeld = _jumpAction.IsPressed();
        JumpReleased = _jumpAction.WasReleasedThisFrame();

        RunIsHeld = _runAction.IsPressed();

        DashInput = _dashAction.WasPressedThisFrame();

        GrabInput = _grabAction.WasPressedThisFrame();
        GrabBeingHeld = _grabAction.IsPressed();
        GrabReleased = _grabAction.WasReleasedThisFrame();

        AttackInput = _attackAction.WasPressedThisFrame();

        AbilityOne = _abilityOne.WasPressedThisFrame();
    }
}
