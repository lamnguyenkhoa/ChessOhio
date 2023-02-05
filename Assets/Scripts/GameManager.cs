using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class GameManager : NetworkBehaviour
    {
        public NetworkVariable<bool> hostConnected = new NetworkVariable<bool>();
        public NetworkVariable<bool> clientConnected = new NetworkVariable<bool>();

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
                GameObject.Find("Board").GetComponent<Chessboard>().StartGame();
            }
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
            {
                StartButtons();
            }
            else
            {
                StatusLabels();
            }

            GUILayout.EndArea();
        }

        void StartButtons()
        {
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

        void StatusLabels()
        {
            var mode = NetworkManager.Singleton.IsHost ?
                "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

            GUILayout.Label("Transport: " +
                NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
            GUILayout.Label("Mode: " + mode);
        }
    }
}