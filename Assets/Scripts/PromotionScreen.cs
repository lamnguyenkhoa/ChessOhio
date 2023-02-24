using TMPro;
using UnityEngine;

public class PromotionScreen : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject buttonArea;
    public GameObject descriptionArea;
    public ChessPieceProfileSO pieceProfile;

    private void OnEnable()
    {
        if (pieceProfile == null)
            return;

        descriptionArea.GetComponent<TextMeshProUGUI>().text = pieceProfile.description;
        foreach (ChessPieceProfileSO promoProfile in pieceProfile.possiblePromoProfiles)
        {
            PromoButton promoButton = GameObject.Instantiate(buttonPrefab, buttonArea.transform).GetComponent<PromoButton>();
            promoButton.Setup(this, promoProfile);
            promoButton.RefreshButtonData();
        }
    }

    private void OnDisable()
    {
        // Delete all button
        foreach (Transform child in buttonArea.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
