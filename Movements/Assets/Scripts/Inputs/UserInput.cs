using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UserInput : MonoBehaviour
{
    public static UserInput instance;

    public Vector2  MoveInput       { get; private set; }
    public Vector2  LookInput       { get; private set; }
    public bool     JumpJustPressed { get; private set; }
    public bool     JumpBeingHeld   { get; private set; }
    public bool     JumpReleased    { get; private set; }
    public bool     DashInput       { get; private set; }
    public bool     MenuOpenInput   { get; private set; }
    public bool     SwitchInput     { get; private set; }
    public bool     LevelInput      { get; private set; }
    public bool     InteractInput   { get; private set; }
    //public bool     AttackInput         { get; private set; }
    
    public Vector2  InteractMoveInput   { get; private set; }
    public bool     StopInteractInput   { get; private set; }

    public Vector2  MousePosition       { get; private set; }
    public bool     UIMenuCloseInput    { get; private set; }
    public bool     ReturnPageInput     { get; private set; }


    public static PlayerInput PlayerInput;

    private InputAction _moveAction;
    private InputAction _lookAction;
    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _menuOpenAction;
    private InputAction _switchAction;
    private InputAction _levelAction;
    private InputAction _interactAction;
    //private InputAction _attackAction;

    private InputAction _moveInteractAction;
    private InputAction _stopInteractAction;

    private InputAction _UIMouseMovement;
    private InputAction _UIMenuCloseAction;
    private InputAction _returnPageAction;

    private void Awake() 
    {
        if(instance == null) instance = this;
        else Destroy(instance);

        PlayerInput = GetComponent<PlayerInput>();

        SetupInputActions();
    }

    private void Update() 
    {
        UpdateInputs();    
    }

    private void SetupInputActions()
    {
        _moveAction     =   PlayerInput.actions["Move"];
        _lookAction     =   PlayerInput.actions["Look"];
        _jumpAction     =   PlayerInput.actions["Jump"];
        _dashAction     =   PlayerInput.actions["Dash"];
        _menuOpenAction =   PlayerInput.actions["MenuOpen"];
        _switchAction   =   PlayerInput.actions["Switch"];
        _levelAction    =   PlayerInput.actions["SwitchLevel"];
        _interactAction =   PlayerInput.actions["Interact"];
        //_attackAction           =   PlayerInput.actions["Attack"];

        _moveInteractAction = PlayerInput.actions["InteractMove"];
        _stopInteractAction = PlayerInput.actions["InteractStop"];

        _UIMouseMovement    = PlayerInput.actions["Point"];
        _UIMenuCloseAction  = PlayerInput.actions["MenuClose"];
        _returnPageAction   = PlayerInput.actions["Return"];
    }

    private void UpdateInputs()
    {
        MoveInput       =   _moveAction.ReadValue<Vector2>();
        LookInput       =   _lookAction.ReadValue<Vector2>();

        JumpJustPressed =   _jumpAction.WasPressedThisFrame();
        JumpBeingHeld   =   _jumpAction.IsPressed();
        JumpReleased    =   _jumpAction.WasReleasedThisFrame();

        DashInput       =   _dashAction.WasPressedThisFrame();
        MenuOpenInput   =   _menuOpenAction.WasPressedThisFrame();

        SwitchInput     =   _switchAction.WasPressedThisFrame();
        LevelInput      =   _levelAction.WasPressedThisFrame();

        InteractInput   =   _interactAction.WasPressedThisFrame();
        //AttackInput         =   _attackAction.WasPressedThisFrame();

        InteractMoveInput   = _moveInteractAction.ReadValue<Vector2>();
        StopInteractInput   = _stopInteractAction.WasPressedThisFrame();

        MousePosition       = _UIMouseMovement.ReadValue<Vector2>();
        UIMenuCloseInput    = _UIMenuCloseAction.WasPressedThisFrame();
        ReturnPageInput     = _returnPageAction.WasPressedThisFrame();
    }
}
