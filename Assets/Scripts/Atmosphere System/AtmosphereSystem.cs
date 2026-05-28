using System;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// This script reads from AtmospherePresets
/// And applies the values to the scene.
/// </summary>

public class AtmosphereSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Light sunLight;
    [SerializeField] private Material skyboxMaterial;

    [Header("Optional: World‑Space Clouds")]
    [SerializeField] private bool useWorldSpaceClouds;
    [SerializeField] private Material cloudMaterial;
    [SerializeField] private MeshRenderer cloudsRenderer;

    [Header("Optional: Post‑Processing")]
    [SerializeField] private bool useVolumeBlending;
    [SerializeField] private Volume volumeA;
    [SerializeField] private Volume volumeB;

    [Header("Presets & Debug")]
    [SerializeField] private AtmospherePreset currentPreset;
    [SerializeField] private AtmospherePreset[] presets;  // for quick testing

    // Runtime materials & state
    private Material _runtimeSkybox;
    private Material _runtimeClouds;
    private Volume _activeVolume;
    private Volume _targetVolume;

    private AtmospherePreset _fromPreset;
    private AtmospherePreset _toPreset;
    private float _blendTime;
    private float _blendDuration;
    private bool _isBlending;

    #region Unity Lifecycle

    private void Awake()
    {
        // Clone skybox material to avoid modifying the asset
        _runtimeSkybox = new Material(skyboxMaterial);
        RenderSettings.skybox = _runtimeSkybox;

        if (useWorldSpaceClouds && cloudMaterial != null && cloudsRenderer != null)
        {
            _runtimeClouds = new Material(cloudMaterial);
            cloudsRenderer.material = _runtimeClouds;
        }

        if (useVolumeBlending && volumeA != null && volumeB != null)
        {
            _activeVolume = volumeA;
            _targetVolume = volumeB;
            _activeVolume.weight = 0f;
            _targetVolume.weight = 0f;
            _activeVolume.profile = null;
            _targetVolume.profile = null;
        }
    }

    private void Update()
    {
        // Optional debug keys – remove if not needed
        if (presets is { Length: > 0 })
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) TransitionTo(presets[0], 2f);
            if (Input.GetKeyDown(KeyCode.Alpha2)) TransitionTo(presets[1], 2f);
            if (Input.GetKeyDown(KeyCode.Alpha3)) TransitionTo(presets[2], 2f);
            if (Input.GetKeyDown(KeyCode.Alpha0)) TransitionTo(presets[3], 2f);
        }

        if (!_isBlending) return;

        _blendTime += Time.deltaTime;
        float rawT = Mathf.Clamp01(_blendTime / _blendDuration);
        float t = rawT * rawT * (3f - 2f * rawT); // SmoothStep

        ApplyBlended(_fromPreset, _toPreset, t);

        if (t >= 1f)
        {
            currentPreset = _toPreset;
            _isBlending = false;
        }
    }

    #endregion

    #region Public API

    /// <summary>Instantly apply a preset.</summary>
    public void ApplyPreset(AtmospherePreset preset)
    {
        if (!preset) return;
        
        ApplySky(preset);
        ApplySun(preset);
        ApplyClouds(preset);
        ApplyFog(preset);
        
        if (useWorldSpaceClouds) ApplyWorldClouds(preset);
        if (useVolumeBlending) ApplyVolume(preset, instant: true);
        currentPreset = preset;
    }

    /// <summary>Smoothly transition to a preset over 'duration' seconds.</summary>
    public void TransitionTo(AtmospherePreset nextPreset, float duration)
    {
        if (!currentPreset)
        {
            ApplyPreset(nextPreset);
            Debug.Log($"Atmosphere: initial preset -> {nextPreset.name}");
            return;
        }

        if (duration <= 0f)
        {
            ApplyPreset(nextPreset);
            return;
        }

        _fromPreset = currentPreset;
        _toPreset = nextPreset;

        // Setup volumes for blending
        if (useVolumeBlending && _activeVolume != null && _targetVolume != null)
        {
            _activeVolume.profile = currentPreset.volumeProfile;
            _targetVolume.profile = nextPreset.volumeProfile;
            _activeVolume.weight = 1f;
            _targetVolume.weight = 0f;
        }

        _blendTime = 0f;
        _blendDuration = duration;
        _isBlending = true;

        Debug.Log($"Atmosphere transition: {currentPreset.name} -> {nextPreset.name} ({duration}s)");
    }

    /// <summary>Cycle to next preset in the inspector array.</summary>
    public void NextPreset(float duration)
    {
        if (presets == null || presets.Length == 0) return;
        int idx = Array.IndexOf(presets, currentPreset);
        if (idx < 0) idx = 0;
        idx = (idx + 1) % presets.Length;
        TransitionTo(presets[idx], duration);
    }

    /// <summary>Cycle to previous preset.</summary>
    public void PrevPreset(float duration)
    {
        if (presets == null || presets.Length == 0) return;
        int idx = Array.IndexOf(presets, currentPreset);
        if (idx < 0) idx = 0;
        idx = (idx - 1 + presets.Length) % presets.Length;
        TransitionTo(presets[idx], duration);
    }

    #endregion

    #region Single‑Preset Applications

    private void ApplySky(AtmospherePreset p)
    {
        _runtimeSkybox.SetColor("_SkyColor", p.zenithColor);
        _runtimeSkybox.SetColor("_HorizonColor", p.horizonColor);
        _runtimeSkybox.SetColor("_GroundColor", p.nadirColor);
    }

    private void ApplySun(AtmospherePreset p)
    {
        if (sunLight != null) sunLight.intensity = p.sunIntensity;
        _runtimeSkybox.SetFloat("_SunSize", p.sunSize);
        _runtimeSkybox.SetFloat("_SunIntensity", p.sunIntensity);
    }

    private void ApplyClouds(AtmospherePreset p)
    {
        _runtimeSkybox.SetFloat("_CloudScatter", p.cloudScatter);
        _runtimeSkybox.SetFloat("_CloudDensity", p.cloudDensity);
        _runtimeSkybox.SetVector("_CloudSpeed", p.panSpeed);
    }

    private void ApplyFog(AtmospherePreset p)
    {
        RenderSettings.fogColor = p.fogColor;
        RenderSettings.fogDensity = p.fogDensity;
    }

    private void ApplyWorldClouds(AtmospherePreset p)
    {
        if (_runtimeClouds == null) return;
        _runtimeClouds.SetFloat("_CloudsPower", p.cloudPower);
        _runtimeClouds.SetVector("_CloudSpeed", p.cloudSpeed);
    }

    private void ApplyVolume(AtmospherePreset p, bool instant)
    {
        if (!useVolumeBlending || _activeVolume == null) return;
        if (instant)
        {
            _activeVolume.profile = p.volumeProfile;
            _activeVolume.weight = 1f;
            if (_targetVolume != null) _targetVolume.weight = 0f;
        }
        else
        {
            // Will be handled by the blend loop
        }
    }

    #endregion

    #region Blending

    private void ApplyBlended(AtmospherePreset a, AtmospherePreset b, float t)
    {
        // Sky
        _runtimeSkybox.SetColor("_SkyColor", Color.Lerp(a.zenithColor, b.zenithColor, t));
        _runtimeSkybox.SetColor("_HorizonColor", Color.Lerp(a.horizonColor, b.horizonColor, t));
        _runtimeSkybox.SetColor("_GroundColor", Color.Lerp(a.nadirColor, b.nadirColor, t));

        // Sun
        float sunSize = Mathf.Lerp(a.sunSize, b.sunSize, t);
        float sunIntensity = Mathf.Lerp(a.sunIntensity, b.sunIntensity, t);
        _runtimeSkybox.SetFloat("_SunSize", sunSize);
        _runtimeSkybox.SetFloat("_SunIntensity", sunIntensity);
        if (sunLight != null) sunLight.intensity = sunIntensity;

        // Skybox clouds
        _runtimeSkybox.SetFloat("_CloudDensity", Mathf.Lerp(a.cloudDensity, b.cloudDensity, t));
        _runtimeSkybox.SetFloat("_CloudScatter", Mathf.Lerp(a.cloudScatter, b.cloudScatter, t));
        _runtimeSkybox.SetVector("_CloudSpeed", Vector2.Lerp(a.panSpeed, b.panSpeed, t));

        // World clouds (if enabled)
        if (useWorldSpaceClouds && _runtimeClouds != null)
        {
            _runtimeClouds.SetFloat("_CloudsPower", Mathf.Lerp(a.cloudPower, b.cloudPower, t));
            _runtimeClouds.SetVector("_CloudSpeed", Vector2.Lerp(a.cloudSpeed, b.cloudSpeed, t));
        }

        // Fog
        RenderSettings.fogColor = Color.Lerp(a.fogColor, b.fogColor, t);
        RenderSettings.fogDensity = Mathf.Lerp(a.fogDensity, b.fogDensity, t);

        // Post‑processing dual‑volume blend
        if (useVolumeBlending && _activeVolume != null && _targetVolume != null)
        {
            _activeVolume.weight = 1f - t;
            _targetVolume.weight = t;

            if (t >= 1f)
            {
                // Finalize blend: swap volumes and clean up
                _activeVolume.weight = 0f;
                _targetVolume.weight = 1f;

                // Ping‑pong
                (_activeVolume, _targetVolume) = (_targetVolume, _activeVolume);
                _targetVolume.profile = null;
                _targetVolume.weight = 0f;
            }
        }
    }

    #endregion
}