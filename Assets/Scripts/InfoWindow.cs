using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    public ChessPiece piece;
    public TextMeshProUGUI pieceName;
    public TextMeshProUGUI pieceDescription;
    [SerializeField] private LocalizeStringEvent localizedPieceStat;
    public Image pieceImage;

    void Start()
    {
    }

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
        localizedPieceStat.StringReference.Arguments = new object[] { piece.GetStatCapturedNumber(), piece.GetStatTurnMoved() };
        localizedPieceStat.RefreshString();
        pieceImage.sprite = profile.sprite;
    }

    public void OnCloseButton()
    {
        CloseInfoWindow();
    }

}
