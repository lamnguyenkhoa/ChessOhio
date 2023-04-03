using Unity.Netcode;
using UnityEngine;

public class RpcHandler : NetworkBehaviour
{

    [ServerRpc(RequireOwnership = false)]
    public void ConfirmResetServerRpc(bool isHost)
    {
        if (isHost)
            GameManager.instance.hostResetConfirmed = true;
        else
            GameManager.instance.clientResetConfirmed = true;
        if (GameManager.instance.hostResetConfirmed && GameManager.instance.clientResetConfirmed)
        {
            GameManager.instance.hostResetConfirmed = false;
            GameManager.instance.clientResetConfirmed = false;
            GameSetting.instance.LoadIngameSceneServerRpc();
        }
    }

    [ClientRpc]
    public void SetupEachPlayerClientRpc(PieceTeam hostChosenTeam)
    {
        GameManager.instance.GetCurrentPlayer().GetComponent<ChessPlayer>().SetTeamAndCamera(hostChosenTeam);
    }

    [ClientRpc]
    public void MadeAMoveClientRpc(Vector2Int before, Vector2Int after)
    {
        if (!IsHost)
        {
            Chessboard.instance.MovePiece(before, after);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void MadeAMoveServerRpc(Vector2Int before, Vector2Int after)
    {
        Chessboard.instance.MovePiece(before, after);
    }

    [ClientRpc]
    public void ChangePieceClientRpc(Vector2Int pos, PieceType type)
    {
        if (!IsHost)
        {
            Debug.Log($"NotifyChangePiece {pos} {type}");
            Chessboard.instance.ChangePiece(pos, type);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangePieceServerRpc(Vector2Int pos, PieceType type)
    {
        Debug.Log($"NotifyChangePiece {pos} {type}");
        Chessboard.instance.ChangePiece(pos, type);
    }

    [ClientRpc]
    public void ChosenRuleCardClientRpc(int ruleCardId)
    {
        if (!IsHost)
        {
            RuleCardSO ruleCard = GameRule.instance.availableRule[ruleCardId];
            GameRule.instance.ChoseThisRule(ruleCard);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChosenRuleCardServerRpc(int ruleCardId)
    {
        RuleCardSO ruleCard = GameRule.instance.availableRule[ruleCardId];
        GameRule.instance.ChoseThisRule(ruleCard);
    }

    [ClientRpc]
    public void StartGameClientRpc()
    {
        Chessboard.instance.StartGame(false);
    }

    [ClientRpc]
    public void EndTurnClientRpc()
    {
        if (!IsHost)
        {
            Chessboard.instance.EndTurn();
        };
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc()
    {
        Chessboard.instance.EndTurn();
    }

    [ClientRpc]
    public void NotifySurrenderClientRpc(PieceTeam teamThatSurrender)
    {
        if (!IsHost)
        {
            Chessboard.instance.CheckMate(teamThatSurrender);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifySurrenderServerRpc(PieceTeam teamThatSurrender)
    {
        Chessboard.instance.CheckMate(teamThatSurrender);
    }

    [ClientRpc]
    public void NotifyGruMinifyClientRpc(int x, int y)
    {
        if (!IsHost)
        {
            Chessboard.instance.GruMinifyPiece(x, y);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyGruMinifyServerRpc(int x, int y)
    {
        Chessboard.instance.GruMinifyPiece(x, y);
    }
}