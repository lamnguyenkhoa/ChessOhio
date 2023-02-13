using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (promotionScreen.activeSelf)
            {
                promotionScreen.SetActive(false);
            }
        }
        else
        {
            Destroy(this);
        }
    }

    public bool ProcessSpecialMoves(ref List<Vector2Int[]> moveList, ref List<SpecialMove> specialMoves, ref ChessPiece[,] chessPieces)
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
                return true;
            }
        }

        return false;
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
            ChessPiece rook = chessPieces[0, ourY];
            chessPieces[3, ourY] = rook;
            Chessboard.instance.PositionSinglePiece(3, ourY);
            chessPieces[0, ourY] = null;
            return true;
        }
        // Right rook castling
        if (lastMove[1].x == 6 && (ourY == 0 || ourY == 7))
        {
            // Move right rook from x = 7 to x = 5
            ChessPiece rook = chessPieces[7, ourY];
            chessPieces[5, ourY] = rook;
            Chessboard.instance.PositionSinglePiece(5, ourY);
            chessPieces[7, ourY] = null;
            return true;
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
                return true;
            }
        }
        return false;
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
        GameManager.instance.SwitchTurnServerRpc();
        // TODO: Notify other player what piece we promoted to.
    }

    public void SetChosenPromote(int chosenType)
    {
        Debug.Log("Promote to " + (PieceType)chosenType);
        chosenPiecePromo = (PieceType)chosenType;
    }

    public void PromotePiece(ChessPiece targetPiece, PieceType chosenPiece)
    {
        int x = targetPiece.currentX;
        int y = targetPiece.currentY;
        ChessPiece[,] chessPieces = Chessboard.instance.GetBoardRef();
        ChessPiece newQueen = Chessboard.instance.SpawnSinglePiece(chosenPiece, targetPiece.team);
        Destroy(chessPieces[x, y].gameObject);
        chessPieces[x, y] = newQueen;
        Chessboard.instance.PositionSinglePiece(x, y, true);
    }

}