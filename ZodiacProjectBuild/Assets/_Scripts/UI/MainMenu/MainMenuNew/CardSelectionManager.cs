using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectionManager : MonoBehaviour
{
    public GameObject[] Cards;

    public GameObject LastSelected { get; set; }
    public int LastSelectedIndex { get; set; }

    private void OnEnable()
    {
        StartCoroutine(SetSelectedAfterOneFrame());
    }

    IEnumerator SetSelectedAfterOneFrame()
    {
        yield return null;
        EventSystem.current.SetSelectedGameObject(Cards[^1]);    
    }

    // void HandleNextCardSelection(int addition)
    // {
    //     if (
    //         EventSystem.current.currentSelectedGameObject == null
    //         && LastSelected != null
    //         )
    //     {
    //         int newIndex = LastSelectedIndex + addition;
    //         newIndex = Mathf.Clamp(newIndex, 0, Cards.Length - 1);
    //         Debug.Log(newIndex);
    //         EventSystem.current.SetSelectedGameObject(Cards[newIndex]);
    //     }
    // }
}
