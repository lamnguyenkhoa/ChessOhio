using System.Collections.Generic;
using UnityEngine;

public class GameRule : MonoBehaviour
{
    public static GameRule instance;
    public Dictionary<PieceType, PieceType> invertDict = new Dictionary<PieceType, PieceType>();
    public Dictionary<PieceType, PieceType> combineDict = new Dictionary<PieceType, PieceType>();

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
}
