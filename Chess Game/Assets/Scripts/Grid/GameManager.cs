using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; 
    public GameStates state;
    public static event Action<GameStates> OnGameStateChanged;
    Pawn pawn;
    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        UpdateGameState(GameStates.PlayerTurn1);
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
                Debug.Log("Player 2's turn");
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

    public enum GameStates
    {
        SelectPiece,
        PlayerTurn1,
        PlayerTurn2,
        Victory,
        Draw,
        GameOver
    }
}
