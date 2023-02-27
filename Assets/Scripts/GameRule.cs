using System;
using System.Collections.Generic;
using UnityEngine;

public class GameRule : MonoBehaviour
{
    public GameObject ruleCardParentUI;
    public GameObject ruleCardPrefab;
    public static GameRule instance;
    public Dictionary<PieceType, PieceType> invertDict = new Dictionary<PieceType, PieceType>();
    public Dictionary<PieceType, PieceType> combineDict = new Dictionary<PieceType, PieceType>();
    // Every rulecardSO.
    public RuleCardSO[] availableRule;
    // Some default starting rulecardSO that can be draw. If certain conditions meet, some rule card
    // will be added from the `availableRule` or removed from the active pool.
    public List<RuleCardSO> activeRulePool = new List<RuleCardSO>();
    // Currently only for debugging purpose, since the 2 dicts above already used to check for logic.
    public List<RuleCardSO> activatedRule = new List<RuleCardSO>();
    public List<PieceType> activeUnits = new List<PieceType>();

    private const int DEFAULT_N_DRAW = 3;


    // Because White move first, Black get to choose rule first
    public PieceTeam teamToChoseRule = PieceTeam.BLACK;


    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        PieceType[] baseUnits = new PieceType[] { PieceType.PAWN, PieceType.ROOK, PieceType.KNIGHT, PieceType.BISHOP, PieceType.KING, PieceType.QUEEN };
        activeUnits.AddRange(baseUnits);
    }

    public void DrawRuleCard()
    {
        int n_draw = activeRulePool.Count >= DEFAULT_N_DRAW ? DEFAULT_N_DRAW : activeRulePool.Count;
        if (n_draw > 0)
        {
            RuleCardSO[] drawedCards = MyHelper.GetRandomItems(activeRulePool, n_draw);
            for (int i = 0; i < drawedCards.Length; i++)
            {
                RuleCard ruleCard = Instantiate(ruleCardPrefab, ruleCardParentUI.transform).GetComponent<RuleCard>();
                ruleCard.profile = drawedCards[i];
            }
        }
        else
        {
            CloseRuleCardMenu();
        }
    }

    public void OpenRuleCardMenu()
    {
        // Calculate team to choose rule
        if (activatedRule.Count % 2 == 0)
            teamToChoseRule = PieceTeam.BLACK;
        else
            teamToChoseRule = PieceTeam.WHITE;
        Chessboard.instance.pauseGame = true;
        DrawRuleCard();
        ruleCardParentUI.SetActive(true);
    }

    public void CloseRuleCardMenu()
    {
        Chessboard.instance.pauseGame = false;
        foreach (Transform child in ruleCardParentUI.transform)
            GameObject.Destroy(child.gameObject);
        ruleCardParentUI.SetActive(false);
    }

    /// <summary>
    /// Player chose this rule. Implement rule into current game and notify other player
    /// if playing LAN game.
    /// </summary>
    /// <param name="chosenRule"></param>
    /// <param name="sendNotification">Notify other player that you chose this rule</param>
    public void ChoseThisRule(RuleCardSO chosenRule, bool sendNotification = false)
    {
        Debug.Log($"Selected {chosenRule.ruleName}");
        activeRulePool.Remove(chosenRule);
        activatedRule.Add(chosenRule);
        RuleImplementinator(chosenRule);
        if (sendNotification)
        {
            int ruleCardId = Array.FindIndex(availableRule, ruleCard => ruleCard.ruleName == chosenRule.ruleName);
            GameManager.instance.NotifyChosenRuleCard(ruleCardId);
        }
    }

    public void RuleImplementinator(RuleCardSO chosenRule)
    {

        if (chosenRule.type == RuleType.INVERT_RULE)
        {
            invertDict.Add(chosenRule.invertBefore, chosenRule.invertAfter);
            if (!activeUnits.Contains(chosenRule.invertAfter))
            {
                activeUnits.Add(chosenRule.invertAfter);
            }
        }
    }
}
