using System.Collections.Generic;
using UnityEngine;

public class Emperor : King
{
    public override List<Vector2Int> GetNormalMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
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
}
