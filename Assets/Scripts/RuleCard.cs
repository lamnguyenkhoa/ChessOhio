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
    // For selecting card, slightly move card up
    private const float HOVER_OFFSET_Y = 20f;
    ///If other player is choosing card, you can't see the details
    public bool showDetails = true;
    // Delay a bit of time to make sure no accident clicking
    private bool delayedInteraction = false;
    // Card already selected. Now only for viewing. Hover over finished card will update
    // the isDisplay card.
    public bool finished = false;
    // This card is for display profile only. No interaction.
    public bool isDisplay = false;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    private void OnEnable()
    {
        if (!audioSource)
        {
            audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
        }
        if (isDisplay)
        {
            DisplayEmptyCard();
            return;
        }
        StartCoroutine(SetOriginalPos());
    }

    private void Update()
    {
        if (isDisplay)
        {
            return;
        }
        if (showDetails && delayedInteraction)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, Time.deltaTime * 10);
            if (Input.GetMouseButtonDown(0) && mouseHovering && !finished)
            {
                finished = true;
                GameRule.instance.ChoseThisRule(profile, true);
                if (audioSource)
                    audioSource.PlayOneShot(clickSound);
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
        if (profile.sprite)
            image.sprite = profile.sprite;
        image.gameObject.SetActive(true);
        ruleName.text = profile.ruleName;
        description.text = profile.description;
    }

    public void DisplayHiddenCardInfo()
    {
        image.sprite = null;
        image.gameObject.SetActive(false);
        ruleName.text = "Wait for other player...";
        description.text = "";
    }

    public void DisplayEmptyCard()
    {
        image.gameObject.SetActive(false);
        ruleName.text = "";
        description.text = "";
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (audioSource)
            audioSource.PlayOneShot(hoverSound);
        if (isDisplay)
            return;
        desiredPos = originalPos + new Vector3(0, HOVER_OFFSET_Y, 0);
        mouseHovering = true;
        if (finished)
        {
            GameManager.instance.SetDisplayCardProfile(profile);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isDisplay)
            return;
        desiredPos = originalPos;
        mouseHovering = false;
        if (finished)
        {
            GameManager.instance.HideDisplayCardProfile();
        }
    }
}
