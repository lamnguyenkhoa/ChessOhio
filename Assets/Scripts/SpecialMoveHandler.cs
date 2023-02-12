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
    public static void ProcessSpecialMoves(ref List<Vector2Int[]> moveList, ref List<SpecialMove> specialMoves, ref ChessPiece[,] chessPieces)
    {
        Vector2Int[] lastMove = moveList[moveList.Count - 1];
        foreach (SpecialMove specialMove in specialMoves)
        {
            // Use switch-case will make the code indent very far to the right
            if (specialMove == SpecialMove.EN_PASSANT)
            {
                ProcessEnPassant(ref moveList, ref chessPieces);
            }
            else if (specialMove == SpecialMove.CASTLING)
            {
                ProcessCastling(ref moveList, ref chessPieces);
            }
            else if (specialMove == SpecialMove.PROMOTION)
            {
                ProcessPromotion(ref moveList, ref chessPieces);
            }
        }
    }

    private static void ProcessEnPassant(ref List<Vector2Int[]> moveList, ref ChessPiece[,] chessPieces)
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
    }

    private static void ProcessCastling(ref List<Vector2Int[]> moveList, ref ChessPiece[,] chessPieces)
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
        }
        // Right rook castling
        if (lastMove[1].x == 6 && (ourY == 0 || ourY == 7))
        {
            // Move right rook from x = 7 to x = 5
            ChessPiece rook = chessPieces[7, ourY];
            chessPieces[5, ourY] = rook;
            Chessboard.instance.PositionSinglePiece(5, ourY);
            chessPieces[7, ourY] = null;
        }
    }

    private static void ProcessPromotion(ref List<Vector2Int[]> moveList, ref ChessPiece[,] chessPieces)
    {
        // TODO: Add a UI allow to choose which piece the pawn
        // promoto to. Currently only promote to Queen.
        Vector2Int[] lastMove = moveList[moveList.Count - 1];
        ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];
        if (targetPawn.type == PieceType.PAWN)
        {
            int ourY = lastMove[1].y;
            if (ourY == 0 || ourY == 7)
            {
                ChessPiece newQueen = Chessboard.instance.SpawnSinglePiece(PieceType.QUEEN, targetPawn.team);
                Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                Chessboard.instance.PositionSinglePiece(lastMove[1].x, lastMove[1].y, true);
            }
        }
    }

}