using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceElimination : MonoBehaviour
{
    private Pawn pawn;
    
    private void Start()
    {
        pawn = FindObjectOfType<Pawn>();
    }

    private void CheckElimination()
    {
        var currentTurn = GameManager.Instance.GetCurrentGameState();
        switch (currentTurn)
        {
            case (GameManager.GameStates.PlayerTurn1):
                
                break;
            case (GameManager.GameStates.PlayerTurn2):
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    
    
    
}
