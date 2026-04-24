using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    [SerializeField] Material nightSkybox;
    [SerializeField] Material sunSkybox;


    void Start()
    {
        GameManager.Ins.RightBeforeGameProperStart += SetNight;
        RenderSettings.skybox = sunSkybox;
        DynamicGI.UpdateEnvironment();
    }

    void OnDisable()
    {
        GameManager.Ins.RightBeforeGameProperStart -= SetNight;
    }

    void SetNight()
    {
        RenderSettings.skybox = nightSkybox;
        DynamicGI.UpdateEnvironment();
    }
}
