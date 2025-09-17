using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Identity
{
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}

public enum Team
{
    Black,  
    White
}


public class Piece
{
    public Identity Type { get; }
    public Team Team { get; }
    public Piece(Identity type, Team team)
    {
        Type = type;
        Team = team;
    }
}
