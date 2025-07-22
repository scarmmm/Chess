using UnityEngine;
using UnityEngine.Tilemaps;

public class LimitTilePlacement : MonoBehaviour
{
    public Tilemap tilemap;  // Reference to the Tilemap
    public TileBase tileToPlace;  // The tile to place
    public Vector2Int gridSize = new Vector2Int(8, 8);  // The size of the grid (8x8)

    void Start()
    {
        // Automatically get the Tilemap component if not manually set in the Inspector
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }
    }

    public void PlaceTile(Vector3Int position)
    {
        // Check if the position is within the 8x8 grid bounds
        if (position.x >= 0 && position.x < gridSize.x && position.y >= 0 && position.y < gridSize.y)
        {
            tilemap.SetTile(position, tileToPlace);  // Place the tile if it's within bounds
        }
        else
        {
            Debug.Log("Position outside of grid bounds!");
            Debug.Log("X: " + position.x + ", Y: " + position.y);
        }
    }

    void Update()
    {
        // If left mouse button is clicked
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int gridPosition = tilemap.WorldToCell(mouseWorldPos);  // Convert world position to grid coordinates

            // Attempt to place the tile at the clicked position
            PlaceTile(gridPosition);
        }
    }
}


