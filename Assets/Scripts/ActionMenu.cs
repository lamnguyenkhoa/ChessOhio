using System.Collections.Generic;
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
    public Button specialPromoButton;


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
        infoButton.gameObject.SetActive(true); // Info button is alway available
        moveButton.gameObject.SetActive(false);
        invertButton.gameObject.SetActive(false);
        combineButton.gameObject.SetActive(false);
        specialPromoButton.gameObject.SetActive(false);

        if (currentSelectPiece.team == GameManager.instance.teamTurn && !currentSelectPiece.lockedControl && !Chessboard.instance.pauseGame)
        {
            if (GameSetting.instance.isLocalGame || GameManager.instance.teamTurn == GameManager.instance.GetCurrentPlayer().team)
            {
                // Only display special action button if certain conditions are met
                moveButton.gameObject.SetActive(true);
                if (GameRule.instance.invertDict.ContainsKey(currentType))
                    invertButton.gameObject.SetActive(true);

                if (GameRule.instance.combineDict.ContainsKey(currentType))
                    combineButton.gameObject.SetActive(true);

                if (GameRule.instance.spPromoDict.ContainsKey(currentType))
                {
                    specialPromoButton.gameObject.SetActive(true);
                    specialPromoButton.GetComponent<Button>().interactable = false;
                    RuleCardSO rule = GameRule.instance.spPromoDict[currentSelectPiece.type];
                    if (GameRule.instance.ResolveSpecialPromoCondition(currentSelectPiece, rule))
                    {
                        specialPromoButton.GetComponent<Button>().interactable = true;
                    }
                }
            }
        }
    }

    public void OnInfoButton()
    {
        GameManager.instance.OpenInfoWindow(currentSelectPiece);
        currentSelectPiece = null;
    }

    public void OnMoveButton()
    {
        Chessboard.instance.SetCurrentlyDraggingPiece(currentSelectPiece);
        GameManager.instance.CloseActionMenu();
        currentSelectPiece = null;
    }

    public void OnInvertButton()
    {
        RuleCardSO rule = GameRule.instance.invertDict[currentSelectPiece.type];
        if (currentSelectPiece.team == GameManager.instance.teamTurn)
        {
            SpecialActionHandler.instance.TransformPiece(currentSelectPiece, rule.invertAfter);
            GameManager.instance.CloseActionMenu();
            currentSelectPiece = null;
        }
    }

    public void OnCombineButton()
    {
        RuleCardSO rule = GameRule.instance.combineDict[currentSelectPiece.type];
        if (currentSelectPiece.team == GameManager.instance.teamTurn)
        {
            GameRule.instance.StartCombineMode(rule, currentSelectPiece);
            GameManager.instance.CloseActionMenu();
            List<PieceType> typesToHighlight = GameRule.instance.GetPieceTypeToHighlightInCombineMode();
            foreach (PieceType type in typesToHighlight)
            {
                Chessboard.instance.HighlightCombinableChessPieces(type, GameManager.instance.teamTurn);
            }
            currentSelectPiece = null;
        }
    }

    public void OnSpecialPromoButton()
    {
        RuleCardSO rule = GameRule.instance.spPromoDict[currentSelectPiece.type];
        if (currentSelectPiece.team == GameManager.instance.teamTurn)
        {
            SpecialActionHandler.instance.TransformPiece(currentSelectPiece, rule.promoAfter);
            GameManager.instance.CloseActionMenu();
            currentSelectPiece = null;
        }
    }
}
