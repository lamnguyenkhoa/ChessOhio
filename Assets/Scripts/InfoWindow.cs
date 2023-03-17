using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    public ChessPiece piece;
    public TextMeshProUGUI pieceName;
    public TextMeshProUGUI pieceDescription;
    public TextMeshProUGUI pieceStat;
    public Image pieceImage;

    public void ShowInfoWindow()
    {
        RefreshData();
        this.gameObject.SetActive(true);
    }

    public void CloseInfoWindow()
    {
        this.gameObject.SetActive(false);
    }

    public void RefreshData()
    {
        if (!piece)
            return;
        ChessPieceProfileSO profile = piece.profile;
        pieceName.text = profile.pieceName;
        pieceDescription.text = profile.description;
        pieceStat.text = $"Captured: {piece.GetStatCapturedNumber()} | Moved: {piece.GetStatTurnMoved()}";
        pieceImage.sprite = profile.sprite;
    }

    public void OnCloseButton()
    {
        CloseInfoWindow();
    }

}
