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

    private void Awake()
    {
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        if (currentSelectPiece == null) return;
        transform.Find("PieceName").GetComponent<TextMeshProUGUI>().text = chessPiece.profile.pieceName;

        PieceType currentType = currentSelectPiece.type;
        if (GameRule.instance.invertDict.ContainsKey(currentType))
            invertButton.gameObject.SetActive(true);
        else
            invertButton.gameObject.SetActive(false);

        if (GameRule.instance.combineDict.ContainsKey(currentType))
            combineButton.gameObject.SetActive(true);
        else
            combineButton.gameObject.SetActive(false);
    }


    public void OnInvertButton()
    {
        if (currentSelectPiece.team == GameManager.instance.teamTurn.Value)
        {
            SpecialActionHandler.instance.TransformPiece(currentSelectPiece, PieceType.NIGHTRIDER);
            GameManager.instance.CloseActionMenu();
        }
    }
}
