using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

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

    public override void OnNetworkSpawn()
    {
        if (IsHost)
        {
            hostConnected.Value = true;
            clientConnected.Value = false;
            hostConnected.OnValueChanged += OnConnectionChanged;
            clientConnected.OnValueChanged += OnConnectionChanged;
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
        NetworkManager.Singleton.StartHost();
    }
    public void OnClientButton()
    {
        isLocalGame = false;
        NetworkManager.Singleton.StartClient();
    }

    public void OnQuitButton()
    {
        Application.Quit();
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
