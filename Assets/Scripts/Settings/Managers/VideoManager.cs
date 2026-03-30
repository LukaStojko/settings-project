using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VideoManager : BaseSettingsManager<VideoManager>
{
    public event Action OnVideoSettingsChanged;
    
    public readonly FullScreenMode[] AllowedFullscreenModes =
    {
        FullScreenMode.Windowed,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.ExclusiveFullScreen
    };

    private const string SECTION = "Video";

    // Keys
    private const string RES_WIDTH = "ResolutionWidth";
    private const string RES_HEIGHT = "ResolutionHeight";
    private const string FULLSCREEN_MODE = "FullscreenMode";
    private const string QUALITY = "Quality";
    private const string MSAA = "MSAA";
    private const string FPS_LIMIT = "FPSLimit";

    private const string SHADOWS = "Shadows";
    private const string BLOOM = "Bloom";
    private const string MOTION_BLUR = "MotionBlur";

    [Header("URP")]
    [SerializeField] private UniversalRenderPipelineAsset urpAssetLow;
    [SerializeField] private UniversalRenderPipelineAsset urpAssetMedium;
    [SerializeField] private UniversalRenderPipelineAsset urpAssetHigh;
    [SerializeField] private Volume globalVolume;
    
    private UniversalRenderPipelineAsset _currentUrpAsset;

    private Bloom _bloom;
    private MotionBlur _motionBlur;

    #region Initialization

    protected override void CreateDefaults()
    {
        SetValue(SECTION, RES_WIDTH, Screen.currentResolution.width.ToString());
        SetValue(SECTION, RES_HEIGHT, Screen.currentResolution.height.ToString());
        SetValue(SECTION, FULLSCREEN_MODE, FullScreenMode.FullScreenWindow.ToString());
        SetValue(SECTION, QUALITY, QualitySettings.GetQualityLevel().ToString());
        SetValue(SECTION, MSAA, "2");
        SetValue(SECTION, FPS_LIMIT, "-1"); // -1 = unlimited

        SetValue(SECTION, SHADOWS, true.ToString());
        SetValue(SECTION, BLOOM, true.ToString());
        SetValue(SECTION, MOTION_BLUR, false.ToString());
    }

    protected override void OnLoaded()
    {
        CacheVolumeComponents();
        ApplyAll();
    }

    protected override void OnReverted()
    {
        ApplyAll();
    }

    protected override string GetManagerSection()
    {
        return SECTION;
    }

    #endregion

    #region Public API

    public (int width, int height) GetResolution()
    {
        return (
            GetInt(SECTION, RES_WIDTH, Screen.currentResolution.width),
            GetInt(SECTION, RES_HEIGHT, Screen.currentResolution.height)
        );
    }

    public FullScreenMode GetFullscreenMode()
    {
        string fsModeStr = GetValue(SECTION, FULLSCREEN_MODE, FullScreenMode.FullScreenWindow.ToString());

        if (!Enum.TryParse(fsModeStr, out FullScreenMode fsMode))
            fsMode = FullScreenMode.FullScreenWindow;

        return fsMode;
    }

    public int GetQualityLevel()
    {
        return GetInt(SECTION, QUALITY, QualitySettings.GetQualityLevel());
    }

    public int GetMSAA()
    {
        return GetInt(SECTION, MSAA, 2);
    }

    public int GetFPSLimit()
    {
        return GetInt(SECTION, FPS_LIMIT, -1);
    }

    public bool GetShadows()
    {
        return GetBool(SECTION, SHADOWS, true);
    }

    public bool GetBloom()
    {
        return GetBool(SECTION, BLOOM, true);
    }

    public bool GetMotionBlur()
    {
        return GetBool(SECTION, MOTION_BLUR, false);
    }

    public void SetResolution(int width, int height)
    {
        SetValue(SECTION, RES_WIDTH, width.ToString());
        SetValue(SECTION, RES_HEIGHT, height.ToString());

        ApplyResolutionAndMode();
        OnVideoSettingsChanged?.Invoke();
    }

    public void SetFullscreenMode(FullScreenMode mode)
    {
        if (!AllowedFullscreenModes.Contains(mode))
            mode = FullScreenMode.FullScreenWindow;

        SetValue(SECTION, FULLSCREEN_MODE, mode.ToString());

        ApplyResolutionAndMode();
        OnVideoSettingsChanged?.Invoke();
    }

    public void SetQualityLevel(int level)
    {
        SetValue(SECTION, QUALITY, level.ToString());
        QualitySettings.SetQualityLevel(level);
        ApplyURPAssetForQuality(level);

        OnVideoSettingsChanged?.Invoke();
    }

    public void SetMSAA(int samples)
    {
        samples = Mathf.Clamp(samples, 0, 8);
        SetValue(SECTION, MSAA, samples.ToString());

        if (_currentUrpAsset != null)
            _currentUrpAsset.msaaSampleCount = samples;

        OnVideoSettingsChanged?.Invoke();
    }

    public void SetFPSLimit(int fps)
    {
        SetValue(SECTION, FPS_LIMIT, fps.ToString());
        Application.targetFrameRate = fps;
        OnVideoSettingsChanged?.Invoke();
    }

    public void SetShadows(bool inEnabled)
    {
        SetValue(SECTION, SHADOWS, inEnabled.ToString());

        QualitySettings.shadows = inEnabled ? UnityEngine.ShadowQuality.All : UnityEngine.ShadowQuality.Disable;

        OnVideoSettingsChanged?.Invoke();
    }

    public void SetBloom(bool inEnabled)
    {
        SetValue(SECTION, BLOOM, inEnabled.ToString());

        if (_bloom != null)
            _bloom.active = inEnabled;

        OnVideoSettingsChanged?.Invoke();
    }

    public void SetMotionBlur(bool inEnabled)
    {
        SetValue(SECTION, MOTION_BLUR, inEnabled.ToString());

        if (_motionBlur != null)
            _motionBlur.active = inEnabled;

        OnVideoSettingsChanged?.Invoke();
    }

    #endregion

    #region Apply Helpers

    private void ApplyAll()
    {
        ApplyResolutionAndMode();

        int quality = GetInt(SECTION, QUALITY, QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(quality);

        int msaa = GetInt(SECTION, MSAA, 2);
        if (_currentUrpAsset != null)
            _currentUrpAsset.msaaSampleCount = msaa;

        int fpsLimit = GetInt(SECTION, FPS_LIMIT, -1);
        Application.targetFrameRate = fpsLimit;

        bool shadows = GetBool(SECTION, SHADOWS, true);
        QualitySettings.shadows = shadows ? UnityEngine.ShadowQuality.All : UnityEngine.ShadowQuality.Disable;

        if (_bloom != null)
            _bloom.active = GetBool(SECTION, BLOOM, true);

        if (_motionBlur != null)
            _motionBlur.active = GetBool(SECTION, MOTION_BLUR, false);
            
        ApplyURPAssetForQuality(quality);
    }

    private void ApplyResolutionAndMode()
    {
        int width = GetInt(SECTION, RES_WIDTH, Screen.currentResolution.width);
        int height = GetInt(SECTION, RES_HEIGHT, Screen.currentResolution.height);

        string fsModeStr = GetValue(SECTION, FULLSCREEN_MODE, FullScreenMode.FullScreenWindow.ToString());
        if (!Enum.TryParse(fsModeStr, out FullScreenMode fsMode))
            fsMode = FullScreenMode.FullScreenWindow;

        Screen.SetResolution(width, height, fsMode);
    }

    #endregion

    #region Helpers

    private void CacheVolumeComponents()
    {
        if (globalVolume == null || globalVolume.profile == null)
            return;

        globalVolume.profile.TryGet(out _bloom);
        globalVolume.profile.TryGet(out _motionBlur);
    }

    public List<Resolution> GetAvailableResolutions()
    {
        return Screen.resolutions
            .Distinct()
            .OrderBy(r => r.width * r.height)
            .ToList();
    }

    public List<string> GetAvailableURPQualities()
    {
        var qualities = new List<string>();
        if (urpAssetLow != null) qualities.Add("Low");
        if (urpAssetMedium != null) qualities.Add("Medium");
        if (urpAssetHigh != null) qualities.Add("High");
        return qualities;
    }

    private void ApplyURPAssetForQuality(int qualityLevel)
    {
        UniversalRenderPipelineAsset targetAsset = null;
        
        // Map quality levels to URP assets
        // Assuming: 0 = Low, 1 = Medium, 2 = High
        switch (qualityLevel)
        {
            case 0:
                targetAsset = urpAssetLow;
                break;
            case 1:
                targetAsset = urpAssetMedium;
                break;
            case 2:
                targetAsset = urpAssetHigh;
                break;
            default:
                targetAsset = urpAssetMedium; // fallback to medium
                break;
        }
        
        if (targetAsset != null)
        {
            _currentUrpAsset = targetAsset;
            GraphicsSettings.defaultRenderPipeline = _currentUrpAsset;
            QualitySettings.renderPipeline = _currentUrpAsset;
        }
    }

    #endregion
}