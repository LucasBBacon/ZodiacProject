using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DropDownPlayerType : MonoBehaviour
{
    private TMP_Dropdown _dropdown;
    private int _index;
    private List<string> _playerTypes = new List<string>() {"Celeste", "Hollow Knight", "Custom Profile 1", "Custom Profile 2"};

    private void Start() 
    {
        _index = GameManager.instance._currentPlayerTypeIndex;
        _dropdown = gameObject.GetComponent<TMP_Dropdown>();
        _dropdown.ClearOptions();
        _dropdown.AddOptions(_playerTypes);
    }

    public void UpdatePlayerTypeList()
    {
        //_index = _dropdown.value;
        GameManager.instance.SwitchPlayerType(_dropdown.value);
    }
}
