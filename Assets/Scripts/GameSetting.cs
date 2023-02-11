using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;

public class GameSetting : NetworkBehaviour
{
    /// INFO: You can remove the #if UNITY_EDITOR code segment and make SceneName public,
    /// but this code assures if the scene name changes you won't have to remember to
    /// manually update it.
#if UNITY_EDITOR
    public UnityEditor.SceneAsset SceneAsset;
    private void OnValidate()
    {
        if (SceneAsset != null)
        {
            m_SceneName = SceneAsset.name;
        }
    }
#endif

    public bool isLocalGame = false;
    public string m_SceneName = "Ingame";
    public static GameSetting instance;
    public NetworkVariable<bool> hostConnected;
    public NetworkVariable<bool> clientConnected;
    private string ipAddress = "127.0.0.1";
    private ushort port = 7777;
    public PieceTeam hostChosenTeam = PieceTeam.WHITE;
    public Button whiteTeamButton;
    public Button blackTeamButton;


    private void Awake()
    {
        if (!instance)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else
        {
            Destroy(this);
        }
    }

    public string GetLocalIPAddress()
    {
        string myAddressLocal = "Not found";
        //Get the local IP
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                myAddressLocal = ip.ToString();
                break;
            }
        }
        return myAddressLocal;
    }

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            hostConnected.OnValueChanged += OnConnectionChanged;
            clientConnected.OnValueChanged += OnConnectionChanged;
            hostConnected.Value = true;
            clientConnected.Value = false;
        }
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
            if (IsHost)
            {
                NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
            }
        }
    }

    public void OnLocalButton()
    {
        isLocalGame = true;
        NetworkManager.Singleton.StartHost();
        NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
    }

    public void OnHostButton()
    {
        isLocalGame = false;
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (!Application.isEditor)
            transport.ConnectionData.Address = GetLocalIPAddress();
        NetworkManager.Singleton.StartHost();
    }
    public void OnClientButton()
    {
        isLocalGame = false;
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ipAddress;
        transport.ConnectionData.Port = port;
        NetworkManager.Singleton.StartClient();
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }

    public void ChangeTeam(int team)
    {
        if (team == 0)
        {
            hostChosenTeam = PieceTeam.WHITE;
            whiteTeamButton.interactable = false;
            blackTeamButton.interactable = true;
        }
        else
        {
            hostChosenTeam = PieceTeam.BLACK;
            whiteTeamButton.interactable = true;
            blackTeamButton.interactable = false;
        }
    }

    public void ChangeIPAddress(string input)
    {
        ipAddress = input;
    }

    public void ChangePort(string input)
    {
        port = ushort.Parse(input);
    }

    private void OnGUI()
    {
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
            GUILayout.Label("Port: " +
                NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port);
        }
        GUILayout.Label("Mode: " + mode);
        if (GUILayout.Button("Disconnect"))
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
