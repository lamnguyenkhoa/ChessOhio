using Unity.Netcode;
using UnityEngine;

public class ChessPlayer : NetworkBehaviour
{
    public PieceTeam team;
    public string playerName;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;
        if (NetworkManager.Singleton.IsHost)
        {
            // Host connection is set to true inside GameManager already
            team = PieceTeam.WHITE;
            playerName = "HosterPro2000";
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            GameManager.getInstance().ClientConnectServerRpc();
            team = PieceTeam.BLACK;
            playerName = "Xx_LmaoClient_xX";
        }
    }

}