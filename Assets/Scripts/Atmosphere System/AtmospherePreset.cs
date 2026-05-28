using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This script reads from AtmospherePresets
/// And applies the values to the scene.
/// </summary>

[CreateAssetMenu(fileName = "AtmospherePreset", menuName = "Atmosphere/New Preset")]
public class AtmospherePreset : ScriptableObject
{
    [Header("Sun")]
    public float sunIntensity = 1f;
    public float sunSize = 0.1f;

    [Header("Sky")]
    public Color zenithColor = Color.blue;
    public Color horizonColor = Color.cyan;
    public Color nadirColor = Color.gray;

    [Header("Skybox Clouds")]
    [Range(0, 1)] public float cloudScatter = 0.5f;
    [Range(0, 1)] public float cloudDensity = 0.3f;
    public Vector2 panSpeed = Vector2.zero;

    [Header("Fog")]
    public Color fogColor = Color.gray;
    public float fogDensity = 0.02f;

    // ---- Optional features (used only if enabled in AtmosphereSystem) ----
    [Header("Optional: World‑Space Clouds")]
    public float cloudPower = 1f;
    public Vector2 cloudSpeed = Vector2.zero;

    [Header("Optional: Post‑Processing")]
    public VolumeProfile volumeProfile;
}
