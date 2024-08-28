using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlackjackUIManager : Singleton<BlackjackUIManager>
{
    public BlackjackStateMachine blackjackStateMachine;
    public Button hitButton;
    public Button standButton;

    public TextMeshProUGUI playerScore, aiScore;
    public Image playerHealthbar, enemyHealthbar;

    void Start()
    {
        hitButton.onClick.AddListener(OnHitButtonClicked);
        standButton.onClick.AddListener(OnStandButtonClicked);
    }

    void OnHitButtonClicked()
    {
        blackjackStateMachine.PlayerHits();
    }

    void OnStandButtonClicked()
    {
        blackjackStateMachine.PlayerStands();
    }

    void Update()
    {

        switch (blackjackStateMachine.currentState)
        {
            case GameState.Phase1_PlayerTurn:
                hitButton.interactable = true;
                standButton.interactable = true;
                break;
            default:
                hitButton.interactable = false;
                standButton.interactable = false;
                break;
        }
    }

    public void UpdatePlayerScore(int score)
    {
        playerScore.SetText(score.ToString());
    }

    public void UpdateAIScore(int score)
    {
        aiScore.SetText(score.ToString());
    }

    public void PlayerTakeDamage(int health)
    {
        playerHealthbar.fillAmount = (float)health/100;
    }

    public void AITakeDamage(int health)
    {
        enemyHealthbar.fillAmount = (float)health/100;
    }
}
