using System.Collections;
using UnityEngine;

public class SceneMainLight : MonoBehaviour
{
    public static SceneMainLight instance;
    private Color originalColor = new Color(1, 0.9568627f, 0.8392157f, 1f);
    private float originalIntensity = 0.2f;

    private Light lightComponent;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Debug.Log("Too many SceneMainLight");
            Destroy(this);
        }
    }

    void Start()
    {
        lightComponent = GetComponent<Light>();
        originalColor = lightComponent.color;
    }

    /// <summary>
    /// Update scene light color and stuff based on rules in effect.
    /// </summary>
    public void UpdateSceneLight()
    {
        if (GameRule.instance.activatedRule.Find(x => x.constraintRuleCode == UniqueRuleCode.CONSTRAINT_FORCE_CAPTURE))
        {
            StartCoroutine(ColorShiftCoroutine(Color.red, 0.4f, 2f));
        }
        else
        {
            StartCoroutine(ColorShiftCoroutine(originalColor, originalIntensity, 2f));
        }
    }

    IEnumerator ColorShiftCoroutine(Color endColor, float endIntensity, float duration)
    {
        Color startColor = lightComponent.color;
        float startIntensity = lightComponent.intensity;
        float timeElapsed = 0.0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float t = Mathf.Clamp01(timeElapsed / duration);
            Color newColor = Color.Lerp(startColor, endColor, t);
            float newIntensity = Mathf.Lerp(startIntensity, endIntensity, t);
            lightComponent.color = newColor;
            lightComponent.intensity = newIntensity;
            yield return null;
        }
    }
}