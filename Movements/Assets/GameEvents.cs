using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance;
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    public event Action onPlayerSwitch;
    public void PlayerSwitch()
    {
        if(onPlayerSwitch != null)
        {
            onPlayerSwitch();
        }
    }
}
