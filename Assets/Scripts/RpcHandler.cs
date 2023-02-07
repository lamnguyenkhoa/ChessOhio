using Unity.Netcode;
using UnityEngine;

public class RpcHandler : NetworkBehaviour
{
    public static RpcHandler instance;

    public static RpcHandler getInstance()
    {
        if (!instance)
        {
            instance = GameObject.Find("GameManager").GetComponent<RpcHandler>();
        }
        return instance;
    }

    [ClientRpc]
    public void StartGameClientRpc()
    {
        if (!IsHost)
        {
            GameObject.Find("Board").GetComponent<Chessboard>().StartGame(false);
        }
    }
}