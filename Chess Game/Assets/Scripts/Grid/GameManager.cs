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
    Pawn pawn;
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
                if (gameMode == GameMode.AI)
                {
                    // AI takes over Player 2's turn
                    StartCoroutine(FindObjectOfType<Pawn>().HandleAIMove());
                }
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

    public GameStates getCurrentGameState()
    {
        return state;
    }

    public GameMode getGameMode()
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
        GameOver
    }

    public enum GameMode
    {
        AI, 
        LocalMultiPlayer
    }
}
