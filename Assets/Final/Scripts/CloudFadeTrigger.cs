using UnityEngine;
using System.Collections;

public class CloudFadeTrigger : MonoBehaviour
{
    [SerializeField] ParticleSystem[] clouds;
    [SerializeField] float fadeDuration = 2f;

    void Start()
    {
        GameManager.Ins.RightBeforeGameProperStart += StartFadeClouds;
    }

    void OnDisable()
    {
        GameManager.Ins.RightBeforeGameProperStart = StartFadeClouds;
    }
    void StartFadeClouds()
    {
        StartCoroutine(FadeClouds());
    }

    IEnumerator FadeClouds()
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            foreach (ParticleSystem ps in clouds)
            {
                var colorOverLifetime = ps.colorOverLifetime;
                colorOverLifetime.enabled = true;
                colorOverLifetime.color = new ParticleSystem.MinMaxGradient(
    new Color(1f, 1f, 1f, alpha));
            }

            yield return null;
        }

        foreach (ParticleSystem ps in clouds)
        {
            ps.Clear();
            ps.Stop();
        }
    }
}