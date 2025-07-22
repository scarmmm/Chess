using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private GameObject _pawnPrefab;
    [SerializeField] private GameObject _chessboard;
    [SerializeField] private Vector3 pawnScale = new Vector3(1, 1, 1); // Scale for the pawns

    private GameObject[,] _piecesOnGrid = new GameObject[8, 8]; // 8x8 grid for storing pieces

    private void Start()
    {
       // PlacePawnRow(1); // Place pawns in row 1

        //var worldPosition = _grid.GetCellCenterWorld(new Vector3Int(0, 0, 0)); // Get the world position of the cell at (0, 0)
        //var worldPosition1 = _grid.GetCellCenterWorld(new Vector3Int(0, 1, 0));
        //var worldPosition2 = _grid.GetCellCenterWorld(new Vector3Int(1, 0, 0));
        //Instantiate(_pawnPrefab, worldPosition, Quaternion.identity); // instantiate a pawn at the world position
        //Instantiate(_pawnPrefab, worldPosition1, Quaternion.identity); 
        //Instantiate(_pawnPrefab, worldPosition2, Quaternion.identity);

        // Example: Access the piece at position (3, 1)
        GameObject specificPiece = GetPieceAtGridPosition(3, 1);
    }

    // Method to place a row of pawns
    private void PlacePawnRow(int row)
    {
        for (int col = 0; col < 8; col++) // Loop through columns 0 to 7
        {
            Vector3Int gridPosition = new Vector3Int(col, row, 0); // Grid coordinates for the current column in the row

            var worldPosition = _grid.GetCellCenterWorld(gridPosition);// Get the world position of the current grid cell

            GameObject instantiatedPawn = Instantiate(_pawnPrefab, worldPosition, Quaternion.identity);// spawn the pawn at this position

           
            instantiatedPawn.transform.localScale = pawnScale; // scale the pawn 

            
            _piecesOnGrid[col, row] = instantiatedPawn;// store the pawn in the 2D array
        }
    }

    private GameObject GetPieceAtGridPosition(int col, int row)
    {
   
        return _piecesOnGrid[col, row];// Return the piece at the specified grid position from the 2D array
    }
}

