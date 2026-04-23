using System.Collections;
using UnityEngine;
public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Ins { get; private set; }

    CanvasGroup cg;

    void Awake()
    {
        Ins = this;
        cg = GetComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;
    }

    public IEnumerator FadeTo(float target, float duration)
    {
        float start = cg.alpha;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            cg.alpha = Mathf.Lerp(start, target, timer / duration);
            yield return null;
        }

        cg.alpha = target;
    }
}