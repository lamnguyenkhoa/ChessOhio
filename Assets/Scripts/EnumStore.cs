public enum ClientID
{
    HOST = 0,
    CLIENT = 1
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

/// <summary>
/// Action in ActionMenu, different from SpecialMove. Think of it like
/// active vs passive skill.
/// </summary>
public enum SpecialAction
{
    NONE = 0,
    INVERT = 1,
    COMBINE = 2,
    SPECIAL_PROMO = 3,
}

/// <summary>
/// SpecialMove, which is different from SpecialAction. Usually resolved
/// at the end of turn. They are the basic stuff: en passant, castling, promotion.
/// </summary>
public enum SpecialMove
{
    NONE = 0,
    EN_PASSANT = 1,
    CASTLING = 2,
    PROMOTION = 3
}

public enum PieceTeam
{
    WHITE = 0,
    BLACK = 1
}

public enum PieceType
{
    NONE = 0,
    PAWN = 1,
    ROOK = 2,
    KNIGHT = 3,
    BISHOP = 4,
    QUEEN = 5,
    KING = 6,
    NIGHTRIDER = 7,
    DESERTER = 8,
    BUNKER = 9,
    ARCHBISHOP = 10,
    CAVALIER = 11,
    PALADIN = 12,

}