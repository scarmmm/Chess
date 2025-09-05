using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AI : MonoBehaviour
{
    public AllValidMoves _validMoves; 
    public Pawn _pawn;
    
    public GameObject ReturnRandomPiece(List<GameObject> pieces)
    {
        if (pieces.Count == 0)
            return  null;
        GameObject piece = null;
        var size = pieces.Count;
        var index = Random.Range(0, size);
        piece = pieces[index];
        return piece;
    }

    public bool GetRandomMove(GameObject pieceSelected, Vector3Int currentPosition, List<Vector3Int> possibleMoves) 
    {   
        if (possibleMoves == null || possibleMoves.Count == 0)
            return false;

        var attempts = 0;
        const int maxAttempts = 50; // number doesn't really matter (yet doesn't need to be high)
    
        while (attempts < maxAttempts)
        {
            var index = Random.Range(0, possibleMoves.Count);
            var moveSelected = possibleMoves[index];
            var value = _pawn.IsValidPosition(pieceSelected, currentPosition, moveSelected, true);
            Debug.Log("Piece that was moved" + pieceSelected);
            if (value)
            {
                _pawn.MoveToCenterOfGrid(moveSelected, pieceSelected);
                Debug.Log($"Valid move found: {moveSelected}");
                return true; // return once we have a valid move
            }

            attempts++;
        }
    
        Debug.LogWarning($"No valid moves found for {pieceSelected.name} after {attempts} attempts.");
        return false;
    }
    
    
    
}
