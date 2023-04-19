using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "ChessPieceProfile", menuName = "ScriptableObjects/ChessPieceProfile")]
public class ChessPieceProfileSO : ScriptableObject
{
    public string pieceName;
    [TextArea(2, 5)]
    public string description;
    public string localizedNameKey;
    public string localizedDescriptionKey;
    public PieceType type;
    public Sprite sprite;
    public ChessPieceProfileSO[] possiblePromoProfiles;


    /// <summary>
    /// Get the name key of this chess piece profile. If not specified, it will
    /// guess it as [pieceName]Key.
    /// </summary>
    /// <returns></returns>
    public string GetLocalizedNameKey()
    {
        if (localizedNameKey != "")
        {
            return localizedNameKey;
        }
        return pieceName.ToLower() + "Key";
    }

    /// <summary>
    /// Get piece name after localized. If failed, it will use English, specified in the ScriptableObject.
    /// </summary>
    /// <returns></returns>
    public string GetLocalizedName()
    {
        AsyncOperationHandle<string> op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("PieceProfileText", GetLocalizedNameKey());
        if (op.IsDone)
        {
            if (op.Result.Contains("No translation"))
            {
                return pieceName;
            }
            return op.Result;
        }
        else
            return pieceName;
    }

    /// <summary>
    /// Get the description key of this chess piece profile. If not specified, it will
    /// guess it as [pieceName]DescriptionKey.
    /// </summary>
    /// <returns></returns>
    public string GetLocalizedDescriptionKey()
    {
        if (localizedDescriptionKey != "")
        {
            return localizedDescriptionKey;
        }
        return pieceName.ToLower() + "DescriptionKey";
    }

    /// <summary>
    /// Get piece description after localized. If failed, it will use English, specified in the ScriptableObject.
    /// </summary>
    /// <returns></returns>
    public string GetLocalizedDescription()
    {
        AsyncOperationHandle<string> op = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("PieceProfileText", GetLocalizedDescriptionKey());
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