using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override List<Vector2Int> GetNormalMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
        };

        int x, y, dirX, dirY;

        for (int i = 0; i < directions.Length; i++)
        {
            dirX = directions[i].x;
            dirY = directions[i].y;
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
                dirX += directions[i].x;
                dirY += directions[i].y;
            }
        }

        return availableMoves;
    }
}
