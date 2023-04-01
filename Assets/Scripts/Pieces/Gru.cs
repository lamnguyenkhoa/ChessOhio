using System.Collections.Generic;
using UnityEngine;

public class Gru : ChessPiece
{
    void Start()
    {
    }
    public override List<Vector2Int> GetNormalMoves(ref ChessPiece[,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> availableMoves = new List<Vector2Int>();

        Vector2Int[] offset = {
            new Vector2Int(0, 3),
            new Vector2Int(-1, 2),
            new Vector2Int(0, 2),
            new Vector2Int(1, 2),
            new Vector2Int(-2, 1),
            new Vector2Int(-1, 1),
            new Vector2Int(1, 1),
            new Vector2Int(2, 1),
            new Vector2Int(-3, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(2, 0),
            new Vector2Int(3, 0),
            new Vector2Int(-2, -1),
            new Vector2Int(-1, -1),
            new Vector2Int(1, -2),
            new Vector2Int(2, -1),
            new Vector2Int(-1, -2),
            new Vector2Int(0, -2),
            new Vector2Int(1, -2),
            new Vector2Int(0, -3),
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

    public override void ResolveAfterMove()
    {
        MinifyEnemyPiece();
    }


    /// <summary>
    /// This run once after the Gru piece is moved / capture enemy
    /// </summary>
    private void MinifyEnemyPiece()
    {
        PieceTeam otherTeam = team == PieceTeam.WHITE ? PieceTeam.BLACK : PieceTeam.WHITE;
        ref ChessPiece[,] board = ref Chessboard.instance.GetBoardRef();
        List<ChessPiece> piecePool = new List<ChessPiece>();

        for (int x = 0; x < Chessboard.TILE_COUNT_X; x++)
        {
            for (int y = 0; y < Chessboard.TILE_COUNT_Y; y++)
            {
                if (board[x, y] != null &&
                    board[x, y].team == otherTeam)
                {
                    if (!board[x, y].isGruMinified)
                    {
                        piecePool.Add(board[x, y]);
                    }
                }
            }
        }

        int randomIndex = Random.Range(0, piecePool.Count);
        ChessPiece pieceToMinify = piecePool[randomIndex];
        Chessboard.instance.GruMinifyPiece(pieceToMinify.currentX, pieceToMinify.currentY);
        if (!GameSetting.instance.isLocalGame)
        {
            GameManager.instance.NotifyGruMinify(pieceToMinify.currentX, pieceToMinify.currentY);
        }

    }
}
