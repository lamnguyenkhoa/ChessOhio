using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public Button button;
    public AudioSource audioSource;

    public void Start()
    {
        // Sync setting
        if (!audioSource)
            return;
        if (GameSetting.instance.muteBGM)
            Mute();
        else
            Unmute();
    }

    public void OnMuteButton()
    {
        if (!audioSource)
            return;
        if (audioSource.mute)
            Unmute();
        else
            Mute();
    }

    public void Mute()
    {
        audioSource.mute = true;
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Unmute BGM";
        GameSetting.instance.muteBGM = true;
    }

    public void Unmute()
    {
        audioSource.mute = false;
        button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Mute BGM";
        GameSetting.instance.muteBGM = false;
    }
}
