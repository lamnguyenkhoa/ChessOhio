using Unity.Netcode;
using UnityEngine;
using TMPro;

public enum ClientID
{
    HOST = 0,
    CLIENT = 1
}

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public NetworkVariable<PieceTeam> teamTurn;
    public TextMeshProUGUI turnDisplay;
    public GameObject actionMenu;
    public GameObject textTooltip;
    public GameObject whiteCamera;
    public GameObject blackCamera;
    public GameObject exitCombineButton;



    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        teamTurn.Value = PieceTeam.WHITE;
        StartGameClientRpc(GameSetting.instance.isLocalGame);
        if (IsHost)
        {
            SetupEachPlayerClientRpc(GameSetting.instance.hostChosenTeam);
        }
    }

    public void ResetGame()
    {

    }

    [ClientRpc]
    private void SetupEachPlayerClientRpc(PieceTeam hostChosenTeam)
    {
        GetCurrentPlayer().GetComponent<ChessPlayer>().SetTeamAndCamera(hostChosenTeam);
    }

    public Chessboard GetChessBoard()
    {
        return GameObject.Find("Board").GetComponent<Chessboard>();
    }

    public ChessPlayer GetCurrentPlayer()
    {
        ulong id = NetworkManager.Singleton.LocalClientId;
        return NetworkManager.SpawnManager.GetPlayerNetworkObject(id).GetComponent<ChessPlayer>();
    }

    /// <summary>
    /// Let the other player know that you made your move.
    /// </summary>
    /// <param name="before"></param>
    /// <param name="after"></param>
    public void NotifyMadeAMove(Vector2Int before, Vector2Int after)
    {
        if (IsHost)
            MadeAMoveClientRpc(before, after);
        else
            MadeAMoveServerRpc(before, after);
    }

    [ClientRpc]
    private void MadeAMoveClientRpc(Vector2Int before, Vector2Int after)
    {
        if (!IsHost)
        {
            Chessboard.instance.MovePiece(before, after);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MadeAMoveServerRpc(Vector2Int before, Vector2Int after)
    {
        Chessboard.instance.MovePiece(before, after);
    }

    public void NotifyChangePiece(Vector2Int pos, PieceType type)
    {
        if (IsHost)
            ChangePieceClientRpc(pos, type);
        else
            ChangePieceServerRpc(pos, type);
    }

    [ClientRpc]
    private void ChangePieceClientRpc(Vector2Int pos, PieceType type)
    {
        if (!IsHost)
        {
            Debug.Log($"NotifyChangePiece {pos} {type}");
            Chessboard.instance.ChangePiece(pos, type);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangePieceServerRpc(Vector2Int pos, PieceType type)
    {
        Debug.Log($"NotifyChangePiece {pos} {type}");
        Chessboard.instance.ChangePiece(pos, type);
    }

    public void NotifyChosenRuleCard(int ruleCardId)
    {
        if (IsHost)
            ChosenRuleCardClientRpc(ruleCardId);
        else
            ChosenRuleCardServerRpc(ruleCardId);
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
    public void StartGameClientRpc(bool isLocalGame)
    {
        GetChessBoard().StartGame(isLocalGame);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchTurnServerRpc()
    {
        if (teamTurn.Value == PieceTeam.WHITE)
        {
            teamTurn.Value = PieceTeam.BLACK;
            AfterSwitchTurnStuffClientRpc("Black's turn");
        }
        else
        {
            teamTurn.Value = PieceTeam.WHITE;
            AfterSwitchTurnStuffClientRpc("White's turn");
        }

    }

    [ClientRpc]
    private void AfterSwitchTurnStuffClientRpc(string turnDisplayText)
    {
        turnDisplay.text = turnDisplayText;
        Chessboard.instance.IncreaseTurnCount();
    }

    public void OpenActionMenu(Vector2 pos, ChessPiece piece)
    {
        actionMenu.GetComponent<ActionMenu>().Setup(piece);
        actionMenu.transform.position = pos;
        actionMenu.SetActive(true);
        Chessboard.instance.disableRaycast = true;
    }

    public void CloseActionMenu()
    {
        actionMenu.SetActive(false);
        Chessboard.instance.disableRaycast = false;
    }

    public void OnToggleTooltipButton()
    {
        GameSetting.instance.showToolTip = !GameSetting.instance.showToolTip;
    }

    public void ShowTextToolTip(string content, Vector3 worldPosition)
    {
        if (GameSetting.instance.showToolTip)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            textTooltip.GetComponent<TextMeshProUGUI>().text = content;
            textTooltip.transform.position = screenPos;
            textTooltip.SetActive(true);
        }
    }

    public void HideTextToolTip()
    {
        if (textTooltip.activeSelf)
            textTooltip.SetActive(false);
    }

    public void OnExitCombineModeButton()
    {
        GameRule.instance.ExitCombineMode();
    }
}