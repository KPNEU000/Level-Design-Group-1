using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class Utils
{
    public static void StartFade(MonoBehaviour caller, GameObject obj, float startAlpha, float endAlpha, float duration)
    {
        caller.StartCoroutine(Fade(obj, startAlpha, endAlpha, duration));
    }

    public static IEnumerator Fade(GameObject obj, float startAlpha, float endAlpha, float duration)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            Debug.LogWarning("Fade failed: no renderers found.");
            yield break;
        }

        // Collect materials (instance materials, safe for runtime editing)
        List<Material> mats = new List<Material>();
        foreach (Renderer r in renderers)
            mats.AddRange(r.materials);

        Material[] materials = mats.ToArray();

        // Ensure transparency if fading out
        if (endAlpha < 1f)
            SetTransparent(materials);

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            SetAlpha(materials, alpha);

            yield return null;
        }

        // Final snap
        SetAlpha(materials, endAlpha);

        // Restore opaque if needed
        if (endAlpha >= 1f)
            SetOpaque(materials);
    }

    // ------------------------
    // URP-SAFE ALPHA HANDLING
    // ------------------------
    private static void SetAlpha(Material[] materials, float alpha)
    {
        foreach (Material mat in materials)
        {
            Debug.Log(mat.name + " | " + mat.shader.name);

            if (!mat.HasProperty("_BaseColor"))
            {
                Debug.LogWarning("No _BaseColor on " + mat.name);
                continue;
            }

            Color c = mat.GetColor("_BaseColor");
            c.a = alpha;
            mat.SetColor("_BaseColor", c);
        }
    }

    // ------------------------
    // URP TRANSPARENT MODE
    // ------------------------
    private static void SetTransparent(Material[] materials)
    {
        foreach (Material mat in materials)
        {
            // Surface type: Transparent
            mat.SetFloat("_Surface", 1);

            // Blend mode: Alpha
            mat.SetFloat("_Blend", 0);

            // Disable depth writing (IMPORTANT)
            mat.SetFloat("_ZWrite", 0);

            // Render settings
            mat.renderQueue = (int)RenderQueue.Transparent;

            // Keywords
            mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            mat.DisableKeyword("_ALPHATEST_ON");
            mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        }
    }

    // ------------------------
    // URP OPAQUE MODE
    // ------------------------
    private static void SetOpaque(Material[] materials)
    {
        foreach (Material mat in materials)
        {
            mat.SetFloat("_Surface", 0);
            mat.SetFloat("_ZWrite", 1);

            mat.renderQueue = (int)RenderQueue.Geometry;

            mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        }
    }
}