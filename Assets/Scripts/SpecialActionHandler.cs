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
        Chessboard.instance.ChangePiece(new Vector2Int(piece.currentX, piece.currentY), type, true);
        Chessboard.instance.EndTurn(true);
    }
}
