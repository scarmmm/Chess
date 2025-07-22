using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UI_Promotion : MonoBehaviour
{
    public enum PromotedPiece { None, Queen, Rook, Bishop, Knight }

    private System.Action<PromotedPiece> onPieceSelected;

    public void Setup(System.Action<PromotedPiece> onSelectCallback)
    {
        gameObject.SetActive(true);
        onPieceSelected = onSelectCallback;
    }

    public void SelectPiece(int index)
    {
        PromotedPiece selected = (PromotedPiece)index;
        onPieceSelected?.Invoke(selected);
        gameObject.SetActive(false);
    }

    public void Clean()
    {
        onPieceSelected = null;
        gameObject.SetActive(false);
    }
}
