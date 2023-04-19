using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "RuleCard", menuName = "ScriptableObjects/RuleCard")]
public class RuleCardSO : ScriptableObject
{
    public string ruleName;
    [TextArea(1, 5)]
    public string description;
    public string localizedNameKey;
    public string localizedDescriptionKey;
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


    /// <summary>
    /// Remove whitespace and colon (:), stick them together and turn into camelCase.
    /// Ex: "Combine: Archbishop" -> "combineArchbishop".
    /// </summary>
    /// <returns></returns>
    private string GetFormattedRuleName()
    {
        string[] words = ruleName.Split(' ', ':');
        string result = "";

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            if (word.Length > 0)
            {
                if (i == 0)
                {
                    result += char.ToLower(word[0]) + word.Substring(1);
                }
                else
                {
                    result += char.ToUpper(word[0]) + word.Substring(1);
                }
            }
        }
        return result;
    }


    /// <summary>
    /// Get the name key of this chess piece profile. If not specified, it will
    /// guess it as [formattedRuleName]Key.
    /// </summary>
    /// <returns></returns>
    public string GetLocalizedNameKey()
    {
        if (localizedNameKey != "")
        {
            return localizedNameKey;
        }
        return GetFormattedRuleName() + "Key";
    }

    /// <summary>
    /// Get rule name after localized. If failed, it will use English, specified in the ScriptableObject.
    /// </summary>
    /// <returns></returns>
    public string GetLocalizedName()
    {
        AsyncOperationHandle<string> op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RuleCardText", GetLocalizedNameKey());
        if (op.IsDone)
        {
            if (op.Result.Contains("No translation"))
            {
                return ruleName;
            }
            return op.Result;
        }
        else
            return ruleName;
    }

    /// <summary>
    /// Get the description key of this rule profile. If not specified, it will
    /// guess it as [formattedRuleName]DescriptionKey.
    /// </summary>
    /// <returns></returns>
    public string GetLocalizedDescriptionKey()
    {
        if (localizedDescriptionKey != "")
        {
            return localizedDescriptionKey;
        }
        return GetFormattedRuleName() + "DescriptionKey";
    }

    /// <summary>
    /// Get rule description after localized. If failed, it will use English, specified in the ScriptableObject.
    /// </summary>
    /// <returns></returns>
    public string GetLocalizedDescription()
    {
        AsyncOperationHandle<string> op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("RuleCardText", GetLocalizedDescriptionKey());
        if (op.IsDone)
        {
            if (op.Result.Contains("No translation"))
            {
                return description;
            }
            return op.Result;
        }
        else
            return description;
    }

}
