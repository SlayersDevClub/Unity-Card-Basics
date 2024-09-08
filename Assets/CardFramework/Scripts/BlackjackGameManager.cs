using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlackjackGameManager : Singleton<BlackjackGameManager>
{
    public Dealer _dealer, _aiDealer;
    public Button hitButton, standButton;
    public float dealSpeed = 3;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AIDraw()
    {
        //StartCoroutine(_aiDealer.DrawCoroutine());
    }

    public void Draw()
    {
        if (_dealer.DealInProgress == 0)
        {
            if(CheckScore() >= 21)
            {
                Discard();
                //StartCoroutine(_dealer.DrawCoroutine());
            }
            else
            {
                //StartCoroutine(_dealer.DrawCoroutine());

            }
        }
    }

    public void Discard()
    {
        _dealer.DiscardCard();
        hitButton.interactable = true;
    }

    public void Stand()
    {
        print("Your current score is " + _dealer.currentCollectiveCardValue);
        hitButton.interactable = true;
    }

    public int CheckScore()
    {
        if(_dealer.currentCollectiveCardValue > 21)
        {
            print("Bust!");
            hitButton.interactable = false;
            
        }
        if(_dealer.currentCollectiveCardValue == 21)
        {
            print("Black jack!");
        }        

        return _dealer.currentCollectiveCardValue;
    }
}
