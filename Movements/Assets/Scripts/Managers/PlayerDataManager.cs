using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Unity.VisualScripting;
using System;
using TMPro;

public class PlayerDataManager : MonoBehaviour
{   
    private PlayerData  _playerData;
    private Player      _player;


    [Header("Variable to Change:")]
    [SerializeField] private Variables _variables = new Variables();
    
    [Space(5)]

    [Header("Display:")]
    [SerializeField] private TMP_Text   _displayText;

    [Space(20)]

    [Header("Field:")]
    [SerializeField] private GameObject _inputField;
    [SerializeField] public Toggle      _toggleButton;
    [SerializeField] public Slider      _sliderField;


    #region Values from Fields

    private string  _passedInVariable;
    private float   _sliderValue;
    private bool    _booleanCheck;
    private string  _jumpForceX;
    private string  _jumpForceY;

    #endregion

    #region Update Methods

    private void Awake() 
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        
        _playerData = _player.playerData;

        DisplayVariable();
        DisableVariableSelection();
    }

    private void Start() 
    {
        GameEvents.instance.onPlayerSwitch += OnPlayerSwitchUpdate;
        Debug.Log(GameManager.instance.IsChangeable);
    }
    
    private void Update() 
    {
        if(PauseManager.instance.IsPaused)
        {
            DisableVariableSelection();
        }
    }

    public void OnPlayerSwitchUpdate()
    {
        _playerData = _player.playerData;
        DisplayVariable();
        DisableVariableSelection();
    }

    #endregion


    #region Value Input Methods

    /// <summary>
    /// Called when a value is passed in through a <c>TMP Input Field</c>.
    /// </summary>
    public void OnEndEditString()
    {
        if(gameObject.CompareTag("SliderField"))
            _passedInVariable = _inputField.GetComponentInChildren<TMP_InputField>().text;
        else
            _passedInVariable = _inputField.GetComponent<TMP_InputField>().text;
        //Debug.Log(_passedInVariable);

        EditVariable();
        DisplayVariable();
    }

    /// <summary>
    /// Called when a value is passed in through a <c>TMP Button</c>.
    /// </summary>
    public void OnButtonToggle()
    {
        _booleanCheck = _toggleButton.isOn;

        EditVariable();
        DisplayVariable();
    }

    /// <summary>
    /// Called when a value is passed in through a <c>Slider</c>.
    /// </summary>
    public void OnSliderChange()
    {
        _sliderValue = _sliderField.value;

        EditVariable();
        DisplayVariable();
    }

    #endregion


    #region Variable Set and Display Methods

    /// <summary>
    /// Sets the corresponding <c>Enum</c> variables to the correct passed in value of the instance. Calculates the rest of the variables.
    /// </summary>
    public void EditVariable()
    {
        switch(_variables)
        {
            case Variables.FallGravityMultiplier:
                _playerData.fallGravityMult          = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.MaxFallSpeed:
                _playerData.maxFallSpeed             = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.FastFallGravityMultiplier:
                _playerData.fastFallGravityMult      = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.MaxFastFallSpeed:
                _playerData.runMaxSpeed              = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.RunMaxSpeed:
                _playerData.runAcceleration          = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.RunAcceleration:
                _playerData.runDecceleration         = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.RunDecceleration:
                _playerData.runDecceleration         = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.AccelerationInAir:
                _playerData.accelInAir               = SetFloatWithRange(_sliderValue, 0f, 1f);
                // typeCheck =2 ;
                break;
            case Variables.DeccelerationInAir:
                _playerData.deccelInAir              = SetFloatWithRange(_sliderValue, 0f, 1f);
                // typeCheck =2 ;
                break;
            case Variables.DoConserveMomentum:
                _playerData.doConserveMomentum       = _booleanCheck;
                // typeCheck = 1;
                break;
            case Variables.JumpHeight:
                _playerData.jumpHeight               = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.JumpTimeToApex:
                _playerData.jumpTimeToApex           = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.JumpCutGravityMultiplier:
                _playerData.jumpCutGravityMult       = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.JumpHangGravityMultiplier:
                _playerData.jumpHangGravityMult      = SetFloatWithRange(_sliderValue, 0f, 1f);
                // typeCheck =2 ;
                break;
            case Variables.JumpHangTimeThreshold:
                _playerData.jumpHangTimeThreshold    = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.JumpHangAccelerationMultiplier:
                _playerData.jumpHangAccelerationMult = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.JumpHangMaxSpeedMultiplier:
                _playerData.jumpHangMaxSpeedMult     = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.WallJumpForceX:
                _playerData.wallJumpForce.x          = CheckString(_jumpForceX);
                // typeCheck = 0;
                break;
            case Variables.WallJumpForceY:
                _playerData.wallJumpForce.y          = CheckString(_jumpForceY);
                // typeCheck = 0;
                break;
            case Variables.WallJumpRunLerp:
                _playerData.wallJumpRunLerp          = SetFloatWithRange(_sliderValue, 0f, 1f);
                // typeCheck =2 ;
                break;
            case Variables.WallJumpTime:
                _playerData.wallJumpTime             = SetFloatWithRange(_sliderValue, 0f, 1f);
                // typeCheck =2 ;
                break;
            case Variables.DoTurnOnWallJump:
                _playerData.doTurnOnWallJump         = _booleanCheck;
                // typeCheck = 1;
                break;
            case Variables.SlideSpeed:
                _playerData.slideSpeed               = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.SlideAcceleration:
                _playerData.slideAccel               = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.DashAmount:
                _playerData.dashAmount               = CheckInt(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.DashSpeed:
                _playerData.dashSpeed                = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.DashSleepTime:
                _playerData.dashSleepTime            = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.DashAttackTime:
                _playerData.dashAttackTime           = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.DashEndTime:
                _playerData.dashEndTime              = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.DashEndSpeedX:
                _playerData.dashEndSpeed.x           = CheckString(_jumpForceX);
                // typeCheck = 0;
                break;
            case Variables.DashEndSpeedY:
                _playerData.dashEndSpeed.y           = CheckString(_jumpForceY);
                // typeCheck = 0;
                break;
            case Variables.DashEndRunLerp:
                _playerData.dashEndRunLerp           = SetFloatWithRange(_sliderValue, 0f, 1f);
                // typeCheck = 0;
                break;
            case Variables.DashRefillTime:
                _playerData.dashRefillTime           = CheckString(_passedInVariable);
                // typeCheck = 0;
                break;
            case Variables.DashInputBufferTime:
                _playerData.dashInputBufferTime      = SetFloatWithRange(_sliderValue, 0f, 1f);
                // typeCheck =2 ;
                break;
            case Variables.CoyoteTime:
                _playerData.coyoteTime               = SetFloatWithRange(_sliderValue, 0f, 1f);
                // typeCheck =2 ;
                break;
            case Variables.JumpInputBufferTime:
                _playerData.jumpInputBufferTime      = SetFloatWithRange(_sliderValue, 0f, 1f);
                // typeCheck =2 ;
                break;
        }
        
        _playerData.Calculate();
        _player.playerData = _playerData; 
    }

    /// <summary>
    /// Displays the variables on the UI.
    /// </summary>
    private void DisplayVariable()
    {
        switch(_variables)
            {
                case Variables.FallGravityMultiplier:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.fallGravityMult.ToString();
                    _displayText.text                               = "Fall Gravity Multiplier";
                    break;
                case Variables.MaxFallSpeed:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.maxFallSpeed.ToString();
                    _displayText.text                               = "Max Fall Speed";
                    break;
                case Variables.FastFallGravityMultiplier:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.fastFallGravityMult.ToString();
                    _displayText.text                               = "Fast Fall Gravity Multiplier";
                    break;
                case Variables.MaxFastFallSpeed:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.maxFastFallSpeed.ToString();
                    _displayText.text                               = "Max Fast Fall Speed";
                    break;
                case Variables.RunMaxSpeed:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.runMaxSpeed.ToString();
                    _displayText.text                               = "Run Max Speed";
                    break;
                case Variables.RunAcceleration:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.runAcceleration.ToString();
                    _displayText.text                               = "Run Acceleration";
                    break;
                case Variables.RunDecceleration:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.runDecceleration.ToString();
                    _displayText.text                               = "Run Decceleration";
                    break;
                case Variables.AccelerationInAir:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.accelInAir.ToString();
                    _sliderField.GetComponent<Slider>().value       = _playerData.accelInAir;
                    _displayText.text                               = "Acceleration In Air";
                    break;
                case Variables.DeccelerationInAir:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.deccelInAir.ToString();
                    _sliderField.GetComponent<Slider>().value       = _playerData.deccelInAir;
                    _displayText.text                               = "Decceleration In Air";
                    break;
                case Variables.DoConserveMomentum:
                    _toggleButton.isOn                              = _booleanCheck;
                    _displayText.text                               = "Do Conserve Momentum";
                    break;
                case Variables.JumpHeight:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.jumpHeight.ToString();
                    _displayText.text                               = "Jump Height";
                    break;
                case Variables.JumpTimeToApex:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.jumpTimeToApex.ToString();
                    _displayText.text = "Jump Time To Apex";
                    break;
                case Variables.JumpCutGravityMultiplier:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.jumpCutGravityMult.ToString();
                    _displayText.text                               = "Jump Cut Gravity Multiplier";
                    break;
                case Variables.JumpHangGravityMultiplier:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.jumpHangGravityMult.ToString();
                    _sliderField.GetComponent<Slider>().value       = _playerData.jumpHangGravityMult;
                    _displayText.text                               = "Jump Hang Gravity Multiplier";
                    break;
                case Variables.JumpHangTimeThreshold:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.jumpHangTimeThreshold.ToString();
                    _displayText.text                               = "Jump Hang Time Threshold";
                    break;
                case Variables.JumpHangAccelerationMultiplier:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.jumpHangAccelerationMult.ToString();
                    _displayText.text                               = "Jump Hang Acceleration Multiplier";
                    break;
                case Variables.JumpHangMaxSpeedMultiplier:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.fallGravityMult.ToString();
                    _displayText.text                               = "Jump Hang Max Speed Multiplier";
                    break;
                case Variables.WallJumpForceX:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.wallJumpForce.x.ToString();
                    _displayText.text                               = "Wall Jump Force X";
                    break;
                case Variables.WallJumpForceY:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.wallJumpForce.y.ToString();
                    _displayText.text                               = "Wall Jump Force Y";
                    break;
                case Variables.WallJumpRunLerp:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.wallJumpRunLerp.ToString();
                    _sliderField.GetComponent<Slider>().value       = _playerData.wallJumpRunLerp;
                    _displayText.text                               = "Wall Jump Run Lerp";
                    break;
                case Variables.WallJumpTime:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.wallJumpTime.ToString();
                    _sliderField.GetComponent<Slider>().value       = _playerData.wallJumpTime;
                    _displayText.text                               = "Wall Jump Time";
                    break;
                case Variables.DoTurnOnWallJump:
                    _toggleButton.isOn                              = _booleanCheck;
                    _displayText.text                               = "Do Turn On Wall Jump";
                    break;
                case Variables.SlideSpeed:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.slideSpeed.ToString();
                    _displayText.text                               = "Slide Speed";
                    break;
                case Variables.SlideAcceleration:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.slideAccel.ToString();
                    _displayText.text                               = "Slide Acceleration";
                    break;
                case Variables.DashAmount:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashAmount.ToString();
                    _displayText.text                               = "Dash Amount";
                    break;
                case Variables.DashSpeed:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashSpeed.ToString();
                    _displayText.text                               = "Dash Speed";
                    break;
                case Variables.DashSleepTime:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashSleepTime.ToString();
                    _displayText.text                               = "Dash Sleep Time";
                    break;
                case Variables.DashAttackTime:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashAttackTime.ToString();
                    _displayText.text                               = "Dash Attack Time";
                    break;
                case Variables.DashEndTime:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashEndTime.ToString();
                    _displayText.text                               = "Dash End Time";
                    break;
                case Variables.DashEndSpeedX:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashEndSpeed.x.ToString();
                    _displayText.text                               = "Dash End Speed X";
                    break;
                case Variables.DashEndSpeedY:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashEndSpeed.y.ToString();
                    _displayText.text                               = "Dash End Speed Y";
                    break;
                case Variables.DashEndRunLerp:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashEndRunLerp.ToString();
                    _sliderField.GetComponent<Slider>().value       = _playerData.dashEndRunLerp;
                    _displayText.text                               = "Dash End Run Lerp";
                    break;
                case Variables.DashRefillTime:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashRefillTime.ToString();
                    _displayText.text                               = "Dash Refill Time";
                    break;
                case Variables.DashInputBufferTime:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.dashInputBufferTime.ToString();
                    _sliderField.GetComponent<Slider>().value       = _playerData.dashInputBufferTime;
                    _displayText.text                               = "Dash Input Buffer Time";
                    break;
                case Variables.CoyoteTime:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.coyoteTime.ToString();
                    _sliderField.GetComponent<Slider>().value       = _playerData.coyoteTime;
                    _displayText.text                               = "Coyote Time";
                    break;
                case Variables.JumpInputBufferTime:
                    _inputField.GetComponent<TMP_InputField>().text = _playerData.jumpInputBufferTime.ToString();
                    _sliderField.GetComponent<Slider>().value       = _playerData.jumpInputBufferTime;
                    _displayText.text                               = "Jump Input Buffer Time";
                    break;
            }  
    }

    #endregion


    #region Input Disable

    /// <summary>
    /// Enables and disables UI elements dependant on the current <c>PlayerData</c> scriptable object.
    /// </summary>
    private void DisableVariableSelection()
    {
        if(gameObject.CompareTag("FloatField"))
        {
            if(!GameManager.instance.IsChangeable)
            {
                _inputField.GetComponent<TMP_InputField>().interactable = false;
            }
            else if(GameManager.instance.IsChangeable)
            {
                _inputField.GetComponent<TMP_InputField>().interactable = true;
            }
        }

        else if(gameObject.CompareTag("ToggleButton"))
        {  
            if(!GameManager.instance.IsChangeable)
            {
                _toggleButton.interactable = false;
            }
            else if(GameManager.instance.IsChangeable)
            {
                _toggleButton.interactable = true;
            }
        }

        else if(gameObject.CompareTag("SliderField"))
        {
            
            if(!GameManager.instance.IsChangeable)
            {
                _sliderField.interactable = false;
                _inputField.GetComponent<TMP_InputField>().interactable = false;
            }
            else if(GameManager.instance.IsChangeable)
            {
                _sliderField.interactable = true;
                _inputField.GetComponent<TMP_InputField>().interactable = true;
            }
        }
    }

    #endregion


    #region Value Check Methods

    /// <summary>
    /// Checks if the passed in <c>string</c> can be converted into a <c>float</c> variable.
    /// </summary>
    /// <param name="value">The string value to be converted into a float.</param>
    /// <returns>Returns the float present in the string. Otherwise returns 0 if there is no float present.</returns>
    private float CheckString(string value)
    {
        if(float.TryParse(value, out float result)) 
        {
            Debug.Log(result);
            return result;
        }
        else
        {
            Debug.Log("Not a float");
            return 0f;
        }
    }

    /// <summary>
    /// Checks if the passed in <c>string</c> can be converted into an <c>integer</c> variable.
    /// </summary>
    /// <param name="value">The string value to be converted into an int.</param>
    /// <returns>Returns the integer present in the string. Otherwise returns 0 if there is no int present.</returns>
    private int CheckInt(string value)
    {
        if(int.TryParse(value, out int result)) return result;
        else
        {
            Debug.Log("Not an int");
            return 0;
        }
    }

    /// <summary>
    /// Sets the passed in float within the passed in boundary values.
    /// </summary>
    /// <param name="value">Value to be checked.</param>
    /// <param name="lowB">Lower boundary value.</param>
    /// <param name="highB">Higher boundary value.</param>
    /// <returns>Returns the passed in value if it is within the boundary range. Otherwise, returns the higher or lower boundary.</returns>
    private float SetFloatWithRange(float value, float lowB, float highB)
    {
        if(value < lowB || value > highB) 
        {
            Debug.Log("Outside Range");
            
            if(value < lowB)
                return lowB;
            
            else if(value > highB)
                return highB;

            else
                return value;
        }
        else return value;
    }

    #endregion
}

/// <summary>
/// Enumerator to select what <c>PlayerData</c> variable is being changed.
/// </summary>
public enum Variables
{
    FallGravityMultiplier,
    MaxFallSpeed,
    FastFallGravityMultiplier,
    MaxFastFallSpeed,
    RunMaxSpeed,
    RunAcceleration,
    RunDecceleration,
    AccelerationInAir,
    DeccelerationInAir,
    DoConserveMomentum,
    JumpHeight,
    JumpTimeToApex,
    JumpCutGravityMultiplier,
    JumpHangGravityMultiplier,
    JumpHangTimeThreshold,
    JumpHangAccelerationMultiplier,
    JumpHangMaxSpeedMultiplier,
    WallJumpForceX,
    WallJumpForceY,
    WallJumpRunLerp,
    WallJumpTime,
    DoTurnOnWallJump,
    SlideSpeed,
    SlideAcceleration,
    DashAmount,
    DashSpeed,
    DashSleepTime,
    DashAttackTime,
    DashEndTime,
    DashEndSpeedX,
    DashEndSpeedY,
    DashEndRunLerp,
    DashRefillTime,
    DashInputBufferTime,
    CoyoteTime,
    JumpInputBufferTime
};

