using Unity.Netcode;
using UnityEngine;

public enum ClientID
{
    HOST = 0,
    CLIENT = 1
}

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public NetworkVariable<bool> hostConnected;
    public NetworkVariable<bool> clientConnected;
    public NetworkVariable<PieceTeam> teamTurn;


    public bool isLocalGame = false;

    private void Awake()
    {
        teamTurn.Value = PieceTeam.WHITE;
    }

    public static GameManager getInstance()
    {
        if (!instance)
        {
            instance = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        return instance;
    }

    public Chessboard GetChessBoard()
    {
        return GameObject.Find("Board").GetComponent<Chessboard>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            hostConnected.Value = false;
            hostConnected.Value = false;
            hostConnected.OnValueChanged += OnConnectionChanged;
            clientConnected.OnValueChanged += OnConnectionChanged;
        }
    }

    public ChessPlayer GetCurrentPlayer()
    {
        ulong id = NetworkManager.Singleton.LocalClientId;
        return NetworkManager.SpawnManager.GetPlayerNetworkObject(id).GetComponent<ChessPlayer>();
    }

    public void HostConnect()
    {
        hostConnected.Value = true;
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

    [ServerRpc(RequireOwnership = false)]
    public void ClientConnectServerRpc()
    {
        clientConnected.Value = true;
    }

    private void OnConnectionChanged(bool previous, bool current)
    {
        if (hostConnected.Value && clientConnected.Value)
        {
            GetChessBoard().StartGame(false);
            StartGameClientRpc();
        }
    }

    private void StartLocalGame()
    {
        GetChessBoard().StartGame(true);
    }

    public void ResetGame()
    {
    }

    [ClientRpc]
    public void StartGameClientRpc()
    {
        if (!IsHost)
        {
            GetChessBoard().StartGame(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchTurnServerRpc()
    {
        if (teamTurn.Value == PieceTeam.WHITE)
        {
            teamTurn.Value = PieceTeam.BLACK;
        }
        else
        {
            teamTurn.Value = PieceTeam.WHITE;
        }
    }


    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 200, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            StartButtons();
        else
            StatusLabels();
        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        if (GUILayout.Button("Local game"))
        {
            NetworkManager.Singleton.StartHost();
            isLocalGame = true;
            StartLocalGame();
        }
        if (GUILayout.Button("Host"))
        {
            NetworkManager.Singleton.StartHost();
            HostConnect();
        }
        if (GUILayout.Button("Client"))
        {
            NetworkManager.Singleton.StartClient();
        }
    }

    private void StatusLabels()
    {
        string mode = "";
        if (isLocalGame) mode = "Local";
        else if (NetworkManager.Singleton.IsHost) mode = "Host";
        else mode = "Client";
        if (!isLocalGame)
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