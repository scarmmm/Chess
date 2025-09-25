using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// New script, attach to GameManager or InputHandler object
public class InputHandler : MonoBehaviour
{
    [SerializeField] private Grid _grid;
    [SerializeField] private LayerMask chessboardMask;
    [SerializeField] private Pawn pawn;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, chessboardMask)) return;
        Vector3 worldMousePosition = hit.point;
        Vector3Int gridPosition = _grid.WorldToCell(worldMousePosition);
        pawn.OnSquareClicked(hit, gridPosition);
    }
}

