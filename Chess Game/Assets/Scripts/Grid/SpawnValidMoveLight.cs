using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnValidMoveLight : MonoBehaviour
{
    [SerializeField] GameObject piecePrefab;
    [SerializeField] private Pawn _pawn;
    // Start is called before the first frame update
    [SerializeField] private List<GameObject> _gridPieces;
    public void SpawnGrid(Vector3Int position)
    {
        Vector3 worldPos = new Vector3(position.x, -0.6f, position.z); 
        var gameObject = Instantiate(piecePrefab, worldPos, Quaternion.identity);
        _gridPieces.Add(gameObject);
        gameObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        _pawn.MoveToCenterOfGrid(position, gameObject);
        gameObject.transform.position += new Vector3(0f, -0.5f, 0f); 

    }

    public void DestroyGrid()
    {
        if (_gridPieces.Count == 0)
            return;
        foreach (var gridPiece in _gridPieces)
            Destroy(gridPiece);
        _gridPieces.Clear();
    }
}
