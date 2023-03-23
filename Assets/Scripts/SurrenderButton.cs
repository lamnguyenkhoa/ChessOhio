using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SurrenderButton : MonoBehaviour
{
    public TextMeshProUGUI surrenderText;
    public GameObject yesButton;
    public GameObject noButton;

    private void Start()
    {
        if (GameSetting.instance.isLocalGame)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnSurrenderButton()
    {
        GetComponent<Button>().interactable = false;
        surrenderText.text = "Confirm?";
        yesButton.SetActive(true);
        noButton.SetActive(true);
    }

    public void OnYesButton()
    {
        PieceTeam teamSurrenderd = GameManager.instance.GetCurrentPlayer().team;
        Chessboard.instance.CheckMate(teamSurrenderd);
        GameManager.instance.NotifySurrender(teamSurrenderd);
    }

    public void OnNoButton()
    {
        GetComponent<Button>().interactable = true;
        surrenderText.text = "Surrender";
        yesButton.SetActive(false);
        noButton.SetActive(false);
    }
}
