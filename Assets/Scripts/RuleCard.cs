using System.Collections;
using System.Collections.Generic;
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

    private const float HOVER_OFFSET_Y = 20f;

    private void Start()
    {
        originalPos = transform.localPosition;
        desiredPos = originalPos;
    }

    private void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, Time.deltaTime * 10);
    }

    private void OnEnable()
    {
        if (profile)
            RefreshCardInfo();
    }

    public void RefreshCardInfo()
    {
        if (profile.image)
            image = profile.image;
        if (profile.type == RuleType.INVERT_RULE ||
            profile.type == RuleType.COMBINE_RULE ||
            profile.type == RuleType.SPECIAL_PROMO_RULE)
        {
            ruleName.text = "New unit: ";
        }
        else
        {
            ruleName.text = "";
        }

        ruleName.text += profile.ruleName;
        description.text = profile.description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        desiredPos = originalPos + new Vector3(0, HOVER_OFFSET_Y, 0);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        desiredPos = originalPos;
    }


}
