using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluateBoard : MonoBehaviour
{
    private int GetPieceValue(Piece piece)
    {
        var ID = piece.Type;
        int value = 0;
        switch (ID)
        {
            case Identity.Pawn:
                value = 1;
                break;
            case Identity.Rook:
            case Identity.Knight:
            case Identity.Bishop:
                value = 3;
                break;
            case  Identity.Queen:
                value = 9;
                break;
            case Identity.King:
                value = 100;
                break;
        }

        return value;
    } 
    
    
    public int GetBoardScore(Dictionary<Vector3Int, Piece> board, bool isMaximizer)
    {
        var score = 0;
        if(isMaximizer)
        {
            //loop through and count the score for each piece
            foreach (var piece in board)
            {
                switch (piece.Value.Team)
                {
                    case Team.White:
                        score += GetPieceValue(piece.Value);
                        break;
                    case Team.Black:
                        score -= GetPieceValue(piece.Value);
                        break;
                }
            }
        }
        else
        {
            //loop through and count the score for each piece
            foreach (var piece in board)
            {
                switch (piece.Value.Team)
                {
                    case Team.Black:
                        score += GetPieceValue(piece.Value);
                        break;
                    case Team.White:
                        score -= GetPieceValue(piece.Value);
                        break;
                }
            }
        }
        
        return score;
    }
    
}
