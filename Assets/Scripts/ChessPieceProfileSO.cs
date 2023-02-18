using UnityEngine;

/// <summary>
/// Action in ActionMenu, different from SpecialMove. Think of it like
/// active vs passive skill.
/// </summary>
public enum SpecialAction
{
    NONE = 0,
    INVERT = 1
}

[CreateAssetMenu(fileName = "ChessPieceProfile", menuName = "ScriptableObjects/ChessPieceProfile")]
public class ChessPieceProfileSO : ScriptableObject
{
    public string pieceName;
    [TextArea(1, 5)]
    public string description;
    public PieceType type;
    public ChessPieceProfileSO[] possiblePromoProfiles;
    public SpecialAction[] specialAction;
}