using UnityEngine;

[CreateAssetMenu(fileName = "ChessPieceProfile", menuName = "ScriptableObjects/ChessPieceProfile")]
public class ChessPieceProfileSO : ScriptableObject
{
    public string pieceName;
    [TextArea(1, 5)]
    public string description;
    public PieceType type;
    public ChessPieceProfileSO[] possiblePromoProfiles;
    // public SpecialAction[] specialAction;
}