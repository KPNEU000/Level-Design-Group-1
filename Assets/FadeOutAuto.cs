using System.Collections;
using UnityEngine;

public class FadeOutAuto : MonoBehaviour
{
    MeshRenderer meshRenderer;
    public Color startColor;
    public Color currentColor;
    public float fadeAfterSeconds;
    public float fadeRate = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        startColor = meshRenderer.materials[0].color;
        currentColor = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        Invoke("StartFadeOut", fadeAfterSeconds);
    }

    void StartFadeOut()
    {
        StartCoroutine(FadeOutObject());
    }

    IEnumerator FadeOutObject()
    {
        print("started faeout");
        while (currentColor.a > 0)
        {
		// Reduce the color's alpha value
		currentColor.a -= fadeRate;

		// Apply the modified color to the object's mesh renderer
		meshRenderer.materials[0].color = currentColor;

		// Wait for the frame to update
		yield return new WaitForEndOfFrame();
	}

        // If the material's color's alpha value is less than or equal to 0, end the coroutine
        yield return new WaitUntil(() => meshRenderer.materials[0].color.a <= 0f);

    if(currentColor.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
