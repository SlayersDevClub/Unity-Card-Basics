using System.Collections;
using DG.Tweening;
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

    public TextMeshProUGUI damageTextPrefab;

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

    public void PlayerTakeDamage(int health, int damage)
    {
        playerHealthbar.fillAmount = (float)health/100;
        ShowDamage(damage, playerHealthbar);
    }

    public void AITakeDamage(int health, int damage)
    {
        enemyHealthbar.fillAmount = (float)health/100;
        ShowDamage(damage, playerHealthbar);
    }



    public void ShowDamage(int damageAmount, Image healthBar)
    {
        // Instantiate the damage text
        TextMeshProUGUI damageText = Instantiate(damageTextPrefab, healthBar.rectTransform.position, Quaternion.identity, healthBar.rectTransform.parent);

        // Set the damage amount
        damageText.text = damageAmount.ToString();

        // Animate the text falling off the health bar
        Vector3 targetPosition = healthBar.rectTransform.position + new Vector3(0, -100f, 0); // Adjust the Y offset as needed
        damageText.transform.DOMove(targetPosition, 1f).SetEase(Ease.OutQuad);

        // Animate the alpha (fade out)
        damageText.DOFade(0, 1f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // Destroy the text after the animation
            Destroy(damageText.gameObject);
        });
    }
}
