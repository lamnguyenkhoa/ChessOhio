using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum RuleType
{
    NONE = 0,
    // Unit-related rule
    // Easy-to-use transform a unit into another one
    INVERT_RULE = 1,
    // Use at least 2 unit to make one
    COMBINE_RULE = 2,
    // Transform after doing something special. Can transform at a later time, doesn't have to be immediately.
    SPECIAL_PROMO_RULE = 3,
    // Modify base rule. Something like knight can only move to black square, or pawn can capture front.
    MODIFICATION_RULE = 4,
    // UNIQUE: New rule, such as Monopoly housing, throw dice or move outside grid
    UNIQUE_RULE = 5
}

[CreateAssetMenu(fileName = "RuleCard", menuName = "ScriptableObjects/RuleCard")]
public class RuleCardSO : ScriptableObject
{
    public string ruleName;
    [TextArea(1, 5)]
    public string description;
    public RuleType type;
    public Image image;
}
