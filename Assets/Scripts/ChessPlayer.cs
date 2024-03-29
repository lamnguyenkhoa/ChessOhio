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
            foreach (GameObject go in GameManager.instance.whiteTeamStuff)
                go.SetActive(true);
            foreach (GameObject go in GameManager.instance.blackTeamStuff)
                go.SetActive(false);
        }
        else
        {
            foreach (GameObject go in GameManager.instance.whiteTeamStuff)
                go.SetActive(false);
            foreach (GameObject go in GameManager.instance.blackTeamStuff)
                go.SetActive(true);
        }
    }

}