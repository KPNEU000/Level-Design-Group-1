using System.Collections;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    MeshRenderer meshRenderer;
    public Color startColor;
    public Color currentColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        startColor = meshRenderer.sharedMaterials[0].color;
        currentColor = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(FadeOutObject());
    }

    IEnumerator FadeOutObject()
    {
        while (currentColor.a > 0)
        {
        print("yes");
		// Reduce the color's alpha value
		currentColor.a -= 0.1f;

		// Apply the modified color to the object's mesh renderer
		meshRenderer.sharedMaterials[0].color = currentColor;

		// Wait for the frame to update
		yield return new WaitForEndOfFrame();
	}

        // If the material's color's alpha value is less than or equal to 0, end the coroutine
        yield return new WaitUntil(() => meshRenderer.materials[0].color.a <= 0f);
    }
}
