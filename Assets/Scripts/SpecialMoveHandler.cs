using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SpecialMove, which is different from SpecialAction. Usually resolved
/// at the end of turn.
/// </summary>
public enum SpecialMove
{
    NONE = 0,
    EN_PASSANT = 1,
    CASTLING = 2,
    PROMOTION = 3
}

public class SpecialMoveHandler : MonoBehaviour
{
    public static SpecialMoveHandler instance;
    public PieceType chosenPiecePromo = PieceType.NONE;
    public GameObject promotionScreen;

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

    public bool ProcessSpecialMoves(ref List<Vector2Int[]> moveList, ref List<SpecialMove> specialMoves, ref ChessPiece[,] chessPieces, bool otherPlayer)
    {
        bool dontEndTurn = false;

        Vector2Int[] lastMove = moveList[moveList.Count - 1];
        foreach (SpecialMove specialMove in specialMoves)
        {
            // Use switch-case will make the code indent very far to the right
            if (specialMove == SpecialMove.EN_PASSANT)
            {
                if (ProcessEnPassant(ref moveList, ref chessPieces))
                    dontEndTurn = true;
            }
            else if (specialMove == SpecialMove.CASTLING)
            {
                if (ProcessCastling(ref moveList, ref chessPieces))
                    dontEndTurn = true;
            }
            else if (specialMove == SpecialMove.PROMOTION)
            {
                // If this promotion come from other player, we don't process it
                if (otherPlayer) return true;
                if (ProcessPromotion(ref moveList, ref chessPieces))
                    dontEndTurn = true;
            }
        }

        return dontEndTurn;
    }

    private bool ProcessEnPassant(ref List<Vector2Int[]> moveList, ref ChessPiece[,] chessPieces)
    {
        Vector2Int[] lastMove = moveList[moveList.Count - 1];
        Vector2Int[] enemyPawnMove = moveList[moveList.Count - 2];
        ChessPiece myPawn = chessPieces[lastMove[1].x, lastMove[1].y];
        ChessPiece enemyPawn = chessPieces[enemyPawnMove[1].x, enemyPawnMove[1].y];

        if (myPawn.currentX == enemyPawn.currentX)
        {
            if (myPawn.currentY == enemyPawn.currentY - 1 ||
                myPawn.currentY == enemyPawn.currentY + 1)
            {
                Chessboard.instance.AddToDeadList(enemyPawn);
                chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;
            }
        }
        return false; // end turn
    }

    private bool ProcessCastling(ref List<Vector2Int[]> moveList, ref ChessPiece[,] chessPieces)
    {
        // TODO: Take account the board size can grow
        Vector2Int[] lastMove = moveList[moveList.Count - 1];
        int ourY = lastMove[1].y;
        // Left rook castling (king moved to the left)
        if (lastMove[1].x == 2 && (ourY == 0 || ourY == 7))
        {
            // Move left rook from x = 0 to x = 3
            ChessPiece leftPiece = chessPieces[0, ourY];
            if (chessPieces[3, ourY] != null)
            {
                Chessboard.instance.AddToDeadList(chessPieces[3, ourY]);
            }
            chessPieces[3, ourY] = leftPiece;
            Chessboard.instance.PositionSinglePiece(3, ourY);
            chessPieces[0, ourY] = null;
        }
        // Right rook castling
        if (lastMove[1].x == 6 && (ourY == 0 || ourY == 7))
        {
            // Move right rook from x = 7 to x = 5
            ChessPiece rightPiece = chessPieces[7, ourY];
            if (chessPieces[5, ourY] != null)
            {
                Chessboard.instance.AddToDeadList(chessPieces[5, ourY]);
            }
            chessPieces[5, ourY] = rightPiece;
            Chessboard.instance.PositionSinglePiece(5, ourY);
            chessPieces[7, ourY] = null;
        }

        return false;
    }

    private bool ProcessPromotion(ref List<Vector2Int[]> moveList, ref ChessPiece[,] chessPieces)
    {
        Vector2Int[] lastMove = moveList[moveList.Count - 1];
        ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];
        if (targetPawn.type == PieceType.PAWN)
        {
            int ourY = lastMove[1].y;
            if (ourY == 0 || ourY == 7)
            {
                chosenPiecePromo = PieceType.NONE;
                StartCoroutine(PromotionScreen(targetPawn));
                return true; // dont end turn
            }
        }
        return false; // end turn
    }

    private IEnumerator PromotionScreen(ChessPiece targetPiece)
    {
        promotionScreen.GetComponent<PromotionScreen>().pieceProfile = targetPiece.profile;
        promotionScreen.SetActive(true);
        while (chosenPiecePromo == PieceType.NONE)
        {
            yield return null;
        }
        PromotePiece(targetPiece, chosenPiecePromo);
        promotionScreen.SetActive(false);
        promotionScreen.GetComponent<PromotionScreen>().pieceProfile = null;
        Chessboard.instance.EndTurn();
        if (!Chessboard.instance.isLocalGame)
        {
            GameManager.instance.NotifyChangePiece(new Vector2Int(targetPiece.currentX, targetPiece.currentY), chosenPiecePromo);
        }

    }

    public void SetChosenPromote(int chosenType)
    {
        Debug.Log("Promote to " + (PieceType)chosenType);
        chosenPiecePromo = (PieceType)chosenType;
    }

    public void PromotePiece(ChessPiece targetPiece, PieceType chosenType)
    {
        int x = targetPiece.currentX;
        int y = targetPiece.currentY;
        ChessPiece[,] chessPieces = Chessboard.instance.GetBoardRef();
        ChessPiece newPiece = Chessboard.instance.SpawnSinglePiece(chosenType, targetPiece.team);
        Destroy(chessPieces[x, y].gameObject);
        chessPieces[x, y] = newPiece;
        Chessboard.instance.PositionSinglePiece(x, y, true);
    }

}