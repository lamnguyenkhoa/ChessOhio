using UnityEngine;

[CreateAssetMenu(fileName = "ChessPieceProfile", menuName = "ScriptableObjects/ChessPieceProfile")]
public class ChessPieceProfileSO : ScriptableObject
{
    public string pieceName;
    [TextArea(2, 5)]
    public string description;
    public PieceType type;
    public Sprite sprite;
    public ChessPieceProfileSO[] possiblePromoProfiles;
}