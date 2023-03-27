using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "RuleCard", menuName = "ScriptableObjects/RuleCard")]
public class RuleCardSO : ScriptableObject
{
    public string ruleName;
    [TextArea(1, 5)]
    public string description;
    public RuleType type;
    public Sprite sprite;
    // Unlock these rules if this rule card selected (prerequisite)
    public RuleCardSO[] unlockRules;

    // Prevent these rule cards from drawed / Removed from pool (conflict)
    public RuleCardSO[] conflictRules;


    [ShowIf("type", RuleType.INVERT_RULE)]
    public PieceType invertBefore;
    [ShowIf("type", RuleType.INVERT_RULE)]
    public PieceType invertAfter;

    [ShowIf("type", RuleType.COMBINE_RULE)]
    public PieceType combineStart;
    [ShowIf("type", RuleType.COMBINE_RULE)]
    public PieceType combineResult;
    [ShowIf("type", RuleType.COMBINE_RULE)]
    public PieceType[] combineMaterials;

    [ShowIf("type", RuleType.SPECIAL_PROMO_RULE)]
    public PieceType promoBefore;
    [ShowIf("type", RuleType.SPECIAL_PROMO_RULE)]
    public PieceType promoAfter;
    [ShowIf("type", RuleType.SPECIAL_PROMO_RULE)]
    public UniqueRuleCode promoRuleCode;

    [ShowIf("type", RuleType.CONSTRAINT_RULE)]
    public UniqueRuleCode constraintRuleCode;

    [ShowIf("type", RuleType.OHIO_RULE)]
    public UniqueRuleCode ohioRuleCode;

}
