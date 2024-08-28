using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;

public class BlackjackStateMachine : MonoBehaviour
{
    public GameState currentState;

    // Example properties for player and AI scores and health
    private int playerScore = 0;
    private int aiScore = 0;
    private int playerHealth = 100;
    private int aiHealth = 100;

    bool playerStood = false, aiStood = false;
    bool playerBroke = false, aiBroke = false;

    public Dealer dealerPlayer, dealerAI;

    void Start()
    {
        currentState = GameState.Phase0_DrawDecks;
        TransitionToState(currentState);
    }

    //void Update()
    //{
    //    // Game loop
    //    switch (currentState)
    //    {
    //        case GameState.Phase0_DrawDecks:
    //            HandlePhase0();
    //            break;
    //        case GameState.Phase1_PlayerTurn:
    //            HandlePhase1();
    //            break;
    //        case GameState.Phase2_AITurn:
    //            HandlePhase2();
    //            break;
    //        case GameState.Phase3_CalculateScores:
    //            HandlePhase3();
    //            break;
    //        case GameState.Phase4_EndGame:
    //            HandlePhase4();
    //            break;
    //    }
    //}

    void TransitionToState(GameState newState)
    {
        currentState = newState;
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
        TransitionToState(GameState.Phase1_PlayerTurn);
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
                TransitionToState(GameState.Phase2_AITurn);
                return;
            } 
            if(playerScore < 21)
            {
                TransitionToState(GameState.Phase2_AITurn);
            }
            else
            {
                playerBroke = true;
                playerStood = true;
                if (!aiStood || !aiBroke)
                    TransitionToState(GameState.Phase2_AITurn);
                else
                    TransitionToState(GameState.Phase3_CalculateScores);
            }
        }
    }

    public void PlayerStands()
    {
        if (currentState == GameState.Phase1_PlayerTurn)
        {
            playerStood = true;
            // Transition to AI's turn
            TransitionToState(GameState.Phase2_AITurn);
        }
    }


    void HandlePhase1()
    {
        // Handle player actions: hit or stand
        Debug.Log("Phase 1: Player's turn...");
        if(playerStood && aiStood)
        { 
            TransitionToState(GameState.Phase3_CalculateScores);
            return;
        }
        if(playerStood)
            TransitionToState(GameState.Phase2_AITurn);
    }

    void HandlePhase2()
    {
        // Handle AI actions: hit or stand
        Debug.Log("Phase 2: AI's turn...");

        // Example condition: AI chooses to hit
        bool aiHits = true; // Placeholder for AI decision-making

        if(aiScore >=17 && aiScore  <= 21)
        {
            aiStood = true;
            print("AI STOOD");
            aiHits = false;


        }
        if (aiHits)
        {
            // Add card to AI's hand
            aiScore += DrawCard();
            Debug.Log("<Color=red>AI hits. Score: </color>" + aiScore);
            BlackjackUIManager.Instance.UpdateAIScore(aiScore);


            if (aiScore == 21)
            {
                TransitionToState(GameState.Phase1_PlayerTurn);
                aiStood = true;
                return;
            }

            // Loop back to Phase 1 if AI has not broken 21
            if (aiScore < 21)
            {
                if(!playerStood)
                {
                    TransitionToState(GameState.Phase1_PlayerTurn);
                }

                else
                    HandlePhase2();
            }
            else{
                aiBroke = true;
                TransitionToState(GameState.Phase3_CalculateScores);}
        }
        else
        {
            if (!playerBroke)
                TransitionToState(GameState.Phase1_PlayerTurn);
            if(playerStood)
                TransitionToState(GameState.Phase3_CalculateScores);
        }
    }

    void HandlePhase3()
    {
        // Calculate difference in scores
        Debug.Log("Phase 3: Calculating scores...");
        int scoreDifference = Mathf.Abs(playerScore - aiScore);

        // Apply damage to the opposing side
        if (!playerBroke)
        {
            if (aiBroke)
            {
                aiHealth -= playerScore;
                BlackjackUIManager.Instance.PlayerTakeDamage(playerHealth, playerScore);
            }
            else
            {
                if (playerScore > aiScore)
                {
                    aiHealth -= scoreDifference;
                    BlackjackUIManager.Instance.AITakeDamage(aiHealth, scoreDifference);
                }
            }
            Debug.Log("AI takes damage. AI Health: " + aiHealth);

        }
            
        if (!aiBroke)
        {          
                if (playerBroke)
                {
                    playerHealth -= aiScore;
                BlackjackUIManager.Instance.PlayerTakeDamage(playerHealth, aiScore);
            }
                else
                {
                    if (aiScore > playerScore)
                        playerHealth -= scoreDifference;
                BlackjackUIManager.Instance.PlayerTakeDamage(playerHealth, scoreDifference);
            }           
        }

        // Check for end game condition
        if (playerHealth <= 0 || aiHealth <= 0)
        {
            TransitionToState(GameState.Phase4_EndGame);
        }
        else
        {
            playerScore = 0;
            aiScore = 0;
            playerBroke = false;
            aiBroke = false;
            playerStood = false;
            aiStood = false;
            dealerPlayer.DiscardCard();
            BlackjackUIManager.Instance.UpdateAIScore(aiScore);
            BlackjackUIManager.Instance.UpdatePlayerScore(playerScore);
            // Return to Phase 1 for a new round
            TransitionToState(GameState.Phase1_PlayerTurn);
        }
    }

    void HandlePhase4()
    {
        // Determine winner and end the game
        Debug.Log("Phase 4: End Game.");
        if (playerHealth <= 0)
        {
            Debug.Log("AI Wins!");
        }
        else if (aiHealth <= 0)
        {
            Debug.Log("Player Wins!");
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


}
