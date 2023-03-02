using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RuleCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RuleCardSO profile;
    public Image image;
    public TextMeshProUGUI ruleName;
    public TextMeshProUGUI description;
    private Vector3 originalPos;
    private Vector3 desiredPos;
    private bool mouseHovering = false;
    private const float HOVER_OFFSET_Y = 20f;
    private bool allowInteraction = true;
    private bool delayedInteraction = false; // Delay a bit of time to make sure no accident clicking

    private void Start()
    {
        StartCoroutine(SetOriginalPos());
        allowInteraction = GameSetting.instance.isLocalGame ||
            GameManager.instance.GetCurrentPlayer().team == GameRule.instance.teamToChoseRule;
        if (profile)
        {
            if (allowInteraction)
            {
                RefreshCardInfo();
            }
            else
            {
                DisplayHiddenCardInfo();
            }
        }
    }

    private void Update()
    {
        if (allowInteraction && delayedInteraction)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, Time.deltaTime * 10);
            if (Input.GetMouseButtonDown(0) && mouseHovering)
            {
                GameRule.instance.ChoseThisRule(profile, true);
            }
        }
    }

    public IEnumerator SetOriginalPos()
    {
        // We need to wait a bit for 2 reasons: 
        // - Grid Layout update the rule card's position
        // - Prevent accident clicking the card
        delayedInteraction = false;
        yield return new WaitForSeconds(0.1f);
        originalPos = transform.localPosition;
        desiredPos = originalPos;
        delayedInteraction = true;
    }

    public void RefreshCardInfo()
    {
        if (profile.image)
            image = profile.image;
        ruleName.text = profile.ruleName;
        description.text = profile.description;
    }

    public void DisplayHiddenCardInfo()
    {
        image = null;
        ruleName.text = "Wait for other player...";
        description.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        desiredPos = originalPos + new Vector3(0, HOVER_OFFSET_Y, 0);
        mouseHovering = true;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        desiredPos = originalPos;
        mouseHovering = false;
    }


}
