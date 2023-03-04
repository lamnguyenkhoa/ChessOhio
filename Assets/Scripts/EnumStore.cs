public enum ClientID
{
    HOST,
    CLIENT
}

/// <summary>
/// Stuff that can't be coded using variable and must be assessed 
/// on a case-by-case basic
/// </summary>
public enum UniqueRuleCode
{
    NONE,
    SP_PROMO_BISHOP_ARCHBISHOP,
    SP_PROMO_KNIGHT_CAVALIER,
    SP_PROMO_CAVALIER_PALADIN,

}

public enum RuleType
{
    NONE,
    // Unit-related rule
    // Easy-to-use transform a unit into another one
    INVERT_RULE,
    // Use at least 2 unit to make one
    COMBINE_RULE,
    // Transform after doing something special. Can transform at a later time, doesn't have to be immediately.
    SPECIAL_PROMO_RULE,
    // Modify base rule. Something like knight can only move to black square, or pawn can capture front.
    MODIFICATION_RULE,
    // UNIQUE: New rule, such as Monopoly housing, throw dice or move outside grid
    UNIQUE_RULE
}

/// <summary>
/// Action in ActionMenu, different from SpecialMove. Think of it like
/// active vs passive skill.
/// </summary>
public enum SpecialAction
{
    NONE,
    INVERT,
    COMBINE,
    SPECIAL_PROMO,
}

/// <summary>
/// SpecialMove, which is different from SpecialAction. Usually resolved
/// at the end of turn. They are the basic stuff: en passant, castling, promotion.
/// </summary>
public enum SpecialMove
{
    NONE,
    EN_PASSANT,
    CASTLING,
    PROMOTION
}

public enum PieceTeam
{
    WHITE,
    BLACK
}

public enum PieceType
{
    NONE,
    PAWN,
    ROOK,
    KNIGHT,
    BISHOP,
    QUEEN,
    KING,
    NIGHTRIDER,
    DESERTER,
    BUNKER,
    ARCHBISHOP,
    CAVALIER,
    PALADIN,
    EMPEROR,
    EMPRESS,

}