using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public Button button;
    public AudioSource audioSource;

    public void OnMuteButton()
    {
        if (audioSource)
            audioSource.mute = !audioSource.mute;
        if (audioSource.mute)
        {
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Unmute";
        }
        else
        {
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mute";
        }
    }
}
