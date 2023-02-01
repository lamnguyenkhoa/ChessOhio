using System.Collections.Generic;
using UnityEngine;

public enum PieceType
{
    NONE = 0,
    PAWN = 1,
    ROOK = 2,
    KNIGHT = 3,
    BISHOP = 4,
    QUEEN = 5,
    KING = 6
}

public enum PieceTeam
{
    WHITE = 0,
    BLACK = 1
}

public class ChessPiece : MonoBehaviour
{
    public PieceType type;
    public PieceTeam team;
    public int currentX;
    public int currentY;

    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 10);
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime * 10);
    }

    public virtual List<Vector2Int> GetAvailableMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        return availableMoves;
    }

    public virtual void SetPosition(Vector3 position, bool instant = false)
    {
        desiredPosition = position;
        if (instant)
        {
            transform.position = desiredPosition;
        }
    }

    public virtual void SetScale(Vector3 scale, bool instant = false)
    {
        desiredScale = scale;
        if (instant)
        {
            transform.localScale = desiredScale;
        }
    }

    /// <summary>
    /// Check if after move dirX and dirY, is tile still within boundary
    /// </summary>
    /// <param name="dirX"></param>
    /// <param name="dirY"></param>
    /// <returns></returns>
    public virtual bool WithinBoundaryAfterMove(int dirX, int dirY)
    {
        return currentY + dirY <= 7 &&
            currentY + dirY >= 0 &&
            currentX + dirX <= 7 &&
            currentX + dirX >= 0;
    }

}
