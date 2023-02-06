using Unity.Netcode;
using UnityEngine;

public class ChessPlayer : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        // Host connection is set to true inside GameManager already,
        // so we only deal with Client here.
        if (IsOwner && NetworkManager.Singleton.IsClient)
        {
            GameManager.getInstance().ClientConnectServerRpc();
        }
    }
}