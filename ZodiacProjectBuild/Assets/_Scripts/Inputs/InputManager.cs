using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    public static PlayerInput PlayerInput;
    public PlayerInput PlayerInputRumble => PlayerInput;

    #region Outputs Values

    public Vector2 MoveInput { get; private set; }

    public bool JumpJustPressed { get; private set; }
    public bool JumpBeingHeld { get; private set; }
    public bool JumpReleased { get; private set; }

    public bool InteractJustPressed { get; private set; }

    public bool DashInput { get; private set; }

    public bool AttackInput { get; private set; }
    public bool BlockInput { get; private set; }

    public bool[] AbilityUse { get; private set; } = new bool[3];
    public bool[] AbilityBeingHeld { get; private set; } = new bool[3];
    public bool[] AbilityReleased { get; private set; } = new bool[3];


    #endregion


    #region UI

    //public static Vector2 NavigationInput { get; private set; }
    public bool MenuOpen { get; private set; }
    public bool UIMenuClose { get; private set; }
    public bool ReturnPageInput { get; private set; }

    #endregion


    #region Input Actions

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _interactAction;
    private InputAction _dashAction;

    private InputAction _attackAction;

    private InputAction _blockAction;

    private InputAction _abilityOneAction;
    private InputAction _abilityTwoAction;
    private InputAction _abilityThreeAction;

    private InputAction _menuOpenAction;

    private InputAction _UIMenuCloseAction;

    private InputAction _navigationAction;
    private InputAction _returnPageAction;

    #endregion

    private void Awake()
    {
        if (instance == null) instance = this;

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
        _interactAction = PlayerInput.actions["Interact"];
        _dashAction = PlayerInput.actions["Dash"];
        _attackAction = PlayerInput.actions["Attack"];
        _blockAction = PlayerInput.actions["Block"];
        _abilityOneAction = PlayerInput.actions["Ability1"];
        _abilityTwoAction = PlayerInput.actions["Ability2"];
        //_abilityThreeAction = PlayerInput.actions["Ability3"];

        _menuOpenAction = PlayerInput.actions["MenuOPEN"];

        _UIMenuCloseAction = PlayerInput.actions["MenuCLOSE"];

        //_navigationAction = PlayerInput.actions["Navigate"];
        //_returnPageAction = PlayerInput.actions["Return"];
    }

    private void UpdateInputs()
    {
        MoveInput = _moveAction.ReadValue<Vector2>().normalized;

        JumpJustPressed = _jumpAction.WasPressedThisFrame();
        JumpBeingHeld = _jumpAction.IsPressed();
        JumpReleased = _jumpAction.WasReleasedThisFrame();

        InteractJustPressed = _interactAction.WasPressedThisFrame();

        DashInput = _dashAction.WasPressedThisFrame();

        AttackInput = _attackAction.WasPressedThisFrame();
        BlockInput = _blockAction.WasPressedThisFrame();

        AbilityUse[(int)AbilityInputs.First] = _abilityOneAction.WasPressedThisFrame();
        AbilityBeingHeld[(int)AbilityInputs.First] = _abilityOneAction.IsPressed();
        AbilityReleased[(int)AbilityInputs.First] = _abilityOneAction.WasReleasedThisFrame();

        AbilityUse[(int)AbilityInputs.Second] = _abilityTwoAction.WasPressedThisFrame();
        AbilityBeingHeld[(int)AbilityInputs.Second] = _abilityTwoAction.IsPressed();
        AbilityReleased[(int)AbilityInputs.Second] = _abilityTwoAction.WasReleasedThisFrame();

        //AbilityUse[(int)AbilityInputs.Third] = _abilityThreeAction.WasPressedThisFrame();
        //AbilityBeingHeld[(int)AbilityInputs.Third] = _abilityThreeAction.IsPressed();
        //AbilityReleased[(int)AbilityInputs.Third] = _abilityThreeAction.WasReleasedThisFrame();

        MenuOpen = _menuOpenAction.WasPressedThisFrame();
        UIMenuClose = _UIMenuCloseAction.WasPressedThisFrame();

        //NavigationInput = _navigationAction.ReadValue<Vector2>();
        //ReturnPageInput = _returnPageAction.WasPressedThisFrame();
    }

    // public void UseAttackInput() => AttackInput = false;
}

public enum AbilityInputs
{
    First,
    Second,
    Third
}