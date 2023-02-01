using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    public override List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        // Down
        for (int i = currentY - 1; i >= 0; i--)
        {
            if (board[currentX, i] == null)
            {
                availableMoves.Add(new Vector2Int(currentX, i));
            }

            if (board[currentX, i] != null)
            {
                if (board[currentX, i].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentX, i));
                }
                break;
            }
        }

        // Up
        for (int i = currentY + 1; i <= tileCountY - 1; i++)
        {
            if (board[currentX, i] == null)
            {
                availableMoves.Add(new Vector2Int(currentX, i));
            }

            if (board[currentX, i] != null)
            {
                if (board[currentX, i].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentX, i));
                }
                break;
            }
        }

        // Right
        for (int i = currentX + 1; i <= tileCountX - 1; i++)
        {
            if (board[i, currentY] == null)
            {
                availableMoves.Add(new Vector2Int(i, currentY));
            }

            if (board[i, currentY] != null)
            {
                if (board[i, currentY].team != team)
                {
                    availableMoves.Add(new Vector2Int(i, currentY));
                }
                break;
            }
        }

        // Left
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (board[i, currentY] == null)
            {
                availableMoves.Add(new Vector2Int(i, currentY));
            }

            if (board[i, currentY] != null)
            {
                if (board[i, currentY].team != team)
                {
                    availableMoves.Add(new Vector2Int(i, currentY));
                }
                break;
            }
        }

        return availableMoves;
    }
}
