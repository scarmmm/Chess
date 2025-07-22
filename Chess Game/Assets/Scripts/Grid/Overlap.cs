using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlap : MonoBehaviour
{
    //this was a bad idea ( I know a better solution now )
    Pawn pawn;
    private void Start()
    {
        pawn = FindObjectOfType<Pawn>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ChessBoard"))
        {
           // Debug.Log("Chessboard");    
            return;
        }
       
        if (pawn == null || pawn.lastSelectedPiece == null)
        {
            Debug.Log("Pawn or last selected piece is null");
            return;
        }

        // HERE IS THE KEY
        // THIS WILL TERMINATE THE FUNCTION OF THE OTHER PIECE SINCE IT WAS NOT THE LAST SELECTED PIECE
        if (gameObject != pawn.lastSelectedPiece)
        {
            Debug.Log("Function terminated");
            return; 
        }

        if (other.gameObject == pawn.lastSelectedPiece)
        {
            return;
        }

        // Check if the other object's tag is NOT the same as this object's tag
        if (!other.CompareTag(gameObject.tag) && !other.CompareTag("ChessBoard"))
        {
            //other.gameObject.tag = pawn.eliminatedPlayer;
            //Destroy(other.gameObject, 0.1f);
            other.gameObject.SetActive(false);
            // Move the selected piece to the position of the destroyed piece
            Vector3 otherPiecePos = other.gameObject.transform.position;
            float posy = gameObject.transform.position.y;
            gameObject.transform.position = otherPiecePos;
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, posy, gameObject.transform.position.z);

        }
        else
        {
            Debug.Log($"Ignored overlap with {other.name} because it has the same tag: {gameObject.tag}");
        }
    }


    private void OnTriggerStay(Collider other)
    {
        // Continue checking for differences in tags
        if (!other.CompareTag(gameObject.tag))
        {
      
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(gameObject.tag))
        {
           
        }
    }
}
