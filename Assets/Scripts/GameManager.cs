using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public PieceTeam teamTurn;
    public TextMeshProUGUI turnDisplay;
    public GameObject actionMenu;
    public GameObject textTooltip;
    public GameObject[] whiteTeamStuff;
    public GameObject[] blackTeamStuff;
    public GameObject exitCombineButton;
    public GameObject viewChosenRuleDisplay;
    private GameObject displayRuleCard;
    public InfoWindow infoWindow;
    public GameObject helpWindow;
    public GameObject disconnectScreen;
    public GameObject davieCheck;

    public bool hostResetConfirmed = false;
    public bool clientResetConfirmed = false;

    private RpcHandler rpcHandler;

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
        teamTurn = PieceTeam.WHITE;
        rpcHandler = GetComponent<RpcHandler>();
        if (displayRuleCard == null)
        {
            displayRuleCard = viewChosenRuleDisplay.transform.Find("DisplayCard").gameObject;
        }
        if (GameSetting.instance.isLocalGame)
        {
            Chessboard.instance.StartGame(true);
        }
        rpcHandler.StartGameClientRpc();
        if (IsHost)
        {
            rpcHandler.SetupEachPlayerClientRpc(GameSetting.instance.hostChosenTeam);
        }

        // Observer pattern
        Chessboard.instance.onEndTurn.AddListener(UpdateChangeTurn);
    }

    public void ResetLANGame()
    {
        // Wait until both player press the reset button
        rpcHandler.ConfirmResetServerRpc(IsHost);
    }

    public void UpdateChangeTurn()
    {
        // Constraint rule
        if (GameRule.instance.activatedConstraintRule == UniqueRuleCode.CONSTRAINT_BALENCIAGA_CATWALK)
        {
            Chessboard.instance.ResolveBalenciagaCatwalk(teamTurn);
        }

        if (teamTurn == PieceTeam.WHITE)
        {
            teamTurn = PieceTeam.BLACK;
            turnDisplay.text = "Black's turn";
        }
        else
        {
            teamTurn = PieceTeam.WHITE;
            turnDisplay.text = "White's turn";
        }
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
            rpcHandler.MadeAMoveClientRpc(before, after);
        else
            rpcHandler.MadeAMoveServerRpc(before, after);
    }

    public void NotifyChangePiece(Vector2Int pos, PieceType type)
    {
        if (IsHost)
            rpcHandler.ChangePieceClientRpc(pos, type);
        else
            rpcHandler.ChangePieceServerRpc(pos, type);
    }

    public void NotifyChosenRuleCard(int ruleCardId)
    {
        if (IsHost)
            rpcHandler.ChosenRuleCardClientRpc(ruleCardId);
        else
            rpcHandler.ChosenRuleCardServerRpc(ruleCardId);
    }

    public void NotifyEndTurn()
    {
        if (IsHost)
            rpcHandler.EndTurnClientRpc();
        else
            rpcHandler.EndTurnServerRpc();
    }

    public void OpenActionMenu(Vector2 pos, ChessPiece piece)
    {
        actionMenu.GetComponent<ActionMenu>().Setup(piece);
        actionMenu.transform.position = pos;
        actionMenu.SetActive(true);
        Chessboard.instance.disableRaycastCount += 1;
    }

    public void CloseActionMenu()
    {
        actionMenu.SetActive(false);
        Chessboard.instance.disableRaycastCount -= 1;
    }

    public void OpenInfoWindow(ChessPiece piece)
    {
        infoWindow.piece = piece;
        infoWindow.ShowInfoWindow();
        CloseActionMenu();
    }

    public void CloseInfoWindow()
    {
        infoWindow.CloseInfoWindow();
    }

    public void OnToggleTooltipButton()
    {
        GameSetting.instance.showToolTip = !GameSetting.instance.showToolTip;
    }

    public void ShowTextToolTip(string content, Vector3 worldPosition)
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
        textTooltip.GetComponent<TextMeshProUGUI>().text = content;
        textTooltip.transform.position = screenPos;
        textTooltip.SetActive(true);
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

    public void OnViewChosenRuleDisplayButton()
    {
        Transform viewChosenRuleContent = viewChosenRuleDisplay.transform.Find("Scroll View").GetChild(0).GetChild(0);
        if (viewChosenRuleDisplay.activeSelf)
        {
            // Destroy all rule cards
            foreach (Transform child in viewChosenRuleContent)
            {
                Destroy(child.gameObject);
            }
        }
        else
        {
            // Recreate rule cards for display
            foreach (RuleCardSO ruleProfile in GameRule.instance.activatedRule)
            {
                RuleCard ruleCard = Instantiate(GameRule.instance.ruleCardPrefab, viewChosenRuleContent).GetComponent<RuleCard>();
                ruleCard.profile = ruleProfile;
                ruleCard.showDetails = true;
                ruleCard.finished = true;
                ruleCard.RefreshCardInfo();
            }
        }
        displayRuleCard.SetActive(false);
        viewChosenRuleDisplay.SetActive(!viewChosenRuleDisplay.activeSelf);
    }

    /// <summary>
    /// Use when select a rule card while "view all chosen rule card" is active
    /// </summary>
    public void QuickRefreshViewChosenRule(RuleCardSO ruleProfile)
    {
        Transform viewChosenRuleContent = viewChosenRuleDisplay.transform.Find("Scroll View").GetChild(0).GetChild(0);
        if (viewChosenRuleDisplay.activeSelf)
        {
            RuleCard ruleCard = Instantiate(GameRule.instance.ruleCardPrefab, viewChosenRuleContent).GetComponent<RuleCard>();
            ruleCard.profile = ruleProfile;
            ruleCard.showDetails = true;
            ruleCard.finished = true;
            ruleCard.RefreshCardInfo();
        }
    }

    public void SetDisplayCardProfile(RuleCardSO profile)
    {
        displayRuleCard.SetActive(true);
        displayRuleCard.GetComponent<RuleCard>().profile = profile;
        displayRuleCard.GetComponent<RuleCard>().RefreshCardInfo();
    }

    public void HideDisplayCardProfile()
    {
        displayRuleCard.GetComponent<RuleCard>().DisplayEmptyCard();
        displayRuleCard.SetActive(false);
    }

    public void OnHelpButton()
    {
        if (!helpWindow.activeSelf)
        {
            // Turn help window on so we pause game
            Chessboard.instance.disableRaycastCount += 1;
        }
        else
        {
            Chessboard.instance.disableRaycastCount -= 1;
        }
        helpWindow.SetActive(!helpWindow.activeSelf);
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
    }

    public void ShowDavieCheck()
    {
        Transform canvas = GameObject.Find("Canvas").transform;
        GameObject go = Instantiate(davieCheck, canvas);
    }

    public void NotifySurrender(PieceTeam teamThatSurrender)
    {
        if (IsHost)
            rpcHandler.NotifySurrenderClientRpc(teamThatSurrender);
        else
            rpcHandler.NotifySurrenderServerRpc(teamThatSurrender);
    }

    public void ReturnToLobby()
    {
        Destroy(GameSetting.instance.gameObject);
        Destroy(NetworkManager.Singleton.gameObject);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("Lobby");
    }

    public void ShowDisconnectScreen()
    {
        Chessboard.instance.disableRaycastCount += 1;
        disconnectScreen.SetActive(true);
    }

    public void NotifyGruMinify(int x, int y)
    {
        if (IsHost)
            rpcHandler.NotifyGruMinifyClientRpc(x, y);
        else
            rpcHandler.NotifyGruMinifyServerRpc(x, y);
    }
}