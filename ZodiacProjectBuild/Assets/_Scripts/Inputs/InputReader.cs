using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input/Input Reader", fileName = "Input Reader")]
public class InputReader : ScriptableObject
{
    [SerializeField] private InputActionAsset _asset;

    #region Input Actions

    private InputAction _moveAction;

    private InputAction _jumpAction;
    private InputAction _dashAction;
    private InputAction _grabAction;

    private InputAction _attackAction;

    #endregion

    private void OnEnable()
    {
        SetupInputActions();
    }

    private void SetupInputActions()
    {
        _moveAction = _asset.FindAction("Move");
        _jumpAction = _asset.FindAction("Jump");
        _dashAction = _asset.FindAction("Dash");
        _grabAction = _asset.FindAction("Grab");
        _attackAction = _asset.FindAction("Attack");
    }
}