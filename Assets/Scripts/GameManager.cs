using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;
    public NetworkVariable<bool> hostConnected = new NetworkVariable<bool>();
    public NetworkVariable<bool> clientConnected = new NetworkVariable<bool>();
    public bool isLocalGame = false;

    public static GameManager getInstance()
    {
        if (!instance)
        {
            instance = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        return instance;
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

    public void HostConnect()
    {
        hostConnected.Value = true;
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
            GameObject.Find("Board").GetComponent<Chessboard>().StartGame(false);
            RpcHandler.getInstance().StartGameClientRpc();
        }
    }

    private void StartLocalGame()
    {
        GameObject.Find("Board").GetComponent<Chessboard>().StartGame(true);
    }


    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
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
    }

}