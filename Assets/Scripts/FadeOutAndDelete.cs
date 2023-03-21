using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadeOutAndDelete : MonoBehaviour
{
    public float delay = 1.0f; // duration of delay before start to fade out
    public float duration = 1.0f; // duration of the fade out effect in seconds
    private Image imageToFade; // reference to the image game object to fade out and delete

    // Call this function to start the fade out effect and delete the image game object
    void Start()
    {
        imageToFade = GetComponent<Image>();
        // Start the coroutine that fades out the image after 1 second
        StartCoroutine(FadeOutAfterDelay(1f));
    }

    IEnumerator FadeOutAfterDelay(float delay)
    {
        // Wait for the specified delay before starting the fade out
        yield return new WaitForSeconds(delay);

        // Gradually reduce the image's alpha value over 1 second
        float fadeDuration = 1f;
        float timer = 0f;
        Color startColor = imageToFade.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            imageToFade.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
            yield return null;
        }

        // Disable the image component once the fade out is complete
        imageToFade.enabled = false;
    }
}