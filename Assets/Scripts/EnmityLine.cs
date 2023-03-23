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
        StartCoroutine(ShowTheRedLine());
    }

    IEnumerator ShowTheRedLine()
    {
        yield return new WaitForSeconds(0.2f);
        Vector3 start = attacker.transform.position;
        Vector3 end = receiver.transform.position;

        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
    }
}
