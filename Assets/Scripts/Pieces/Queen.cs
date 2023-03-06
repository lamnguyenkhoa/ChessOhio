using System.Collections.Generic;
using UnityEngine;

public class Queen : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, -1),
        };

        int x, y, distanceX, distanceY;

        for (int i = 0; i < directions.Length; i++)
        {
            distanceX = directions[i].x;
            distanceY = directions[i].y;
            while (WithinBoundaryAfterMove(distanceX, distanceY))
            {
                x = currentX + distanceX;
                y = currentY + distanceY;
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
                distanceX += directions[i].x;
                distanceY += directions[i].y;
            }
        }

        return availableMoves;
    }
}
