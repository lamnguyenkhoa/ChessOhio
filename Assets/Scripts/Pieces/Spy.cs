using System.Collections.Generic;
using UnityEngine;

public class Spy : ChessPiece
{
    public override List<Vector2Int> GetNormalMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        Vector2Int[] offset = {
            new Vector2Int(0, 1),
            new Vector2Int(0, 2),
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
        List<SpecialMove> specialMoves = new List<SpecialMove>();
        int direction = (team == PieceTeam.WHITE) ? 1 : -1;

        // Promotion
        if ((team == PieceTeam.WHITE && currentY == 6 || (team == PieceTeam.BLACK && currentY == 1)))
            specialMoves.Add(SpecialMove.PROMOTION);

        return specialMoves;
    }
}
