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

    public TextMeshProUGUI playerScore, aiScore, outputText;
    public Image playerHealthbar, enemyHealthbar;
    public TextMeshProUGUI damageTextPrefab;
    public AttackFXMover spawner;
    public RectTransform player, enemy;

    void Start()
    {
        hitButton.onClick.AddListener(OnHitButtonClicked);
        standButton.onClick.AddListener(OnStandButtonClicked);
    }

    async void OnHitButtonClicked()
    {
        await blackjackStateMachine.PlayerHits();
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
        FlashRed();
        ShakeCamera();
        spawner.SpawnProjectile(enemy, player);
    }

    public void AITakeDamage(int health, int damage)
    {
        enemyHealthbar.fillAmount = (float)health/100;
        ShowDamage(damage, enemyHealthbar);
        DOTween.Play("hit");
        spawner.SpawnProjectile(player, enemy);
    }

    public Animator playerGun, aiGun;

    public void AIKillPlayer()
    {
        if(aiGun)aiGun.SetTrigger("Shoot");
    }

    public void PlayerKillAI()
    {
        if(playerGun)playerGun.SetTrigger("Shoot");
    }

    public void ShowDamage(int damageAmount, Image healthBar)
    {
        // Instantiate the damage text
        TextMeshProUGUI damageText = Instantiate(damageTextPrefab, healthBar.rectTransform.position, Quaternion.identity, healthBar.rectTransform.parent);

        // Set the damage amount
        damageText.text = damageAmount.ToString();

        // Animate the text falling off the health bar
        Vector3 targetPosition = healthBar.rectTransform.position + new Vector3(30, -100f, 0); // Adjust the Y offset as needed
        damageText.transform.DOMove(targetPosition, 2f).SetEase(Ease.OutQuad);

        // Animate the alpha (fade out)
        damageText.DOFade(0, 1f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            // Destroy the text after the animation
            Destroy(damageText.gameObject);
        });
    }
    private Tween currentTween;
    public void ShowText(string message, float displayDuration = 2f, float fadeDuration = 0.5f)
    {
        // Interrupt current tween if it's running
        if (currentTween != null && currentTween.IsActive())
        {
            currentTween.Kill();
        }

        // Set the text
        outputText.SetText(message);

        // Start with fully transparent text
        outputText.alpha = 0f;

        // Fade in
        currentTween = outputText.DOFade(1f, fadeDuration).OnComplete(() =>
        {
            // Keep the text displayed for `displayDuration` seconds
            currentTween = DOVirtual.DelayedCall(displayDuration, () =>
            {
                // Fade out
                currentTween = outputText.DOFade(0f, fadeDuration);
            });
        });
    }
    public Canvas aiCanvas;

    public TextMeshProUGUI AIVoiceText;

    public void ShowAIText(string message, float displayDuration = 2f, float fadeDuration = 0.5f)
    {
        Tween currentTween;

        GameObject aiVoiceObj = (GameObject)Instantiate(AIVoiceText.gameObject, aiCanvas.transform) as GameObject;
        TextMeshProUGUI AIVoiceTxt = aiVoiceObj.GetComponent<TextMeshProUGUI>();

        AIVoiceTxt.SetText(message);
        AIVoiceTxt.alpha = 0f;

        // Fade in
        currentTween = AIVoiceTxt.DOFade(1f, fadeDuration).OnComplete(() =>
        {

            currentTween = AIVoiceTxt.DOFade(0f, fadeDuration).SetDelay(displayDuration);

        });

        AIVoiceTxt.GetComponent<DOTweenAnimation>().DORewindAndPlayNext();
    }

    [SerializeField] private Image damageImage, blackImage;  // Assign a full-screen UI Image with red color and 0 alpha
    [SerializeField] private float flashDuration = 0.2f;
    [SerializeField] private float flashAlpha = 0.5f;

    public void FlashRed()
    {
        damageImage.DOFade(flashAlpha, flashDuration / 2)
            .OnComplete(() => damageImage.DOFade(0, flashDuration / 2));
    }
    public void FadeToBlack()
    {
        blackImage.color = Color.white;
        blackImage.DOFade(.9f, flashDuration / 2).OnComplete(() =>
        {
            blackImage.DOColor(Color.black, flashDuration/3);
            blackImage.DOFade(1, flashDuration / 2).SetDelay(flashDuration);

        });
    }

    public void PlayerKilledFlash()
    {
        blackImage.color = Color.white;
        blackImage.DOFade(.9f, flashDuration / 2).OnComplete(() =>
        {
            blackImage.DOFade(0, flashDuration / 2).SetDelay(flashDuration);

        });
    }
    public void AIKillPlayerFlash()
    {
        blackImage.color = Color.white;
        blackImage.DOFade(.9f, flashDuration / 2).OnComplete(() =>
        {
            blackImage.DOColor(Color.black, flashDuration / 2);
            blackImage.DOFade(255, flashDuration / 2).SetDelay(flashDuration);

        });
    }
    public GameObject playerBreakImage;
    public void ShowPlayerX(bool b)
    {
        if(!b)
        playerBreakImage.SetActive(false);
        else
        playerBreakImage.SetActive(true);
    }


    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 1f;
    [SerializeField] private int shakeVibrato = 10;
    [SerializeField] private float shakeRandomness = 90f;

    Tween camShakeTween;
    public void ShakeCamera()
    {
        if(camShakeTween == null)
            camShakeTween = Camera.main.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato, shakeRandomness).SetAutoKill(false);

            camShakeTween.Rewind();
            camShakeTween.Play();
    }
}
