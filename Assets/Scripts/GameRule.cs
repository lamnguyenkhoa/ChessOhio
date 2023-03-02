using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


/// <summary>
/// Stuff that can't be coded using variable and must be assessed 
/// on a case-by-case basic
/// </summary>
public enum UniqueRuleCode
{
    NONE,
    SP_PROMO_BISHOP_ARCHBISHOP,
    SP_PROMO_KNIGHT_CAVALIER
}

public class GameRule : MonoBehaviour
{
    public GameObject ruleCardParentUI;
    public GameObject ruleCardPrefab;
    public static GameRule instance;
    public Dictionary<PieceType, RuleCardSO> invertDict = new Dictionary<PieceType, RuleCardSO>();
    public Dictionary<PieceType, RuleCardSO> combineDict = new Dictionary<PieceType, RuleCardSO>();
    public Dictionary<PieceType, RuleCardSO> spPromoDict = new Dictionary<PieceType, RuleCardSO>();
    // Every rulecardSO.
    public RuleCardSO[] availableRule;
    // Some default starting rulecardSO that can be draw. If certain conditions meet, some rule card
    // will be added from the `availableRule` or removed from the active pool.
    public List<RuleCardSO> activeRulePool = new List<RuleCardSO>();
    public List<RuleCardSO> activatedRule = new List<RuleCardSO>();
    public List<PieceType> activeUnits = new List<PieceType>();
    private const int DEFAULT_N_DRAW = 3;
    // Because White move first, Black get to choose rule first
    public PieceTeam teamToChoseRule = PieceTeam.BLACK;
    [Header("Combine")]
    public List<ChessPiece> piecesToCombine = new List<ChessPiece>();
    // If sourcePiece removed from piecesToCombine, sotp combine mode. Also used for combineResult position.
    public ChessPiece sourcePiece;
    private RuleCardSO currentCombineRecipe;


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

    private void Update()
    {

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
        if (!Chessboard.instance.isLocalGame && sendNotification)
        {
            int ruleCardId = Array.FindIndex(availableRule, ruleCard => ruleCard.ruleName == chosenRule.ruleName);
            GameManager.instance.NotifyChosenRuleCard(ruleCardId);
        }
        CloseRuleCardMenu();
    }

    public void RuleImplementinator(RuleCardSO chosenRule)
    {

        if (chosenRule.type == RuleType.INVERT_RULE)
        {
            if (!invertDict.ContainsKey(chosenRule.invertBefore))
            {
                invertDict.Add(chosenRule.invertBefore, chosenRule);
            }
            else
            {
                // Later in development, add some animation to clearly show that
                // the new rule replaced the old rule.
                Debug.Log("Replaced rule");
                invertDict[chosenRule.invertBefore] = chosenRule;
            }
            if (!activeUnits.Contains(chosenRule.invertAfter))
                activeUnits.Add(chosenRule.invertAfter);
        }

        if (chosenRule.type == RuleType.COMBINE_RULE)
        {
            if (!combineDict.ContainsKey(chosenRule.combineStart))
            {
                combineDict.Add(chosenRule.combineStart, chosenRule);
            }
            else
            {
                Debug.Log("Replaced rule");
                combineDict[chosenRule.combineStart] = chosenRule;
            }
            if (!activeUnits.Contains(chosenRule.combineResult))
                activeUnits.Add(chosenRule.combineResult);
        }

        if (chosenRule.type == RuleType.SPECIAL_PROMO_RULE)
        {
            if (!spPromoDict.ContainsKey(chosenRule.promoBefore))
            {
                spPromoDict.Add(chosenRule.promoBefore, chosenRule);
            }
            else
            {
                Debug.Log("Replaced rule");
                spPromoDict[chosenRule.promoBefore] = chosenRule;
            }
            if (!activeUnits.Contains(chosenRule.promoAfter))
                activeUnits.Add(chosenRule.promoAfter);
        }
    }

    public void AddOrRemovePiecesToCombine(ChessPiece cp)
    {
        Vector3 originalLocalPos = Chessboard.instance.GetTileCenter(cp.currentX, cp.currentY);
        if (GameRule.instance.piecesToCombine.Contains(cp))
        {
            // Remove
            cp.SetPosition(originalLocalPos);
            GameRule.instance.piecesToCombine.Remove(cp);
            if (sourcePiece == cp)
            {
                ExitCombineMode();
            }
        }
        else
        {
            // Add
            cp.SetPosition(originalLocalPos + Vector3.up * Chessboard.instance.dragOffset);
            GameRule.instance.piecesToCombine.Add(cp);

            // Check if fulfiled recipe materials
            if (piecesToCombine.Count == currentCombineRecipe.combineMaterials.Length)
            {
                PieceType[] setA = piecesToCombine.Select((piece) => piece.type).ToArray<PieceType>();
                PieceType[] setB = currentCombineRecipe.combineMaterials;
                bool areEquivalent = (setA.Count() == setB.Count()) && !setA.Except(setB).Any();
                if (areEquivalent)
                {
                    ProcessCombine();
                }
            }
        }
    }

    private void ProcessCombine()
    {
        for (int i = 1; i < piecesToCombine.Count; i++)
        {
            Chessboard.instance.ChangePiece(new Vector2Int(piecesToCombine[i].currentX, piecesToCombine[i].currentY), PieceType.NONE, true);
        }
        Chessboard.instance.ChangePiece(new Vector2Int(sourcePiece.currentX, sourcePiece.currentY), currentCombineRecipe.combineResult, true);
        Chessboard.instance.EndTurn(true);
        ExitCombineMode();
    }

    /// <summary>
    /// </summary>
    /// <param name="chosenRule">Contain the combine recipe</param>
    /// <param name="sourcePiece">The piece that start Combine process</param>
    public void StartCombineMode(RuleCardSO chosenRule, ChessPiece sourcePiece)
    {
        this.sourcePiece = sourcePiece;
        currentCombineRecipe = chosenRule;
        Chessboard.instance.combineMode = true;
        GameManager.instance.exitCombineButton.SetActive(true);
        AddOrRemovePiecesToCombine(sourcePiece);
    }

    public void ExitCombineMode()
    {
        currentCombineRecipe = null;
        Chessboard.instance.combineMode = false;
        piecesToCombine.Clear();
        Chessboard.instance.PositionAllPieces(false);
        GameManager.instance.exitCombineButton.SetActive(false);
    }

    public bool ResolveSpecialPromoCondition(ChessPiece cp, RuleCardSO rule)
    {
        if (rule.promoRuleCode == UniqueRuleCode.SP_PROMO_BISHOP_ARCHBISHOP)
        {
            if (Chessboard.instance.turnCount > 30)
                return true;
        }
        if (rule.promoRuleCode == UniqueRuleCode.SP_PROMO_KNIGHT_CAVALIER)
        {
            if (cp.captureHistory.ContainsKey(PieceType.KNIGHT) &&
                cp.captureHistory[PieceType.KNIGHT] >= 1)
                return true;
        }
        return false;
    }
}
