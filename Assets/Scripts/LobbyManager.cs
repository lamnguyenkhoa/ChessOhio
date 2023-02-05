using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LobbyManager : MonoBehaviour
{
    public void OnLocalButton()
    {
        SceneManager.LoadScene("Ingame", LoadSceneMode.Single);
    }

    public void OnHostButton()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void OnClientButton()
    {
        NetworkManager.Singleton.StartClient();
    }
}
