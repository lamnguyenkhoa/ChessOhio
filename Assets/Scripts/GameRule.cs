using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameRule : MonoBehaviour
{
    public static GameRule instance;
    public Dictionary<PieceType, PieceType> invertDict = new Dictionary<PieceType, PieceType>();
    public Dictionary<PieceType, PieceType> combineDict = new Dictionary<PieceType, PieceType>();
    public RuleCardSO[] availableRule; // Every rulecardSO
    public List<RuleCardSO> activeRulePool = new List<RuleCardSO>(); // Some default starting rulecardSO

    public const int N_DRAW = 3;
    public GameObject ruleCardUI;


    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        invertDict.Add(PieceType.KNIGHT, PieceType.NIGHTRIDER);
    }

    public void DrawRuleCard()
    {
        RuleCardSO[] drawedCards = { };
        if (activeRulePool.Count >= N_DRAW)
        {
            drawedCards = MyHelper.GetRandomItems(activeRulePool, 3);
        }
        else
        {
            Debug.Log("Not enough card to draw");
        }

        for (int i = 0; i < N_DRAW; i++)
        {
            RuleCard ruleCard = ruleCardUI.transform.GetChild(i).GetComponent<RuleCard>();
            ruleCard.profile = drawedCards[i];
        }
        ruleCardUI.SetActive(true);
    }
}
