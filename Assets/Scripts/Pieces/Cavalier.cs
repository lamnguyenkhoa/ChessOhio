using System.Collections.Generic;
using UnityEngine;

public class Cavalier : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        Vector2Int[] direction;
        if (team == PieceTeam.WHITE)
        {
            direction = new Vector2Int[] {
                new Vector2Int(-1, 2),
                new Vector2Int(1, 2),
                new Vector2Int(-2, 1),
                new Vector2Int(2, 1)
            };
        }
        else
        {
            direction = new Vector2Int[] {
                new Vector2Int(-2, -1),
                new Vector2Int(2, -1),
                new Vector2Int(1, -2),
                new Vector2Int(-1, -2)
            };
        }

        int x, y;
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

    public override bool CanMoveAgainAfterCapture()
    {
        return true;
    }
}
