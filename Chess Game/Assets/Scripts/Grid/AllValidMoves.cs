using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllValidMoves : MonoBehaviour
{
    private readonly int minX = -6;
    private readonly int maxX = 1;
    private readonly int minY = 0;
    private readonly int maxY = 7;
    
    
    //this script will get all available moves that a given piece has
    //the AI will call this script to get its possible moves after it chooses a piece (minimax needs this aswell)
   public List<Vector3Int> GetCandidates(GameObject pieceSelected, Vector3Int currentPosition)
    {
        List<Vector3Int> candidates = new List<Vector3Int>();
        PieceIdentity pieceIdentity = pieceSelected.GetComponent<PieceIdentity>();
        var id = pieceIdentity.pieceType;
        switch (id)
        {
            case ChessPieceType.Player1King:
                case ChessPieceType.Player2King:
                AddKingMoves(candidates, currentPosition);
                break;

            case ChessPieceType.Player1Knight:
                case ChessPieceType.Player2Knight:
                AddKnightMoves(candidates, currentPosition);
                break;
            case ChessPieceType.Player1Pawn:
                case ChessPieceType.Player2Pawn:
                    var firstMoveComponent = pieceSelected.GetComponent<PawnMove>();
                    var hasMoved = firstMoveComponent.isFirstMove;
                    AddPawnMoves(candidates, currentPosition,id,hasMoved);
                    break;
            case ChessPieceType.Player1Queen:
                case ChessPieceType.Player2Queen:
                    AddSlidingMoves(candidates, currentPosition, new Vector2Int[] {
                        new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1),
                        new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
                    });
                    break;
            case ChessPieceType.Player1Rook:
                case ChessPieceType.Player2Rook:
                    AddSlidingMoves(candidates,currentPosition, new Vector2Int[]
                    {
                        new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1),
                    });
                break;
            case ChessPieceType.Player1Bishop:
            case ChessPieceType.Player2Bishop:
                AddSlidingMoves(candidates, currentPosition, new Vector2Int[] {
                    new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
                });
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        foreach (var c in candidates)
        {
            //Debug.Log($"Candidate: {c}");
        }

        return candidates;
    }
    // Overload for the Board State
    public List<Vector3Int> GetCandidates(Piece piece, Vector3Int currentPosition, bool hasMoved = true)
    {
        List<Vector3Int> candidates = new List<Vector3Int>();
        var type = piece.Type;
        ChessPieceType id = ChessPieceType.Player1Pawn;
        switch (type)
        {
            case Identity.King:
                AddKingMoves(candidates, currentPosition);
                break;

            case Identity.Knight:
                AddKnightMoves(candidates, currentPosition);
                break;

            case Identity.Pawn:
                if (piece.Team == Team.Black)
                    id = ChessPieceType.Player1Pawn;
                else if (piece.Team == Team.White)
                    id = ChessPieceType.Player2Pawn;
                AddPawnMoves(candidates, currentPosition, id, hasMoved);
                break;

            case Identity.Queen:
                AddSlidingMoves(candidates, currentPosition, new Vector2Int[] {
                    new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1),
                    new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
                });
                break;

            case Identity.Rook:
                AddSlidingMoves(candidates, currentPosition, new Vector2Int[] {
                    new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1)
                });
                break;

            case Identity.Bishop:
                AddSlidingMoves(candidates, currentPosition, new Vector2Int[] {
                    new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
                });
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        return candidates;
    }


    private void AddPawnMoves(List<Vector3Int> candidates, Vector3Int currentPosition, ChessPieceType id, bool hasMoved)
    {
        var forward = (id == ChessPieceType.Player1Pawn) ? -1 : 1;

        // Single step forward
        var oneStep = new Vector3Int(currentPosition.x + forward, currentPosition.y, 0);
        if (IsInsideBoard(oneStep))
            candidates.Add(oneStep);

        // Double step forward (only if not moved)
        var twoStep = new Vector3Int(currentPosition.x + 2 * forward, currentPosition.y, 0);
        if (!hasMoved && IsInsideBoard(twoStep))
            candidates.Add(twoStep);

        // Capture diagonals
        var diagLeft  = new Vector3Int(currentPosition.x + forward, currentPosition.y - 1, 0);
        var diagRight = new Vector3Int(currentPosition.x + forward, currentPosition.y + 1, 0);

        if (IsInsideBoard(diagLeft))
            candidates.Add(diagLeft);

        if (IsInsideBoard(diagRight))
            candidates.Add(diagRight);
    }

    private void AddKingMoves(List<Vector3Int> candidates, Vector3Int pos)
    {
        Vector2Int[] directions = {
            new Vector2Int(1,0), new Vector2Int(-1,0), new Vector2Int(0,1), new Vector2Int(0,-1),
            new Vector2Int(1,1), new Vector2Int(1,-1), new Vector2Int(-1,1), new Vector2Int(-1,-1)
        };

        foreach (var j in directions)
            OutOfBounds(candidates, pos.x + j.x, pos.y + j.y);
    }

    private void AddKnightMoves(List<Vector3Int> candidates, Vector3Int pos)
    {
        Vector2Int[] jumps = {
            new Vector2Int(2,1), new Vector2Int(2,-1), new Vector2Int(-2,1), new Vector2Int(-2,-1),
            new Vector2Int(1,2), new Vector2Int(1,-2), new Vector2Int(-1,2), new Vector2Int(-1,-2)
        };

        foreach (var j in jumps)        
            OutOfBounds(candidates, pos.x + j.x, pos.y + j.y);
    }
    
        
    
    //the piece can slide across the board
    private void AddSlidingMoves(List<Vector3Int> candidates, Vector3Int pos, Vector2Int[] directions)
    {
        foreach (var dir in directions)
        {
            var x = pos.x;
            var y = pos.y;
            while (true)
            {
                //here we will move along each direction || ex: forward --> (1,0) =--> (2,0) , until out of bounds
                x += dir.x;
                y += dir.y;
                if (!OutofBounds(candidates, x, y)) break; // stop if we go out of bounds
            }
        }
    }
    private bool IsOutOfBounds(Vector3Int destination)
    {
        if (destination.x is < -6 or > 1)
            return true;
        if(destination.y is >7 or <0)
            return true;
        return false; 
    }
    
    private void OutOfBounds(List<Vector3Int> list, int x, int y)
    {
        if (x >= minX && x <= maxX && y >= minY && y <= maxY)
        {
            list.Add(new Vector3Int(x, y, 0));
        }
    }
    private bool OutofBounds(List<Vector3Int> list, int x, int y)
    {
        if (x >= minX && x <= maxX && y >= minY && y <= maxY)
        {
            list.Add(new Vector3Int(x, y, 0));
            return true;
        }
        return false;
    }
    private bool IsInsideBoard(Vector3Int pos)
    {
        return pos.x >= -6 && pos.x <= 1 &&
               pos.y >= 0 && pos.y <= 7;
    }

}
