using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetNormalMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        int BLACK_PAWN_Y_START = tileCountY - 2;
        int WHITE_PAWN_Y_START = 1;
        List<Vector2Int> availableMoves = new List<Vector2Int>();
        int direction = (team == PieceTeam.WHITE) ? 1 : -1;

        // One in front
        if (WithinBoundaryAfterMove(0, direction) &&
            board[currentX, currentY + direction] == null)
        {
            availableMoves.Add(new Vector2Int(currentX, currentY + direction));
        }

        // Two in front
        if (WithinBoundaryAfterMove(0, direction * 2) &&
            board[currentX, currentY + direction] == null)
        {
            if (team == PieceTeam.WHITE && currentY == WHITE_PAWN_Y_START &&
                board[currentX, currentY + (direction * 2)] == null)
            {
                availableMoves.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }

            if (team == PieceTeam.BLACK && currentY == BLACK_PAWN_Y_START &&
                board[currentX, currentY + (direction * 2)] == null)
            {
                availableMoves.Add(new Vector2Int(currentX, currentY + (direction * 2)));
            }

            availableMoves.Add(new Vector2Int(currentX, currentY + direction));
        }

        // Diagonal (Kill move)
        if (WithinBoundaryAfterMove(0, direction))
        {
            if (currentX != tileCountX - 1)
            {
                if (board[currentX + 1, currentY + direction] != null && board[currentX + 1, currentY + direction].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                }
            }
            if (currentX != 0)
            {
                if (board[currentX - 1, currentY + direction] != null && board[currentX - 1, currentY + direction].team != team)
                {
                    availableMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
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

        // En passant
        if (EnPassant(ref board, ref moveList, ref availableMoves))
        {
            specialMoves.Add(SpecialMove.EN_PASSANT);
        };

        return specialMoves;
    }

    private bool EnPassant(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves)
    {
        int direction = (team == PieceTeam.WHITE) ? 1 : -1;
        if (moveList.Count > 0)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            if (board[lastMove[1].x, lastMove[1].y].type == PieceType.PAWN) // If last piece was a pawn
            {
                if (Mathf.Abs(lastMove[0].y - lastMove[1].y) == 2) // If last move was moving 2 tile vertical
                {
                    if (board[lastMove[1].x, lastMove[1].y].team != team) // Move from other team
                    {
                        if (lastMove[1].y == currentY) // If both pawn on same Y
                        {
                            if (lastMove[1].x == currentX - 1) // Landed left
                            {
                                availableMoves.Add(new Vector2Int(currentX - 1, currentY + direction));
                                return true;
                            }
                            if (lastMove[1].x == currentX + 1) // Landed right
                            {
                                availableMoves.Add(new Vector2Int(currentX + 1, currentY + direction));
                                return true;
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public override List<PieceType> GetPossiblePromotions()
    {
        List<PieceType> possiblePromotions = new List<PieceType> { PieceType.QUEEN,
            PieceType.BISHOP, PieceType.KNIGHT, PieceType.ROOK };
        return possiblePromotions;
    }
}
