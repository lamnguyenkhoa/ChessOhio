using System.Collections;
using UnityEngine;

public class RuleDecoration : MonoBehaviour
{
    public static RuleDecoration instance;

    [Header("Pawn Hub")]
    public GameObject pawnHubParent;
    public float phSpeed = 2f;
    public float phDistanceToMove = 3f;
    private float phCurrentDistanceMoved = 0f;

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

    public void ShowPawnHub()
    {
        pawnHubParent.SetActive(true);
        StartCoroutine(MoveObject());
    }

    IEnumerator MoveObject()
    {
        while (phCurrentDistanceMoved < phDistanceToMove)
        {
            // Move the object up by the specified speed
            transform.Translate(Vector3.up * phSpeed * Time.deltaTime);

            // Update the amount of distance the object has moved so far
            phCurrentDistanceMoved += phSpeed * Time.deltaTime;

            yield return null;
        }
    }
}
