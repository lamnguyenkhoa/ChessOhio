using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : MonoBehaviour
{
    public ChessPieceProfileSO profile;
    public TextMeshProUGUI pieceName;
    public TextMeshProUGUI pieceDescription;
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
        if (!profile)
            return;
        pieceName.text = profile.pieceName;
        pieceDescription.text = profile.description;
        pieceImage.sprite = profile.sprite;
    }

    public void OnCloseButton()
    {
        CloseInfoWindow();
    }

}
