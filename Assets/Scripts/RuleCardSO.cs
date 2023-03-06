using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "RuleCard", menuName = "ScriptableObjects/RuleCard")]
public class RuleCardSO : ScriptableObject
{
    public string ruleName;
    [TextArea(1, 5)]
    public string description;
    public RuleType type;
    public Image image;
    // Unlock these rules if this rule card selected (prerequisite)
    public RuleCardSO[] unlockRules;


    // Should have conditional display these field depend on this rule card's rule type.
    [Header("Invert")]
    public PieceType invertBefore;
    public PieceType invertAfter;
    [Header("Combine")]
    public PieceType combineStart;
    public PieceType combineResult;
    public PieceType[] combineMaterials;

    [Header("Special Promo")]
    public PieceType promoBefore;
    public PieceType promoAfter;
    public UniqueRuleCode promoRuleCode;


}
