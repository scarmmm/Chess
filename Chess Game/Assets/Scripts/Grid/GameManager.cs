using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    public GameStates state;
    public GameMode gameMode;
    public static event Action<GameStates> OnGameStateChanged;

    private Pawn _pawn;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
    }
    void Start()
    {
        UpdateGameState(GameStates.PlayerTurn1);
        gameMode = MainMenu.selectedMode;
    }
    
    public void UpdateGameState(GameStates newState) 
    {
       state = newState;
        switch (newState) 
        {
            case GameStates.SelectPiece:
                //Debug.Log("Select a piece");
                break;
            case GameStates.PlayerTurn1:
                Debug.Log("Player 1's turn");
                break;
            case GameStates.PlayerTurn2:
                if (gameMode == GameMode.AIEasy)
                {
                    Debug.Log("Started AI easy code");
                    StartCoroutine(FindObjectOfType<Pawn>().HandleAIEasyMove());
                }
                else if (gameMode == GameMode.AIMedium)
                    Debug.Log("We will run out minimax here");  
                break;
            case GameStates.Victory:
                //Debug.Log("Victory");
                break;
            case GameStates.Draw:
                //Debug.Log("Draw");
                break;
            default:
                throw new ArgumentOutOfRangeException();

        }
        OnGameStateChanged?.Invoke(state);
    }

    public GameStates GetCurrentGameState()
    {
        return state;
    }

    public GameMode GetGameMode()
    {
        return gameMode;
    }

    public enum GameStates
    {
        SelectPiece,
        PlayerTurn1,
        PlayerTurn2,
        Victory,
        Draw,
    }

    public enum GameMode
    {
        AIEasy,
        AIMedium,
        LocalMultiPlayer
    }
}
