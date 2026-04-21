using System.Collections;
using UnityEngine;

public static class Utils
{

    public static void StartFade(MonoBehaviour caller, GameObject obj, float startAlpha, float endAlpha, float duration)
    {
        caller.StartCoroutine(Fade(obj, startAlpha, endAlpha, duration));
    }

    public static IEnumerator Fade(GameObject obj, float startAlpha, float endAlpha, float duration)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer == null) yield break;

        Material[] materials = renderer.materials;

        if (endAlpha < 1f)
            SetTransparent(materials);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            SetAlpha(materials, alpha);
            yield return null;
        }

        SetAlpha(materials, endAlpha);

        if (endAlpha >= 1f)
            SetOpaque(materials);
    }

    private static void SetAlpha(Material[] materials, float alpha)
    {
        foreach (Material mat in materials)
        {
            Color c = mat.color;
            c.a = alpha;
            mat.color = c;
        }
    }

    private static void SetTransparent(Material[] materials)
    {
        foreach (Material mat in materials)
        {
            mat.SetFloat("_Surface", 1);
            mat.SetOverrideTag("RenderType", "Transparent");
            mat.renderQueue = 3000;
        }
    }

    private static void SetOpaque(Material[] materials)
    {
        foreach (Material mat in materials)
        {
            mat.SetFloat("_Surface", 0);
            mat.SetOverrideTag("RenderType", "Opaque");
            mat.renderQueue = -1;
        }
    }
}