using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackGameManager : Singleton<BlackjackGameManager>
{
    public float dealSpeed;
    BlackjackStateMachine StateMachine;

    public int playerHealth = 100, playerArmor, enemyHealth = 100, enemyArmor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

  
}
