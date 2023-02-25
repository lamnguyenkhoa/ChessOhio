using System.Collections.Generic;
using UnityEngine;

public class Deserter : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        int direction = (team == PieceTeam.WHITE) ? -1 : 1;

        // One in front
        if (WithinBoundaryAfterMove(0, direction))
        {
            availableMoves.Add(new Vector2Int(currentX, currentY + direction));
        }

        return availableMoves;

    }

}
