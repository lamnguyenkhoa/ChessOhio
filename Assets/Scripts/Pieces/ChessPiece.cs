using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
    public bool hasEnmityLine = false;
    public bool dead = false;

    [Header("Common Stat")]
    public Dictionary<PieceType, int> captureHistory = new Dictionary<PieceType, int>(); // Not display in InfoWindow
    private int turnMoved = 0;

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

    public virtual List<Vector2Int> GetNormalMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        return availableMoves;
    }

    /// <summary>
    /// Has rule-filtered moves.
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetAvailableMoves()
    {
        ChessPiece[,] board = Chessboard.instance.GetBoardRef();
        List<SpecialMove> specialMoves = Chessboard.instance.GetSpecialMovesRef();
        int tileCountX = Chessboard.TILE_COUNT_X;
        int tileCountY = Chessboard.TILE_COUNT_Y;

        List<Vector2Int> availableMoves = GetNormalMoves(ref board, tileCountX, tileCountY);
        specialMoves = GetSpecialMoves(ref board, ref Chessboard.instance.GetMoveListRef(), ref availableMoves);

        if (GameRule.instance.activatedConstraintRule == UniqueRuleCode.CONSTRAINT_FORCE_CAPTURE)
        {
            List<Vector2Int> captureOnlyMoves = new List<Vector2Int>();
            foreach (Vector2Int move in availableMoves)
            {
                //If this tile has enemy piece
                if (board[move.x, move.y] != null && board[move.x, move.y].team != team)
                {
                    captureOnlyMoves.Add(move);
                }
            }
            if (captureOnlyMoves.Count > 0)
            {
                availableMoves = captureOnlyMoves;
            }
        }

        // Remove dangerous tiles for essential piece.
        if (IsEssential())
        {
            // First temporary remove the essential piece (king, empress,...)
            ChessPiece essentialPiece = this;
            board[this.currentX, this.currentY] = null;

            availableMoves = availableMoves.FindAll(x => !Chessboard.instance.IsThisTileDangerous(x, team));

            // Now add the essential piece back
            board[this.currentX, this.currentY] = this;
        }
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

    public virtual void UpdateTurnMoved()
    {
        turnMoved += 1;
    }

    public virtual bool CanMoveAgainAfterCapture()
    {
        return false;
    }

    public virtual int MaxTimeMoveAgain()
    {
        return 0;
    }


    /// <summary>
    /// End game if captured
    /// </summary>
    /// <returns></returns>
    public virtual bool IsEssential()
    {
        return false;
    }

    public virtual int GetStatCapturedNumber()
    {
        int sum = captureHistory.Values.Sum();
        return sum;
    }

    public virtual int GetStatTurnMoved()
    {
        return turnMoved;
    }

    /// <summary>
    /// Tile that this piece can potentially reach and capture the King.
    /// Example: Pawn has 3 tiles: front, front left and front right.
    /// We don't check if there any enemy piece on the front left or front right tile.
    /// </summary>
    /// <returns></returns>
    public virtual List<Vector2Int> GetAttackMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        return GetNormalMoves(ref board, tileCountX, tileCountY);
    }

}
