using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public AudioSource audioSource;
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        if (!audioSource)
            audioSource = GameObject.Find("SFX").GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
            audioSource.PlayOneShot(clickSound);
    }


}
