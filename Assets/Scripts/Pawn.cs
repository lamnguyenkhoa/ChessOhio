using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        int BLACK_PAWN_Y_START = tileCountY - 2;
        int WHITE_PAWN_Y_START = 1;

        List<Vector2Int> availableMoves = new List<Vector2Int>();

        int direction = (team == PieceTeam.WHITE) ? 1 : -1;

        // One in front
        if (WithinBoundary(0, direction) &&
            board[currentX, currentY + direction] == null)
        {
            availableMoves.Add(new Vector2Int(currentX, currentY + direction));
        }

        // Two in front
        if (WithinBoundary(0, direction * 2) &&
            board[currentX, currentY + direction] == null)
        {
            if (team == PieceTeam.WHITE && currentY == WHITE_PAWN_Y_START &&
                board[currentX, currentY + (direction * 2)] == null)
            {
                availableMoves.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }

            if (team == PieceTeam.BLACK && currentY == BLACK_PAWN_Y_START &&
                board[currentX, currentY + (direction * 2)] == null)
            {
                availableMoves.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }

            availableMoves.Add(new Vector2Int(currentX, currentY + direction));
        }

        // Diagonal (Kill move)
        if (WithinBoundary(0, direction))
        {
            if (currentX != tileCountX - 1)
            {
                if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                }
            }
            if (currentX != 0)
            {
                if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
                }
            }
        }

        // En passant

        return availableMoves;
    }
}
