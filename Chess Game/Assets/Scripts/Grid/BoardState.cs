using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;

public class BoardState : MonoBehaviour
{
   public Dictionary<Vector3Int, Piece> GridPositions = new Dictionary<Vector3Int, Piece>();
   private Pawn _pawnInstance;
   private ChessPieceType _pieceSelected;
   private AllValidMoves _allValidMoves;
   private bool _isMaximizer;
   private EvaluateBoard _evaluateBoard;
   private Vector3Int _from;
   private Vector3Int _to;
   private Piece _piece;
   private void Start()
   {
       SetGridPositions();
   }
   private void SetGridPositions()
   {
       //black pieces
       for (int i = 0; i < 8; i++)
       {
           GridPositions[new Vector3Int(0, i, 0)] = new Piece(Identity.Pawn, Team.Black);
       }
       GridPositions[new Vector3Int(1, 0, 0)] = new Piece(Identity.Rook, Team.Black);
       GridPositions[new Vector3Int(1, 1, 0)] = new Piece(Identity.Knight, Team.Black);
       GridPositions[new Vector3Int(1, 2, 0)] = new Piece(Identity.Bishop, Team.Black);
       GridPositions[new Vector3Int(1, 3, 0)] = new Piece(Identity.Queen, Team.Black);
       GridPositions[new Vector3Int(1, 4, 0)] = new Piece(Identity.King, Team.Black);
       GridPositions[new Vector3Int(1, 5, 0)] = new Piece(Identity.Bishop, Team.Black);
       GridPositions[new Vector3Int(1, 6, 0)] = new Piece(Identity.Knight, Team.Black);
       GridPositions[new Vector3Int(1, 7, 0)] = new Piece(Identity.Rook, Team.Black);
       
       //white pieces
       for (int i = 0; i < 8; i++)
       {
           GridPositions[new Vector3Int(-5, i, 0)] = new Piece(Identity.Pawn, Team.White);
       }
       GridPositions[new Vector3Int(-6, 0, 0)] = new Piece(Identity.Rook, Team.White);
       GridPositions[new Vector3Int(-6, 1, 0)] = new Piece(Identity.Knight, Team.White);
       GridPositions[new Vector3Int(-6, 2, 0)] = new Piece(Identity.Bishop, Team.White);
       GridPositions[new Vector3Int(-6, 3, 0)] = new Piece(Identity.Queen, Team.White);
       GridPositions[new Vector3Int(-6, 4, 0)] = new Piece(Identity.King, Team.White);
       GridPositions[new Vector3Int(-6, 5, 0)] = new Piece(Identity.Bishop, Team.White);
       GridPositions[new Vector3Int(-6, 6, 0)] = new Piece(Identity.Knight, Team.White);
       GridPositions[new Vector3Int(-6, 7, 0)] = new Piece(Identity.Rook, Team.White);
      
   }
   
   //after the human makes a turn the AI needs to get the current state of the board
   public void UpdateBoardStateAfterHumanTurn()
   {
       GridPositions.Clear();
       AddPiecesToBoard(_pawnInstance.Team1, Team.White);
       AddPiecesToBoard(_pawnInstance.Team2, Team.Black);
   }

   //only add active pieces to board
   private void AddPiecesToBoard(IEnumerable<GameObject> pieces, Team team)
   {
       foreach (var pieceGO in pieces)
       {
           if (!pieceGO.activeInHierarchy)
               continue;
           var position = _pawnInstance.GetGridPosition(pieceGO);
           var pieceType = pieceGO.GetComponent<PieceIdentity>().pieceType; // returns Identity enum
           GridPositions[position] = Convert(pieceType);
       }
   }
   //when we first call minimax we will pass the global board dictionary
   public int MiniMax(Dictionary<Vector3Int, Piece> currentBoard, int depth, bool isMaximizer, bool storeMove)
   {
       if(depth == 0)
           return _evaluateBoard.GetBoardScore(currentBoard);
       
       if (isMaximizer)
       {
           var maxVal = -10000;
           var wasAMoveFound = false;
           //check each piece in the board
           foreach (var kvp in currentBoard)
           {
               if (kvp.Value.Team == Team.White)
               {
                   //get all valid moves for current piece
                   var list = _allValidMoves.GetCandidates(kvp.Value, kvp.Key,true);
                   foreach (var position in list)
                   {    //can the piece reach the possible square? (here is where we create the new board dictionary)
                       if (IsValidPosition(kvp.Key, position, currentBoard) && !WillMovePlaceUsInCheck(kvp.Key, position, currentBoard, true))
                       {
                           wasAMoveFound = true;
                           var newBoard = new Dictionary<Vector3Int, Piece>();
                           //deep copy (if for the valid move)
                           foreach (var kvp1 in currentBoard) 
                               newBoard[kvp1.Key] = kvp1.Value.Clone();
                           var movedPiece = kvp.Value.Clone();
                           newBoard.Remove(kvp.Key);
                           newBoard[position] = movedPiece;
                           var score = MiniMax(newBoard, depth - 1, false, false); // a value will only be returned when we reach the required depth
                           if (score > maxVal)
                           {
                               maxVal = score;
                               //here we store the move that was taken 
                               if (storeMove)
                               { _from = kvp.Key; _to = position; _piece = kvp.Value; }
                           }
                       }
                   }
               }
           }
            //this board state generated no valid moves !
           if (!wasAMoveFound)
           {
               return CheckMate(currentBoard, true) ? 10000 : 0;
           }

           return maxVal;
       }
       else
       {
           var minVal = 10000;
           var wasAMoveFound = false;
           foreach (var kvp in currentBoard)
           {
               if (kvp.Value.Team == Team.Black)
               {
                   var list = _allValidMoves.GetCandidates(kvp.Value, kvp.Key,true);
                   foreach (var position in list)
                   {    //can the piece reach the square? (here is where we create the new board dictionary)
                       if (IsValidPosition(kvp.Key, position, currentBoard) && !WillMovePlaceUsInCheck(kvp.Key, position, currentBoard, false))
                       {
                           wasAMoveFound = true; 
                           var newBoard = new Dictionary<Vector3Int, Piece>();
                           //deep copy (if for the valid move)
                           foreach (var kvp1 in currentBoard) 
                               newBoard[kvp1.Key] = kvp1.Value.Clone();
                           var movedPiece = kvp.Value.Clone();
                           newBoard.Remove(kvp.Key);
                           newBoard[position] = movedPiece;
                           var score = MiniMax(newBoard, depth - 1, true, false); // a value will only be returned when we reach the required depth
                           minVal = Math.Min(minVal, score);
                       }
                   }
               }
           }
           if (!wasAMoveFound)
           {
               return CheckMate(currentBoard, true) ? 10000 : 0;
           }
           return minVal;
       }
   }
   
   //here will commit the chosen move from minimax
   public Dictionary<Vector3Int, Piece> CommitMove(Vector3Int from, Vector3Int to, Piece piece, Dictionary<Vector3Int, Piece> currentBoard)
   {
       currentBoard.Remove(from);
       currentBoard[to] = piece;
       return currentBoard;
   }

   //this for move validation (might not need this now)
   private bool IsValidPosition(Vector3Int currentPosition, Vector3Int destinationPosition, Dictionary<Vector3Int, Piece> currentBoard)
    { 
        if (IsOutOfBounds(destinationPosition))
            return false;
        // what is the difference between destination and current piece position
        var dx = destinationPosition.x - currentPosition.x;
        var dy = destinationPosition.y - currentPosition.y;
        var pieceId = currentBoard[currentPosition].Type;
        switch (pieceId)
        {
            // PAWN 
            case Identity.Pawn:
               if(currentBoard[currentPosition].Team == Team.Black) 
               {
                   if (dy == 0 && dx == -1 && !currentBoard.ContainsKey(destinationPosition)) return true; // single forward and check if empty
                   //if (dy == 0 && dx == -2 && isThisMovingThePiece && currentBoard[destinationPosition].Team == Team.White) return true; // double move forward + location empty (need to make sure that it cannot do this after first move) *MUST FIX*
                   if (Mathf.Abs(dy) == 1 && dx == -1 && currentBoard[destinationPosition].Team == Team.White) return true; // diagonal capture if there is an enemy piece there
                   return false;
               }
               if (dy == 0 && dx == 1 && !currentBoard.ContainsKey(destinationPosition)) return true; // single forward and check if empty
               //if (dy == 0 && dx == 2 && isThisMovingThePiece && currentBoard[destinationPosition].Team == Team.Black) this is the double move that needs to be fixed later
                   //return true; // double forward (first move)
               if (Mathf.Abs(dy) == 1 && dx == 1 && currentBoard[destinationPosition].Team == Team.Black)
                   return true; // diagonal capture
               return false;

            //ROOK 
            case Identity.Rook:
            {
                if (dx != 0 && dy != 0) return false; // check if the destination is horizontal/vertical, if not return false
                if (PathIsBlocked(currentPosition, destinationPosition, currentBoard)) return false;
                return true;
            }

            //BISHOP
            case Identity.Bishop:
            {
                if (Mathf.Abs(dx) != Mathf.Abs(dy)) return false; // must be diagonal
                if (PathIsBlocked(currentPosition, destinationPosition,currentBoard)) return false;
                return true;
            }

            // QUEEN
            case Identity.Queen:
            {
                var isStraight = (dx == 0 || dy == 0);
                var isDiagonal = Mathf.Abs(dx) == Mathf.Abs(dy);
                if (!isStraight && !isDiagonal) return false;
                if (PathIsBlocked(currentPosition, destinationPosition, currentBoard)) return false;
                return true;
            }

            // KNIGHT
            case Identity.Knight:
            {
                return (Mathf.Abs(dx) == 2 && Mathf.Abs(dy) == 1) || (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 2);
            }

            // KING
            case Identity.King:
            {
                return Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) == 1 && !currentBoard.ContainsKey(destinationPosition) ;
            }
            
            default:
                return false;
        }
}

    private static bool PathIsBlocked(Vector3Int currentPosition, Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard)
    {
        //will let us know what direction we have to iterate towards
        var dx = Math.Sign(destination.x - currentPosition.x);
        var dy = Math.Sign(destination.y - currentPosition.y);

        // move one square at a time until just before the destination
        var x = currentPosition.x + dx;
        var y = currentPosition.y + dy;
    
        //stop the loop if we reach the destination position
        while (x != destination.x || y != destination.y)
        {
            var pos = new Vector3Int(x, y, 0);
            //we have to check if the position exists on the board (both team and enemy will block a path)
            if (currentBoard.ContainsKey(pos))
            {
                return true; // we are being blocked
            }
            x += dx;
            y += dy;
        }
        return false; // no obstacles
    }

    public bool WillMovePlaceUsInCheck(Vector3Int currentPosition, Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard, bool isMaximizer)
    {
        //let's get the piece we are trying to move
        if (!currentBoard.TryGetValue(currentPosition, out Piece movingPiece))
            return false;
        var kingPosition = GetKingPosition(currentBoard,isMaximizer);
        //could fix this below idk 
        if (movingPiece.Type == Identity.King)
            kingPosition = destination;
        // Create a copy of the current board
        var newBoard = new Dictionary<Vector3Int, Piece>(currentBoard);
        // Apply the move on the copy and remove the old position
        newBoard[destination] = movingPiece;
        newBoard.Remove(currentPosition);
        return IsGridUnderAttack(kingPosition,newBoard);
    }   

    
    //check if a piece is under attack
    public bool IsGridUnderAttack(Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard)
    {
        // see if the destination exists
        if (!currentBoard.TryGetValue(destination, out var targetPiece))
            targetPiece = null;
        // Look through enemy pieces
        foreach (var kvp in currentBoard)
        {   
            Piece attacker = kvp.Value;

            // don't check friendly pieces
            if (targetPiece != null && attacker.Team == targetPiece.Team)
                continue;
            if (CanPieceReachSquare(kvp.Key, destination, currentBoard))
            {
                Debug.Log(targetPiece.Team);
                Debug.Log(destination);
                Debug.Log("This has us in check" + kvp.Key + kvp.Value.Team + kvp.Value.Type);
                return true; //square is under attack
            }
            
        }

        return false; //we are safe
    }


///////////////////helper functions///////////////////////
    private bool CanPieceReachSquare(Vector3Int piecePosition, Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard)
    {
        var pieceID= currentBoard[piecePosition].Type;
        switch (pieceID)
        {
            case Identity.Pawn:
                return CanPawnReachSquare(piecePosition, destination,currentBoard);
            case Identity.Knight:
                return CanKnightReachSquare(piecePosition, destination, currentBoard);
            case Identity.Rook:
                return CanRookReachSquare(piecePosition, destination, currentBoard);
            case Identity.Bishop:
                return CanBishopReachSquare(piecePosition, destination, currentBoard);
            case Identity.Queen:
                return CanQueenReachSquare(piecePosition, destination, currentBoard);
            case Identity.King:
                return CanKingReachSquare(piecePosition, destination, currentBoard);
        }
    
        return false; 
    }

    private bool CheckMate(Dictionary<Vector3Int, Piece> currentBoard, bool isMaximizer)
    {
        var kingPosition = GetKingPosition(currentBoard,isMaximizer);
        if (!IsGridUnderAttack(kingPosition, currentBoard))
            return false;
        if (WillPieceRemoveCheck(kingPosition, currentBoard, isMaximizer))
            return false;
        return true;
    }

    private bool WillPieceRemoveCheck(Vector3Int kingPosition, Dictionary<Vector3Int, Piece> currentBoard, bool isMaximizer)
    { 
        //here is a list of all possible moves
        var allMoves = GetAllPossibleMoveForTeam(currentBoard, isMaximizer);
        //Check to see if a friendly piece
        //can reach the candidate and if
        //it saves the king
        if (isMaximizer)
        {
            foreach (var keyValue in currentBoard)
            {
                if (keyValue.Value.Team == Team.White)
                {
                    foreach (var move in allMoves)
                    {
                        if (!CanPieceReachSquare(keyValue.Key, move, currentBoard))
                            continue;
                        // Create a copy of the current board
                        var newBoard = new Dictionary<Vector3Int, Piece>(currentBoard);
                        // Apply the move on the copy
                        newBoard[move] = newBoard[keyValue.Key];  // move piece to new square
                        newBoard.Remove(keyValue.Key);
                        if (!IsGridUnderAttack(kingPosition, newBoard))
                        {
                            return true;
                        }

                    }
                }
            }
        }
        else
        {
            foreach (var keyValue in currentBoard)
            {
                if (keyValue.Value.Team == Team.Black)
                {
                    foreach (var move in allMoves)
                    {
                        if (!CanPieceReachSquare(keyValue.Key, move, currentBoard))
                            continue;
                        // Create a copy of the current board
                        var newBoard = new Dictionary<Vector3Int, Piece>(currentBoard);
                        // Apply the move on the copy
                        newBoard[move] = newBoard[keyValue.Key];  // move piece to new square
                        newBoard.Remove(keyValue.Key);
                        if (!IsGridUnderAttack(kingPosition, newBoard))
                        {
                            return true;
                        }

                    }
                }
            }
        }
        return false; 
    }
    
    private List<Vector3Int> GetAllPossibleMoveForTeam(Dictionary<Vector3Int, Piece> currentBoard, bool isMaximizer)
    {
        var possibleMoves = new List<Vector3Int>();
        if (isMaximizer)
        {
            foreach (var piece in currentBoard)
            {
                if(piece.Value.Team == Team.Black)
                    possibleMoves.AddRange(_allValidMoves.GetCandidates(piece.Value, piece.Key,true));
            }
        }
        else
        {
            foreach (var piece in currentBoard)
            {
                if(piece.Value.Team == Team.White)
                    possibleMoves.AddRange(_allValidMoves.GetCandidates(piece.Value, piece.Key,true));
            }
        }
        return possibleMoves;
    }
    

    private bool CanPawnReachSquare(Vector3Int pawnPosition, Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard)
{
    if (IsOutOfBounds(destination))
        return false;
    //make sure that we are not capturing a friendly piece
    if (currentBoard.TryGetValue(destination, out Piece destPiece))
    {
        if (destPiece.Team == currentBoard[pawnPosition].Team)
            return false; // can't capture your own piece
    }
    var dx = destination.x - pawnPosition.x;
    var dy = destination.y - pawnPosition.y;
    var id = currentBoard[pawnPosition].Type;
    switch (id)
    {
        case Identity.Pawn:
            if (currentBoard[pawnPosition].Team == Team.Black)
            {
                if (dy == 0 && dx == -1 && !currentBoard.ContainsKey(destination)) return true; // single forward
                //if (dy == 0 && dx == -2 && destinationID == ID.None)
                    //return true; // double move forward + location empty (need to make sure that it cannot do this after first move) *MUST FIX*
                if (Mathf.Abs(dy) == 1 && dx == -1)
                    return true; // diagonal capture if there is an enemy piece there
                break;
            }
            if (dy == 0 && dx == 1 && !currentBoard.ContainsKey(destination)) return true; // single forward
            //if (dy == 0 && dx == 2 && destinationID == ID.None) return true; // double forward (first move)
            if (Mathf.Abs(dy) == 1 && dx == 1 && currentBoard[destination].Team != currentBoard[pawnPosition].Team) return true; // diagonal capture
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }
    return false;
}

private bool CanRookReachSquare(Vector3Int rookPosition, Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard)
{
    if (IsOutOfBounds(destination))
        return false;
    if (currentBoard.TryGetValue(destination, out Piece destPiece))
    {
        if (destPiece.Team == currentBoard[rookPosition].Team)
            return false; // can't capture your own piece
    }
    var dx = destination.x - rookPosition.x;
    var dy = destination.y - rookPosition.y;
    //make sure that at least one of the x/y stays the same if not we are not moving correctly
    if (dx != 0 && dy != 0) 
        return false;
    if(PathIsBlocked(rookPosition, destination, currentBoard))
        return false;
    return true;
}

private bool CanKnightReachSquare(Vector3Int kingPosition, Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard)
{
    if (IsOutOfBounds(destination))
        return false;
    if (currentBoard.TryGetValue(destination, out Piece destPiece))
    {
        if (destPiece.Team == currentBoard[kingPosition].Team)
            return false; // can't capture your own piece
    }
    var dx = destination.x - kingPosition.x;
    var dy = destination.y - kingPosition.y;

    if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 2 || Mathf.Abs(dx) == 2 && Mathf.Abs(dy) == 1)
        return true;
    return false; 
}

private bool CanBishopReachSquare(Vector3Int bishopPosition, Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard)
{
    if (IsOutOfBounds(destination))
        return false; 
    if (currentBoard.TryGetValue(destination, out Piece destPiece))
    {
        if (destPiece.Team == currentBoard[bishopPosition].Team)
            return false; // can't capture your own piece
    }
    var dx = destination.x - bishopPosition.x;
    var dy = destination.y - bishopPosition.y;
    //difference must be the same for them to be moving diagonally 
    if (Mathf.Abs(dx) != Mathf.Abs(dy))
        return false; 
    if(PathIsBlocked(bishopPosition, destination, currentBoard))
        return false;
    return true; 
}

private bool CanQueenReachSquare(Vector3Int queenPosition, Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard)
{
    if (IsOutOfBounds(destination))
        return false;
    if (currentBoard.TryGetValue(destination, out Piece destPiece))
    {
        if (destPiece.Team == currentBoard[queenPosition].Team)
            return false; // can't capture your own piece
    }
    var dx = destination.x - queenPosition.x;
    var dy = destination.y - queenPosition.y;
    if(PathIsBlocked(queenPosition, destination, currentBoard))
        return false;
    //not diagonal or not vertical/horizontal
    if (!(dx == 0 || dy == 0 || Mathf.Abs(dx) == Mathf.Abs(dy)))
        return false;
    return true; 
}


private bool CanKingReachSquare(Vector3Int kingPosition, Vector3Int destination, Dictionary<Vector3Int, Piece> currentBoard)
{
    //////////////MIGHT NEED TO FIX (LAST IF) ///////////////////////
    if (IsOutOfBounds(destination))
        return false;
    if (currentBoard.TryGetValue(destination, out Piece destPiece))
    {
        if (destPiece.Team == currentBoard[kingPosition].Team)
            return false; // can't capture your own piece
    }
    var dx = destination.x - kingPosition.x;
    var dy = destination.y - kingPosition.y;
    if (Mathf.Abs(dx) == 1 && Mathf.Abs(dy)==0 || Mathf.Abs(dx) == 0 && Mathf.Abs(dy) == 1 || Mathf.Abs(dx) == 1 && Mathf.Abs(dy) == 1)
        return true;
    return false; 
}

private Vector3Int GetKingPosition(Dictionary<Vector3Int, Piece> currentBoard, bool isMaximizer)
{
    Vector3Int destination = new Vector3Int();
    if (isMaximizer)
    {
        foreach (var piece in currentBoard)
        {
            if (piece.Value.Team == Team.White && piece.Value.Type == Identity.King)
            {
                destination = piece.Key;
            }
        }
    }
    else
    {
        foreach (var piece in currentBoard)
        {
            if (piece.Value.Team == Team.Black && piece.Value.Type == Identity.King)
            {
                destination = piece.Key;
            }
        }
    }
    return destination;
}

private bool IsOutOfBounds(Vector3Int destination)
{
    if (destination.x is < -6 or > 1)
        return true;
    if(destination.y is >7 or <0)
        return true;
    return false; 
}
public static Piece Convert(ChessPieceType oldType)
{
    switch (oldType)
    {
        case ChessPieceType.Player1Pawn:   return new Piece(Identity.Pawn, Team.Black);
        case ChessPieceType.Player1Rook:   return new Piece(Identity.Rook, Team.Black);
        case ChessPieceType.Player1Knight: return new Piece(Identity.Knight, Team.Black);
        case ChessPieceType.Player1Bishop: return new Piece(Identity.Bishop, Team.Black);
        case ChessPieceType.Player1Queen:  return new Piece(Identity.Queen, Team.Black);
        case ChessPieceType.Player1King:   return new Piece(Identity.King, Team.Black);

        case ChessPieceType.Player2Pawn:   return new Piece(Identity.Pawn, Team.White);
        case ChessPieceType.Player2Rook:   return new Piece(Identity.Rook, Team.White);
        case ChessPieceType.Player2Knight: return new Piece(Identity.Knight, Team.White);
        case ChessPieceType.Player2Bishop: return new Piece(Identity.Bishop, Team.White);
        case ChessPieceType.Player2Queen:  return new Piece(Identity.Queen, Team.White);
        case ChessPieceType.Player2King:   return new Piece(Identity.King, Team.White);

        default: return null;
    }
}

public Identity? Convert2(ChessPieceType oldType)
{
    switch (oldType)
    {
        case ChessPieceType.Player1Pawn:   return Identity.Pawn;
        case ChessPieceType.Player1Rook:   return Identity.Rook;
        case ChessPieceType.Player1Knight: return Identity.Knight;
        case ChessPieceType.Player1Bishop: return Identity.Bishop;
        case ChessPieceType.Player1Queen:  return Identity.Queen;
        case ChessPieceType.Player1King:   return Identity.King;

        case ChessPieceType.Player2Pawn:   return Identity.Pawn;
        case ChessPieceType.Player2Rook:   return Identity.Rook;
        case ChessPieceType.Player2Knight: return Identity.Knight;
        case ChessPieceType.Player2Bishop: return Identity.Bishop;
        case ChessPieceType.Player2Queen:  return Identity.Queen;
        case ChessPieceType.Player2King:   return Identity.King;

        default: return null;
    }
}

}



