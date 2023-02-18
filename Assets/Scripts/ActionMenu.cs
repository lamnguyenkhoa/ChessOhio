using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActionMenu : MonoBehaviour
{
    public ChessPiece chessPiece;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                GameManager.instance.CloseActionMenu();
            }
        }
    }

    public void Setup(ChessPiece chessPiece)
    {
        this.chessPiece = chessPiece;
        if (chessPiece == null) return;
        transform.Find("PieceName").GetComponent<TextMeshProUGUI>().text = chessPiece.profile.pieceName;
    }

    private void OnDisable()
    {

    }

    public void OnInvertButton()
    {
        if (chessPiece.team == GameManager.instance.teamTurn.Value)
        {
            SpecialActionHandler.instance.TransformPiece(chessPiece, PieceType.NIGHTRIDER);
            GameManager.instance.CloseActionMenu();
        }
    }
}
