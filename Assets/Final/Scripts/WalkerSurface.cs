using UnityEngine;

public class WalkerSurface : MonoBehaviour
{
    public enum SurfaceType { Grass, Mud }

    [Header("Terrain")]
    [SerializeField] Terrain terrain;
    [SerializeField] SurfaceType[] layerMap;

    [Header("Debug")]
    [SerializeField] bool debugMode;

    [Header("Trail")]
    [SerializeField] Material grassTrail;
    [SerializeField] Material mudTrail;

    [Header("Particles")]
    [SerializeField] ParticleSystem grassParticles;
    [SerializeField] ParticleSystem mudParticles;

    TrailRenderer trail;
    SurfaceType currentSurface;
    Vector3 lastPosition;

    void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        if (terrain == null) terrain = Terrain.activeTerrain;
    }

    void Start()
    {
        lastPosition = transform.position;
        currentSurface = DetectSurface();
        ApplyTrailMaterial(currentSurface);
        Debug.Log($"[WalkerSurface] Initialized. Terrain: {(terrain != null ? terrain.name : "NULL")} | LayerMap size: {layerMap.Length}");
        Debug.Log($"[WalkerSurface] GrassParticles: {(grassParticles != null ? "assigned" : "NULL")} | MudParticles: {(mudParticles != null ? "assigned" : "NULL")}");
        Debug.Log($"[WalkerSurface] Initial surface: {currentSurface}");
        UpdateParticles(currentSurface, false);

        if (terrain != null)
        {
            TerrainLayer[] layers = terrain.terrainData.terrainLayers;
            for (int i = 0; i < layers.Length; i++)
            {
                string mapped = i < layerMap.Length ? layerMap[i].ToString() : "UNMAPPED (defaults to Mud)";
                Debug.Log($"[WalkerSurface] Layer {i}: \"{layers[i].name}\" → {mapped}");
            }
        }
    }

    void Update()
    {
        Vector3 delta = transform.position - lastPosition;
        bool moving = new Vector2(delta.x, delta.z).sqrMagnitude > 0.001f;
        lastPosition = transform.position;

        SurfaceType surface = DetectSurface();

        if (debugMode)
            Debug.Log($"[WalkerSurface] Detected: {surface} | Moving: {moving}");

        if (surface != currentSurface)
        {
            currentSurface = surface;
            ApplyTrailMaterial(surface);
            Debug.Log($"[WalkerSurface] Surface changed: {surface}");
        }

        UpdateParticles(surface, moving);
    }

    SurfaceType DetectSurface()
    {
        if (terrain == null) return SurfaceType.Mud;

        TerrainData td = terrain.terrainData;
        Vector3 local = transform.position - terrain.transform.position;

        int x = Mathf.Clamp(Mathf.RoundToInt((local.x / td.size.x) * td.alphamapWidth),  0, td.alphamapWidth  - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt((local.z / td.size.z) * td.alphamapHeight), 0, td.alphamapHeight - 1);

        float[,,] alphas = td.GetAlphamaps(x, z, 1, 1);

        int dominant = 0;
        for (int i = 1; i < td.alphamapLayers; i++)
            if (alphas[0, 0, i] > alphas[0, 0, dominant]) dominant = i;

        string layerName = dominant < td.terrainLayers.Length ? td.terrainLayers[dominant].name : "unknown";
        SurfaceType result = dominant < layerMap.Length ? layerMap[dominant] : SurfaceType.Mud;
        if (debugMode)
            Debug.Log($"[WalkerSurface] Dominant layer index: {dominant} | Name: \"{layerName}\" | Weight: {alphas[0, 0, dominant]:F3} | Mapped to: {result}");

        return result;
    }

    void ApplyTrailMaterial(SurfaceType surface)
    {
        if (trail == null) return;
        trail.material = surface == SurfaceType.Grass ? grassTrail : mudTrail;
    }

    void UpdateParticles(SurfaceType surface, bool moving)
    {
        if (!moving)
        {
            StopParticles(grassParticles, clear: true);
            StopParticles(mudParticles,   clear: true);
            return;
        }

        // Moving — soft stop on the outgoing surface so particles fade out naturally
        SetParticles(grassParticles, surface == SurfaceType.Grass);
        SetParticles(mudParticles,   surface == SurfaceType.Mud);
    }

    void SetParticles(ParticleSystem ps, bool shouldPlay)
    {
        if (ps == null) return;
        if (shouldPlay && !ps.isPlaying)
            ps.Play();
        else if (!shouldPlay)
            StopParticles(ps, clear: false);
    }

    void StopParticles(ParticleSystem ps, bool clear)
    {
        if (ps == null || !ps.isPlaying) return;
        ps.Stop(false, clear ? ParticleSystemStopBehavior.StopEmittingAndClear
                              : ParticleSystemStopBehavior.StopEmitting);
    }
}
