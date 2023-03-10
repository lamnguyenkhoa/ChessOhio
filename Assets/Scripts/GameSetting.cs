using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;

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
    public PieceTeam hostChosenTeam = PieceTeam.WHITE;
    public int turnForNewRule = 5;
    public bool showToolTip = false;

    [Header("Lobby stuff")]
    public Button whiteTeamButton;
    public Button blackTeamButton;
    public Button hostButton;
    public Button connectButton;
    public TextMeshProUGUI versionText;
    [SerializeField] private GameObject[] hideIfWebGL;


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

    private void Start()
    {
        versionText.text = $"Version {Application.version}";
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Hide LAN game buttons
            foreach (GameObject go in hideIfWebGL)
            {
                go.SetActive(false);
            }
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
        NetworkManager.Shutdown();
        Destroy(NetworkManager.gameObject);
        SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
    }

    public void OnHostButton()
    {
        isLocalGame = false;
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (!Application.isEditor)
            transport.ConnectionData.Address = GetLocalIPAddress();
        NetworkManager.Singleton.StartHost();
        hostButton.interactable = false;
        connectButton.interactable = false;
    }
    public void OnConnectButton()
    {
        isLocalGame = false;
        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = ipAddress;
        NetworkManager.Singleton.StartClient();
        hostButton.interactable = false;
        connectButton.interactable = false;
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

    private void OnGUI()
    {
        if (!isLocalGame)
        {
            GUILayout.BeginArea(new Rect(10, 10, 100, 200));
            if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
                StatusLabels();
            GUILayout.EndArea();
        }
    }

    private void StatusLabels()
    {
        string mode = "";
        if (isLocalGame) mode = "Local";
        else
        {
            if (NetworkManager.Singleton.IsHost) mode = "Host";
            else mode = "Client";
        }

        GUILayout.Label("Mode: " + mode);
        if (!isLocalGame)
        {
            GUILayout.Label("Port: " +
                NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port);
        }
        if (GUILayout.Button("Disconnect"))
        {
            NetworkManager.Singleton.Shutdown();
            hostButton.interactable = true;
            connectButton.interactable = true;
        }
    }
}
