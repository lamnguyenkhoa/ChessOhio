using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
