using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{
    public class ChessPlayer : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            // Host connection is set to true inside GameManager already
            if (IsOwner && NetworkManager.Singleton.IsClient)
            {
                GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
                manager.ClientConnectServerRpc();
            }
        }
    }
}