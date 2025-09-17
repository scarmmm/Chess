using UnityEngine;


struct MoveRecord
{
    public Vector3Int from; 
    public Vector3Int to;

    public MoveRecord(Vector3Int from, Vector3Int to)
    {
        this.from = from;
        this.to = to;
    }
}