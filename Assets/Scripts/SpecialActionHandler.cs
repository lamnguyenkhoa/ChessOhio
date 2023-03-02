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
        Chessboard.instance.ChangePiece(new Vector2Int(piece.currentX, piece.currentY), type, true);
        Chessboard.instance.EndTurn(true);
    }
}
