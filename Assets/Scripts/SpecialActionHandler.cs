using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Action in ActionMenu, different from SpecialMove. Think of it like
/// active vs passive skill.
/// </summary>
public enum SpecialAction
{
    NONE = 0,
    INVERT = 1,
    COMBINE = 2,
    SPECIAL_PROMO = 3,
}

public class SpecialActionHandler : MonoBehaviour
{
    public static SpecialActionHandler instance;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public void TransformPiece(ChessPiece piece, PieceType type)
    {
        //Since it has the same function, I re-use it
        SpecialMoveHandler.instance.PromotePiece(piece, type);

        GameManager.instance.SwitchTurnServerRpc();
        if (!Chessboard.instance.isLocalGame)
        {
            GameManager.instance.NotifyChangePiece(new Vector2Int(piece.currentX, piece.currentY), type);
        }
    }
}
