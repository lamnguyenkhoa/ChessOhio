using System.Collections.Generic;
using UnityEngine;

public class Chancellor : ChessPiece
{
    public override List<Vector2Int> GetNormalMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        // Rook part
        Vector2Int[] rookDirections = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
        };
        int x, y, dirX, dirY;
        for (int i = 0; i < rookDirections.Length; i++)
        {
            dirX = rookDirections[i].x;
            dirY = rookDirections[i].y;
            while (WithinBoundaryAfterMove(dirX, dirY))
            {
                x = currentX + dirX;
                y = currentY + dirY;
                if (board[x, y] == null)
                {
                    availableMoves.Add(new Vector2Int(x, y));
                }
                if (board[x, y] != null)
                {
                    if (board[x, y].team != team)
                    {
                        availableMoves.Add(new Vector2Int(x, y));
                    }
                    break;
                }
                dirX += rookDirections[i].x;
                dirY += rookDirections[i].y;
            }
        }

        // Knight part
        Vector2Int[] knightDirections = {
            new Vector2Int(-1, -2),
            new Vector2Int(-1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(1, 2),
            new Vector2Int(-2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(2, 1)
        };
        for (int i = 0; i < knightDirections.Length; i++)
        {
            if (WithinBoundaryAfterMove(knightDirections[i].x, knightDirections[i].y))
            {
                x = currentX + knightDirections[i].x;
                y = currentY + knightDirections[i].y;
                if (board[x, y] == null ||
                    board[x, y].team != team)
                {
                    availableMoves.Add(new Vector2Int(x, y));
                }
            }
        }
        return availableMoves;
    }
}
