using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ChessPieceProfileSO profile;

    public void Setup(ChessPieceProfileSO profile)
    {
        this.profile = profile;
        if (profile == null) return;
        transform.Find("PieceName").GetComponent<TextMeshProUGUI>().text = profile.pieceName;
    }

    private void OnDisable()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Chessboard.instance.disableRaycast = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Chessboard.instance.disableRaycast = false;
    }
}
