using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameRule : MonoBehaviour
{
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
    private const int N_DRAW = 3;
    public GameObject ruleCardUI;
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
    }

    public void DrawRuleCard()
    {
        RuleCardSO[] drawedCards = { };
        if (activeRulePool.Count >= N_DRAW)
        {
            drawedCards = MyHelper.GetRandomItems(activeRulePool, N_DRAW);
        }
        else
        {
            Debug.Log("Not enough card to draw");
            return;
        }

        for (int i = 0; i < N_DRAW; i++)
        {
            RuleCard ruleCard = ruleCardUI.transform.GetChild(i).GetComponent<RuleCard>();
            ruleCard.profile = drawedCards[i];
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
        ruleCardUI.SetActive(true);
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

    public void CloseRuleCardMenu()
    {
        Chessboard.instance.pauseGame = false;
        ruleCardUI.SetActive(false);
    }

    public void RuleImplementinator(RuleCardSO chosenRule)
    {
        if (chosenRule.type == RuleType.INVERT_RULE)
        {
            invertDict.Add(chosenRule.invertBefore, chosenRule.invertAfter);
        }
    }
}
