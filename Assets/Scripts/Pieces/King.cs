using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        Vector2Int[] offset = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
        };

        int x, y;
        for (int i = 0; i < offset.Length; i++)
        {
            if (WithinBoundaryAfterMove(offset[i].x, offset[i].y))
            {
                x = currentX + offset[i].x;
                y = currentY + offset[i].y;
                if (board[x, y] == null ||
                    board[x, y].team != team)
                {
                    availableMoves.Add(new Vector2Int(x, y));
                }
            }
        }

        return availableMoves;
    }

    public override List<SpecialMove> GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        // Check for castling
        List<SpecialMove> specialMoves = new List<SpecialMove>();
        const int R_ROOK_X = Chessboard.TILE_COUNT_Y - 1;
        const int L_ROOK_X = 0;
        const int KING_X = 4;
        int ourY = (team == PieceTeam.WHITE) ? 0 : Chessboard.TILE_COUNT_Y - 1;

        Vector2Int[] kingMove = moveList.Find(m => m[0].x == KING_X && m[0].y == ourY);
        Vector2Int[] leftRookMove = moveList.Find(m => m[0].x == L_ROOK_X && m[0].y == ourY);
        Vector2Int[] rightRookMove = moveList.Find(m => m[0].x == R_ROOK_X && m[0].y == ourY);

        // If king haven't move
        if (kingMove == null && board[KING_X, ourY].type == PieceType.KING)
        {
            // If left rook
            if (leftRookMove == null && board[L_ROOK_X, ourY].type == PieceType.ROOK && board[L_ROOK_X, ourY].team == team)
            {
                bool spaceBetweenIsEmpty = true;
                for (int i = L_ROOK_X + 1; i < KING_X; i++)
                {
                    if (board[i, ourY] != null) spaceBetweenIsEmpty = false;
                }
                if (spaceBetweenIsEmpty)
                {
                    availableMoves.Add(new Vector2Int(KING_X - 2, ourY));
                    specialMoves.Add(SpecialMove.CASTLING);

                }
            }
            if (rightRookMove == null && board[R_ROOK_X, ourY].type == PieceType.ROOK && board[R_ROOK_X, ourY].team == team)
            {
                bool spaceBetweenIsEmpty = true;
                for (int i = KING_X + 1; i < R_ROOK_X; i++)
                {
                    if (board[i, ourY] != null) spaceBetweenIsEmpty = false;
                }
                if (spaceBetweenIsEmpty)
                {
                    availableMoves.Add(new Vector2Int(KING_X + 2, ourY));
                    specialMoves.Add(SpecialMove.CASTLING);
                }
            }
        }
        return specialMoves;
    }
}
