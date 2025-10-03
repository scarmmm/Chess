using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EasyAI : MonoBehaviour
{
    public AllValidMoves _validMoves; 
    public Pawn _pawn;
    public BoardState _boardState;
    
    public GameObject ReturnRandomPiece(List<GameObject> pieces)
    {
        if (pieces.Count == 0)
            return null;
        GameObject piece = null;
        var currentBoardState = _pawn.ConvertGameObjectsToDictionary();
        var kingPosition = _pawn.GetGridPosition(_pawn.kings2[0]);
        if (_boardState.IsGridUnderAttack(kingPosition, currentBoardState))
        {
            piece = _pawn.kings2[0];
            return piece;
        }
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
        const int maxAttempts = 20; // number doesn't really matter (yet doesn't need to be high)
        while (attempts < maxAttempts)
        {
            var index = Random.Range(0, possibleMoves.Count);
            var moveSelected = possibleMoves[index];
            //we do the check logic here, but it may not be optimal 
            var value = _pawn.IsValidPosition(pieceSelected, currentPosition, moveSelected, true);
            //we need to make sure the simulated move does (can only be checked if valid)
            //not place our king in check (this is not optimal at all)
            if (value)
            {
                var id = pieceSelected.GetComponent<PieceIdentity>().pieceType;
                var identity = _boardState.Convert2(id);
                var team = pieceSelected.CompareTag("Player1") ? Team.Black : Team.White;
                var currentBoardState = _pawn.ConvertGameObjectsToDictionary();
                var newboard = new Dictionary<Vector3Int, Piece>(currentBoardState);
                //remove old piece position that is being moved
                newboard.Remove(_pawn.GetGridPosition(pieceSelected));
                //make sure the position is occupied and it has an enemy piece to remove
                if (currentBoardState.ContainsKey(moveSelected) && currentBoardState[moveSelected].Team == Team.Black)
                    newboard.Remove(moveSelected);
                if (identity != null) newboard.Add(moveSelected, new Piece((Identity)identity, team));
                var isMaximizer = GameManager.Instance.GetCurrentGameState() != GameManager.GameStates.PlayerTurn1;
                if (_boardState.WillMovePlaceUsInCheck(_pawn.GetGridPosition(pieceSelected), moveSelected, newboard, isMaximizer))
                    return false;
                _pawn.MoveToCenterOfGrid(moveSelected, pieceSelected);
                return true; // return once we have a valid move
            }
            attempts++;
        }
    
        Debug.LogWarning($"No valid moves found for {pieceSelected.name} after {attempts} attempts.");
        return false;
    }
    
    
    
}
