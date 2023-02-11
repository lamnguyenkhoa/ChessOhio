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
            GameObject.Find("Main Camera (White)").GetComponent<Camera>().enabled = true;
            GameObject.Find("Main Camera (White)").GetComponent<AudioListener>().enabled = true;
            GameObject.Find("Main Camera (Black)").GetComponent<Camera>().enabled = false;
            GameObject.Find("Main Camera (Black)").GetComponent<AudioListener>().enabled = false;
        }
        else
        {
            GameObject.Find("Main Camera (White)").GetComponent<Camera>().enabled = false;
            GameObject.Find("Main Camera (White)").GetComponent<AudioListener>().enabled = false;
            GameObject.Find("Main Camera (Black)").GetComponent<Camera>().enabled = true;
            GameObject.Find("Main Camera (Black)").GetComponent<AudioListener>().enabled = true;
        }
    }

}