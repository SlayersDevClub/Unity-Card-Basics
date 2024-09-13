using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections;

public class BlackjackStateMachine : Singleton<BlackjackStateMachine>
{
    public GameState currentState;

    // Example properties for player and AI scores and health
    private int playerScore = 0;
    private int aiScore = 0;
    private int playerHealth = 100;
    private int aiHealth = 100;
    public int playerArmor = 0, enemyArmor = 0;

    bool playerStood = false, aiStood = false;
    bool aiBroke
    {
        get { return _aiBroke; }
        set
        {
            _aiBroke = value;
            if (value)
            {
                BlackjackUIManager.Instance.aiBreakImage.SetActive(true);
            }
            else
            {
                BlackjackUIManager.Instance.aiBreakImage.SetActive(false);
            }
        }
    }
    bool _aiBroke = false;

    bool playerBroke 
    {
        get{return _playerBroke;} 
        set
        {
            if(value)
            {
                BlackjackUIManager.Instance.playerBreakImage.SetActive(true);
            }
            else 
            {
                BlackjackUIManager.Instance.playerBreakImage.SetActive(false);
            }
            _playerBroke = value;
        }
    }
    bool _playerBroke;

    public Dealer dealerPlayer, dealerAI;

    void Start()
    {
        currentState = GameState.Phase0_DrawDecks;
        StartCoroutine(TransitionToState(currentState));
        playerBroke = false;
        aiBroke = false;
    }

    IEnumerator TransitionToState(GameState newState)
    {
        currentState = newState;
        yield return new WaitForSeconds(0.1f);
        switch (currentState)
        {
            case GameState.Phase0_DrawDecks:
                HandlePhase0();
                break;
            case GameState.Phase1_PlayerTurn:
                HandlePhase1();
                break;
            case GameState.Phase2_AITurn:
                //Invoke("HandlePhase2", 2f);
                HandlePhase2();
                break;
            case GameState.Phase3_CalculateScores:
                //Invoke("HandlePhase3", 2f);
                HandlePhase3();
                break;
            case GameState.Phase4_EndGame:
                HandlePhase4();
                break;
        }
    }

    void HandlePhase0()
    {
        // Initialize decks, shuffle, etc.
        Debug.Log("Phase 0: Drawing decks...");

        // Transition to Phase 1
        StartCoroutine(TransitionToState(GameState.Phase1_PlayerTurn));
    }

    public async Task PlayerHits()
    {
        if (currentState == GameState.Phase1_PlayerTurn)
        {
            // Add card to player's hand
            int value = await PlayerDrawCard();
            playerScore += value;
            
            Debug.Log("<Color=green>Player hits. Score: </color>" + playerScore);
            BlackjackUIManager.Instance.UpdatePlayerScore(playerScore);


            // Transition to AI's turn or calculate scores if player breaks 21
            if (playerScore == 21)
            {
                StartCoroutine(TransitionToState(GameState.Phase2_AITurn));
                return;
            } 
            if(playerScore < 21)
            {
                StartCoroutine(TransitionToState(GameState.Phase2_AITurn));
            }
            else
            {
                playerBroke = true;
                playerStood = true;
                BlackjackUIManager.Instance.ShowText("You stood.");

                if (!aiStood || !aiBroke)
                    StartCoroutine(TransitionToState(GameState.Phase2_AITurn));
                else
                    StartCoroutine(TransitionToState(GameState.Phase3_CalculateScores));
            }
        }
    }

    public void PlayerStands()
    {
        if (currentState == GameState.Phase1_PlayerTurn)
        {
            playerStood = true;
            BlackjackUIManager.Instance.ShowText("You stood.");
            StartCoroutine(TransitionToState(GameState.Phase1_PlayerTurn));
        }
    }


    void HandlePhase1()
    {
        // Handle player actions: hit or stand
        Debug.Log("Phase 1: Player's turn...");
        if (playerStood && aiStood)
        {
            StartCoroutine(TransitionToState(GameState.Phase3_CalculateScores));
            return;
        }
        if (playerStood && !aiStood)
            StartCoroutine(TransitionToState(GameState.Phase2_AITurn));



    }

    async void HandlePhase2()
    {
        // Handle AI actions: hit or stand
        Debug.Log("Phase 2: AI's turn...");

        if(aiBroke || aiStood)
        {
        if(!playerBroke) StartCoroutine(TransitionToState(GameState.Phase1_PlayerTurn));
        else StartCoroutine(TransitionToState(GameState.Phase3_CalculateScores));
        }

        // Example condition: AI chooses to hit
        bool aiHits = true; // Placeholder for AI decision-making

        if(aiScore >=17 && aiScore  <= 21)
        {
            aiStood = true;
            BlackjackUIManager.Instance.ShowAIText("I stand.");

            aiHits = false;


        }
        if (aiHits)
        {
            // Add card to AI's hand
            int value = await AIDrawCard();
            aiScore +=  value;
            Debug.Log("<Color=red>AI hits. Score: </color>" + aiScore);
            BlackjackUIManager.Instance.UpdateAIScore(aiScore);


            if (aiScore == 21)
            {
                StartCoroutine(TransitionToState(GameState.Phase1_PlayerTurn));
                aiStood = true;
                BlackjackUIManager.Instance.ShowAIText("Blackjack, bitch.");
                return;
            }

            // Loop back to Phase 1 if AI has not broken 21
            if (aiScore < 21)
            {
                if(!playerBroke)
                {
                    StartCoroutine(TransitionToState(GameState.Phase1_PlayerTurn));
                }

                else
                    StartCoroutine(TransitionToState(GameState.Phase2_AITurn));

            }
            if(aiScore > 21)
            {
                aiBroke = true;
                aiStood = true;
                BlackjackUIManager.Instance.ShowAIText("Fuck.. I broke.");

                if (playerBroke || playerStood)
                    StartCoroutine(TransitionToState(GameState.Phase3_CalculateScores));
                else
                    StartCoroutine(TransitionToState(GameState.Phase1_PlayerTurn));

            }
        }
        else
        {
            if (!playerBroke)
                StartCoroutine(TransitionToState(GameState.Phase1_PlayerTurn));
            else
                StartCoroutine(TransitionToState(GameState.Phase3_CalculateScores));

            BlackjackUIManager.Instance.ShowAIText("I stand.");

        }
    }

    void HandlePhase3()
    {
        Debug.Log("Damage Phase");

        int scoreDifference = 0;

        // AI BROKE
        if (!playerBroke && aiBroke)
        {
            aiHealth -= playerScore;
            AITakeDamage(aiHealth, playerScore);
            BlackjackUIManager.Instance.ShowText("Prisoner takes " + playerScore + " damage!");
            BlackjackUIManager.Instance.ShowAIText("Ouch..");              
        }
        //PLAYER BROKE
        if (!aiBroke && playerBroke)
        {
            playerHealth -= aiScore;
            PlayerTakeDamage(playerHealth, aiScore);
            BlackjackUIManager.Instance.ShowText("Player takes " + aiScore + " damage!");
            BlackjackUIManager.Instance.ShowAIText("Heh.");
        }
        //BOTH PLAYERS NOT BREAK
        if (!playerBroke && !aiBroke)
        {
            if(playerScore > aiScore)
            {
                scoreDifference = Mathf.Abs(playerScore - aiScore);
                aiHealth -= scoreDifference;
                AITakeDamage(aiHealth, scoreDifference);
                BlackjackUIManager.Instance.ShowText("Prisoner takes " + scoreDifference + " damage!");
                BlackjackUIManager.Instance.ShowAIText("OUCH!");
            }
            if(aiScore > playerScore)
            {
                scoreDifference = Mathf.Abs(aiScore - playerScore);
                playerHealth -= scoreDifference;
                PlayerTakeDamage(playerHealth, scoreDifference);
                BlackjackUIManager.Instance.ShowText("Player takes " + scoreDifference + " damage!");
                BlackjackUIManager.Instance.ShowAIText("Oh yeah..");
            }
            if (playerScore == aiScore)
            {
                BlackjackUIManager.Instance.ShowText("Draw!");
            }       
        }


        // Check for end game condition
        if (playerHealth <= 0 || aiHealth <= 0)
        {
            StartCoroutine(TransitionToState(GameState.Phase4_EndGame));
        }
        else
        {
            playerStood = false;
            aiStood = false;
            // Return to Phase 1 for a new round
            StartCoroutine  (WaitForDamagePhase());
        }
    }

    IEnumerator WaitForDamagePhase()
    {
        yield return new WaitForSeconds(2);
        playerScore = 0;
        aiScore = 0;
        playerBroke = false;
        aiBroke = false;
        dealerPlayer.DiscardCard();
        dealerAI.DiscardCard();
        BlackjackUIManager.Instance.UpdateAIScore(aiScore);
        BlackjackUIManager.Instance.UpdatePlayerScore(playerScore);
        StartCoroutine(TransitionToState(GameState.Phase1_PlayerTurn));
    }

    public RectTransform player, enemy;
    public void PlayerTakeDamage(int health, int damage)
    {
        tmpDamage = damage;
        tmpHealth = health;
        BlackjackUIManager.Instance.spawner.SpawnProjectile(enemy, player, damage, true);
        Invoke("PlayerTakeDamageDelay", 1f);

    }
    int tmpHealth, tmpDamage;
    void PlayerTakeDamageDelay()
    {
        int tmpArmor = BlackjackGameManager.Instance.playerArmor;
        if (tmpArmor > 0)
        {
            int dmgDifference = tmpArmor - tmpDamage;
            if (tmpDamage > tmpArmor)
            {

            }
        }
        else
        {
            BlackjackUIManager.Instance.playerHealthbar.fillAmount = (float)tmpHealth / 100;
            BlackjackUIManager.Instance.playerHealthbar.GetComponentInChildren<TMPro.TextMeshProUGUI>().SetText(tmpHealth.ToString());
            BlackjackUIManager.Instance.ShowDamage(tmpDamage, BlackjackUIManager.Instance.playerHealthbar);
        }

        BlackjackUIManager.Instance.FlashRed();
        BlackjackUIManager.Instance.ShakeCamera();
    }

    public void AITakeDamage(int health, int damage)
    {
        BlackjackUIManager.Instance.enemyHealthbar.fillAmount = (float)health / 100;
        BlackjackUIManager.Instance.ShowDamage(damage, BlackjackUIManager.Instance.enemyHealthbar);
        DG.Tweening.DOTween.Play("hit");
        BlackjackUIManager.Instance.spawner.SpawnProjectile(player, enemy, damage);
    }

    void HandlePhase4()
    {
        // Determine winner and end the game
        Debug.Log("Phase 4: End Game.");
        if (playerHealth <= 0)
        {
            Debug.Log("AI Wins!");
            BlackjackUIManager.Instance.ShowText("Prisoner wins! Sweet dreams..");
            BlackjackUIManager.Instance.ShowAIText("Sweet dreams..");
            BlackjackUIManager.Instance.AIKillPlayer();
        }
        else if (aiHealth <= 0)
        {
            Debug.Log("Player Wins!");
            BlackjackUIManager.Instance.ShowText("You Win!!");
            BlackjackUIManager.Instance.ShowAIText("Oh no..");
            BlackjackUIManager.Instance.PlayerKillAI();
        }
    }

    int DrawCard()
    {
        // Placeholder for drawing a card from the deck
        return Random.Range(1, 11); // Example: return a card value between 1 and 10
    }

    async Task<int> PlayerDrawCard()
    {
        int result = await dealerPlayer.PlayerDrawCard();
        return result;
    }

    async Task<int> AIDrawCard()
    {
        int result = await dealerAI.PlayerDrawCard();
        return result;
    }

}
