using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Currently it's act more like a LobbyManager than a GameSetting. Will do
/// refactor later (when I have implemented a Game setting menu).
/// </summary>
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

    public bool isLocalGame = true;
    public string m_SceneName = "Ingame";
    public static GameSetting instance;
    public NetworkVariable<bool> hostConnected;
    public NetworkVariable<bool> clientConnected;

    [Header("Settings")]
    public PieceTeam hostChosenTeam = PieceTeam.WHITE;
    private string ipAddress = "127.0.0.1";
    public int turnForNewRule = 5;
    public bool showToolTip = false;
    public float bgmVol = 1f;
    public float soundVol = 1f;

    public GameObject settingMenu;

    [Header("Component")]
    private AudioSource bgmAudioSource;
    private AudioSource soundAudioSource;

    [Header("Lobby stuff")]
    public Button whiteTeamButton;
    public Button blackTeamButton;
    public Button hostButton;
    public Button connectButton;
    public TextMeshProUGUI versionText;
    [SerializeField] private GameObject[] hideIfWebGL;
    public GameObject localWindow;
    public GameObject LANWindow;
    public GameObject creditWindow;

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
        if (versionText)
        {
            versionText.text = $"Version {Application.version}";
        }
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // Hide LAN game buttons
            foreach (GameObject go in hideIfWebGL)
            {
                go.SetActive(false);
            }
        }

        bgmAudioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
        soundAudioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        SceneManager.activeSceneChanged += ChangedActiveScene;
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
        bgmAudioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
        soundAudioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        bgmAudioSource.volume = bgmVol;
        soundAudioSource.volume = soundVol;
        GameObject.Find("Canvas/SettingButton").GetComponent<Button>().onClick.AddListener(OnSettingMenuButton);
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
            BindDisconnectHandlerClientRpc();
            LoadIngameSceneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void LoadIngameSceneServerRpc()
    {
        NetworkManager.SceneManager.LoadScene(m_SceneName, LoadSceneMode.Single);
    }

    public void OnStartLocalButton()
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

    public void ToggleLocalWindow()
    {
        LANWindow.SetActive(false);
        creditWindow.SetActive(false);
        ChangeTeam((int)PieceTeam.WHITE);
        localWindow.SetActive(!localWindow.activeSelf);
    }

    public void ToggleLANWindow()
    {
        localWindow.SetActive(false);
        creditWindow.SetActive(false);
        ChangeTeam((int)PieceTeam.WHITE);
        LANWindow.SetActive(!LANWindow.activeSelf);
    }

    public void OnQuitButton()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.OpenURL("about:blank");
        }
        else
        {
            Application.Quit();
        }
    }

    public void ToggleCreditButton()
    {
        LANWindow.SetActive(false);
        localWindow.SetActive(false);
        creditWindow.SetActive(!creditWindow.activeSelf);
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
        if (isLocalGame)
        {
            return;
        }
        GUILayout.BeginArea(new Rect(10, 10, 100, 200));
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
            StatusLabels();
        GUILayout.EndArea();
    }

    private void StatusLabels()
    {
        if (isLocalGame)
        {
            return;
        }
        // Only show Status in Lobby
        if (SceneManager.GetActiveScene().name != "Lobby")
        {
            return;
        }
        string mode = "";
        if (NetworkManager.Singleton.IsHost) mode = "Host";
        else mode = "Client";

        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("Port: " +
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port);
        if (GUILayout.Button("Disconnect"))
        {
            NetworkManager.Singleton.Shutdown();
            hostButton.interactable = true;
            connectButton.interactable = true;
        }
    }

    private void ClientDisconected(ulong id)
    {
        GameManager.instance.ShowDisconnectScreen();
    }

    private IEnumerator PollServerConnection()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (!NetworkManager.Singleton.IsConnectedClient)
            {
                GameManager.instance.ShowDisconnectScreen();
                yield break;
            }
        }
    }

    [ClientRpc]
    private void BindDisconnectHandlerClientRpc()
    {
        if (IsHost)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += ClientDisconected;
        }
        else
        {
            StartCoroutine(PollServerConnection());
        }
    }

    public void OnToggleTooltip(bool change)
    {
        showToolTip = change;
    }

    public void OnMusicSliderChanged(float vol)
    {
        bgmVol = vol;
        bgmAudioSource.volume = bgmVol;
    }

    public void OnSoundSliderChanged(float vol)
    {
        soundVol = vol;
        soundAudioSource.volume = soundVol;
    }

    public void OnSettingMenuButton()
    {
        if (Chessboard.instance)
        {
            if (settingMenu.activeSelf)
            {
                Chessboard.instance.disableRaycastCount -= 1;
            }
            else
            {
                Chessboard.instance.disableRaycastCount += 1;
            }
        }
        settingMenu.SetActive(!settingMenu.activeSelf);
    }

}
