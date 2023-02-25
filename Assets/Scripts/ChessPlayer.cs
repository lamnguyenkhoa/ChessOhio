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
            // Host connection is set to true inside GameSetting already
            team = PieceTeam.WHITE;
            playerName = "HosterPro2000";
        }
        else if (NetworkManager.Singleton.IsClient)
        {
            team = PieceTeam.BLACK;

            playerName = "Xx_LmaoClient_xX";
            GameSetting.instance.ClientConnectServerRpc();
        }
        DontDestroyOnLoad(this);
    }

    public void SetTeamAndCamera(PieceTeam hostChosenTeam)
    {
        if (IsHost)
        {
            team = GameSetting.instance.hostChosenTeam;
        }
        else
        {
            if (hostChosenTeam == PieceTeam.BLACK)
            {
                team = PieceTeam.WHITE;
            }
        }

        if (team == PieceTeam.WHITE)
        {
            GameManager.instance.whiteCamera.SetActive(true);
            GameManager.instance.blackCamera.SetActive(false);
        }
        else
        {
            GameManager.instance.whiteCamera.SetActive(false);
            GameManager.instance.blackCamera.SetActive(true);
        }
    }

}