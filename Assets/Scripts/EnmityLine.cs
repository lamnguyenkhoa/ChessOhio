using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnmityLine : MonoBehaviour
{
    private LineRenderer line;
    public ChessPiece attacker;
    public ChessPiece receiver;

    void Start()
    {
    }

    /// <summary>
    /// Check if attacker still target receiver. If this enmity line is not relevent, delete itself
    /// </summary>
    public bool CheckIfStillRelevent()
    {
        if (attacker == null || receiver == null || attacker.dead || receiver.dead)
        {
            attacker.hasEnmityLine = false;
            return false;
        }
        List<Vector2Int> moves = attacker.GetAvailableMoves();
        Vector2Int receiverPos = new Vector2Int(receiver.currentX, receiver.currentY);
        if (moves.Contains(receiverPos))
        {
            RefreshData();
            return true;
        }
        attacker.hasEnmityLine = false;
        return false;
    }

    public void RefreshData()
    {
        if (!line)
        {
            line = GetComponent<LineRenderer>();
        }
        if (!attacker || !receiver)
            return;
        // Delay a bit to wait the piece fully positioned.
        attacker.hasEnmityLine = true;
        Vector3 start = Chessboard.instance.GetTileCenter(attacker.currentX, attacker.currentY, true);
        Vector3 end = Chessboard.instance.GetTileCenter(receiver.currentX, receiver.currentY, true);

        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);

    }

}
