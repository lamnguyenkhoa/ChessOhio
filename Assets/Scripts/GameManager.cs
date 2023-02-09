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
            SetEachPlayerCameraClientRpc();
        }
    }

    [ClientRpc]
    private void SetEachPlayerCameraClientRpc()
    {
        GetCurrentPlayer().GetComponent<ChessPlayer>().SetCamera();
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
        {
            MadeAMoveClientRpc(before, after);
        }
        else
        {
            MadeAMoveServerRpc(before, after);
        }
    }

    [ClientRpc]
    private void MadeAMoveClientRpc(Vector2Int before, Vector2Int after)
    {
        if (!IsHost)
        {
            GetChessBoard().MovePiece(before, after);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void MadeAMoveServerRpc(Vector2Int before, Vector2Int after)
    {
        GetChessBoard().MovePiece(before, after);
    }

    public void ResetGame()
    {
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
            ChangeTurnDisplayTextClientRpc("Black's turn");
        }
        else
        {
            teamTurn.Value = PieceTeam.WHITE;
            ChangeTurnDisplayTextClientRpc("White's turn");
        }
    }

    [ClientRpc]
    private void ChangeTurnDisplayTextClientRpc(string text)
    {
        turnDisplay.text = text;
    }


    private void OnGUI()
    {
        if (!NetworkManager.Singleton)
            return;
        GUILayout.BeginArea(new Rect(10, 10, 200, 300));
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
            StatusLabels();
        GUILayout.EndArea();
    }

    private void StatusLabels()
    {
        string mode = "";
        if (GameSetting.instance.isLocalGame) mode = "Local";
        else if (NetworkManager.Singleton.IsHost) mode = "Host";
        else mode = "Client";
        if (!GameSetting.instance.isLocalGame)
        {
            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        }
        GUILayout.Label("Mode: " + mode);
        if (GUILayout.Button("Get current player"))
        {
            Debug.Log(GetCurrentPlayer().playerName);
        }
    }

}