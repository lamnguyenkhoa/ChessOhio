using System.Collections.Generic;
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public PieceType type;
    public PieceTeam team;
    public int currentX;
    public int currentY;
    private Vector3 desiredPosition;
    private Vector3 desiredScale = Vector3.one;
    public List<PieceType> possiblePromotions = new List<PieceType>();
    public ChessPieceProfileSO profile;
    public bool lockedControl = false;

    [Header("Common Stat")]
    public Dictionary<PieceType, int> captureHistory = new Dictionary<PieceType, int>();

    [Header("Logic stat")]
    public int timeMoveAgain; // How many time this piece move again (due to its ability)

    private void Start()
    {
        transform.rotation = Quaternion.Euler((team == PieceTeam.BLACK) ? Vector3.zero : new Vector3(0, 180, 0));
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPosition, Time.deltaTime * 10);
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
            transform.localPosition = desiredPosition;
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

    public virtual List<SpecialMove> GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        List<SpecialMove> specialMoves = new List<SpecialMove>();
        return specialMoves;
    }

    public virtual List<PieceType> GetPossiblePromotions()
    {
        return new List<PieceType>();
    }

    public virtual bool CanCaptureAlly()
    {
        return false;
    }

    public virtual void UpdateStatCaptureHistory(PieceType enemyType)
    {
        if (captureHistory.ContainsKey(enemyType))
        {
            captureHistory[enemyType] += 1;
        }
        else
        {
            captureHistory[enemyType] = 1;
        }
    }

    public virtual bool CanMoveAgainAfterCapture()
    {
        return false;
    }

    public virtual int MaxTimeMoveAgain()
    {
        return 0;
    }
}
