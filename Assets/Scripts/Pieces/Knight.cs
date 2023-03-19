using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{
    public override List<Vector2Int> GetNormalMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        Vector2Int[] directions = {
            new Vector2Int(-1, -2),
            new Vector2Int(-1, 2),
            new Vector2Int(1, -2),
            new Vector2Int(1, 2),
            new Vector2Int(-2, -1),
            new Vector2Int(-2, 1),
            new Vector2Int(2, -1),
            new Vector2Int(2, 1)
        };

        int x, y;
        for (int i = 0; i < directions.Length; i++)
        {
            if (WithinBoundaryAfterMove(directions[i].x, directions[i].y))
            {
                x = currentX + directions[i].x;
                y = currentY + directions[i].y;
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
