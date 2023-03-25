using UnityEngine;

public class PawnHub : MonoBehaviour
{
    public int turnForNewPawn = 7;
    public int turnCount = 0;
    public GameObject pawnPrefab;


    void Start()
    {
        Chessboard.instance.onEndTurn.AddListener(PawnHubAction);
    }

    public void PawnHubAction()
    {
        turnCount += 1;
        if (turnCount == 7)
        {
            // Find empty space to spawn white pawn
            for (int i = 0; i < Chessboard.TILE_COUNT_X; i++)
            {
                if (Chessboard.instance.TrySpawnSinglePieceAt(PieceType.PAWN, PieceTeam.WHITE, new Vector2Int(i, 0)))
                {
                    break;
                }
            }

            // Find empty space to spawn black pawn
            for (int i = 0; i < Chessboard.TILE_COUNT_X; i++)
            {
                if (Chessboard.instance.TrySpawnSinglePieceAt(PieceType.PAWN, PieceTeam.BLACK, new Vector2Int(i, Chessboard.TILE_COUNT_Y - 1)))
                {
                    break;
                }
            }
            turnCount = 0;
        }
    }
}
