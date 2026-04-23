using UnityEngine;
using System.Collections;

public class LightController : MonoBehaviour
{
    public Light dirLight;
    [SerializeField] Color etherealColor = Color.white;
    [SerializeField] Color nightColor = Color.darkGray;
    [SerializeField] Color sunnyColor = Color.yellowNice;
    [SerializeField] float lerpSpeed = 3f;

    void Awake()
    {
        GameManager.Ins.OnGameProperStart += SetNight;
        dirLight.intensity = 2f;
        dirLight.color = etherealColor;
    }

    void OnDisable()
    {
        GameManager.Ins.OnGameProperStart -= SetNight;
    }

    void Update()
    {

    }

    void SetNight()
    {
        StartCoroutine(LerpColor(etherealColor, nightColor, lerpSpeed));
    }

    void SetDay()
    {
        StartCoroutine(LerpColor(etherealColor, nightColor, lerpSpeed));
    }

    IEnumerator LerpColor(Color startColor, Color endColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            dirLight.color = Color.Lerp(startColor, endColor, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dirLight.color = endColor;
    }
}
