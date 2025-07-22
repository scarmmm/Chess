using System;
using UnityEngine;
using TMPro; // Ensure you have this if using TextMeshPro

public class UI_SelectedPiece : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pieceText;

    private void Start()
    {
        ClearSelectedPiece();
    }

    public void SetPieceName(string text)
    {
        if (pieceText != null)
        {
            pieceText.text = text;
        }
    }

    public void ClearSelectedPiece()
    {
        SetPieceName("Selected: None");
    }
}
