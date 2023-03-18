using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PromoButton : MonoBehaviour, IPointerEnterHandler
{
    private PromotionScreen promotionScreen;
    private ChessPieceProfileSO profileToDisplay;

    public void Setup(PromotionScreen promotionScreen, ChessPieceProfileSO profileToDisplay)
    {
        this.promotionScreen = promotionScreen;
        this.profileToDisplay = profileToDisplay;
        transform.name = $"{profileToDisplay.pieceName}Button";
        transform.GetComponent<Button>().onClick.AddListener(delegate { SpecialMoveHandler.instance.SetChosenPromote((int)profileToDisplay.type); });
        transform.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = profileToDisplay.pieceName;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        promotionScreen.descriptionArea.GetComponent<TextMeshProUGUI>().text = profileToDisplay.description;
        promotionScreen.image.sprite = profileToDisplay.sprite;
        promotionScreen.image.gameObject.SetActive(true);
    }
}
