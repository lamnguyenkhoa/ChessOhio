using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour
{
    public ChessPiece currentSelectPiece;
    public Button infoButton;
    public Button moveButton;
    public Button invertButton;
    public Button combineButton;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                GameManager.instance.CloseActionMenu();
            }
        }
    }

    public void Setup(ChessPiece chessPiece)
    {
        this.currentSelectPiece = chessPiece;
        transform.Find("PieceName").GetComponent<TextMeshProUGUI>().text = chessPiece.profile.pieceName;

        PieceType currentType = currentSelectPiece.type;
        infoButton.gameObject.SetActive(true);
        moveButton.gameObject.SetActive(false);
        invertButton.gameObject.SetActive(false);
        combineButton.gameObject.SetActive(false);

        if (currentSelectPiece.team == GameManager.instance.teamTurn.Value)
        {
            if (GameSetting.instance.isLocalGame || GameManager.instance.teamTurn.Value == GameManager.instance.GetCurrentPlayer().team)
            {
                moveButton.gameObject.SetActive(true);
                if (GameRule.instance.invertDict.ContainsKey(currentType))
                    invertButton.gameObject.SetActive(true);

                if (GameRule.instance.combineDict.ContainsKey(currentType))
                    combineButton.gameObject.SetActive(true);
            }
        }
    }

    public void OnInvertButton()
    {
        if (currentSelectPiece.team == GameManager.instance.teamTurn.Value)
        {
            SpecialActionHandler.instance.TransformPiece(currentSelectPiece, GameRule.instance.invertDict[currentSelectPiece.type]);
            GameManager.instance.CloseActionMenu();
        }
    }
}
