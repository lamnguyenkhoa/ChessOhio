using System.Collections.Generic;
using UnityEngine;

public class Archbishop : ChessPiece
{
    public override List<Vector2Int> GetNormalMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        // Bishop part
        Vector2Int[] bishopDirections = {
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
        };
        int x, y, dirX, dirY;
        for (int i = 0; i < bishopDirections.Length; i++)
        {
            dirX = bishopDirections[i].x;
            dirY = bishopDirections[i].y;
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
                dirX += bishopDirections[i].x;
                dirY += bishopDirections[i].y;
            }
        }

        // Knight part
        Vector2Int[] direction = {
            new Vector2Int(-1, -2),
            new Vector2Int(-1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(1, 2),
            new Vector2Int(-2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(2, 1)
        };
        for (int i = 0; i < direction.Length; i++)
        {
            if (WithinBoundaryAfterMove(direction[i].x, direction[i].y))
            {
                x = currentX + direction[i].x;
                y = currentY + direction[i].y;
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
