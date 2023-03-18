using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PromotionScreen : MonoBehaviour
{
    public GameObject buttonPrefab;
    public GameObject buttonArea;
    public GameObject descriptionArea;
    public Image image;
    // Piece that about to be promoted
    public ChessPieceProfileSO pieceProfile;

    private void OnEnable()
    {
        if (pieceProfile == null)
            return;
        image.gameObject.SetActive(false);
        descriptionArea.GetComponent<TextMeshProUGUI>().text = "";

        foreach (ChessPieceProfileSO promoProfile in pieceProfile.possiblePromoProfiles)
        {
            PromoButton promoButton = GameObject.Instantiate(buttonPrefab, buttonArea.transform).GetComponent<PromoButton>();
            promoButton.Setup(this, promoProfile);
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
