using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VideoSettingsUI : MonoBehaviour
{
    [Header("Dropdowns")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown fullscreenDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown msaaDropdown;
    [SerializeField] private TMP_Dropdown fpsDropdown;

    [Header("Toggles")]
    [SerializeField] private Toggle shadowsToggle;
    [SerializeField] private Toggle bloomToggle;
    [SerializeField] private Toggle motionBlurToggle;

    private List<Resolution> _resolutions;

    private void Start()
    {
        InitResolutions();
        InitFullscreenModes();
        InitQuality();
        InitMSAA();
        InitFPS();
        
        InitShadows();
        InitBloom();
        InitMotionBlur();
        
        VideoManager.Instance.OnSettingsReverted += RefreshUI;
        LocalizationManager.Instance.OnLanguageChanged += OnLanguageChanged;
    }

    private void OnDestroy()
    {
        VideoManager.Instance.OnSettingsReverted += RefreshUI;
        LocalizationManager.Instance.OnLanguageChanged -= OnLanguageChanged;
    }

    private void OnLanguageChanged()
    {
        // Reinitialize dropdowns with new localized text
        InitFullscreenModes();
        InitQuality();
        InitMSAA();
        InitFPS();
    }

    #region Init

    private void InitResolutions()
    {
        if (!resolutionDropdown) return;
        
        _resolutions = VideoManager.Instance.GetAvailableResolutions();
        
        resolutionDropdown.ClearOptions();

        var options = _resolutions
            .Select(r => $"{r.width} x {r.height} ({r.refreshRate}Hz)")
            .ToList();

        resolutionDropdown.AddOptions(options);

        // Use saved resolution instead of current screen resolution
        var savedResolution = VideoManager.Instance.GetResolution();
        int currentIndex = _resolutions
            .FindIndex(r => r.width == savedResolution.width && r.height == savedResolution.height);

        resolutionDropdown.value = currentIndex >= 0 ? currentIndex : 0;
        resolutionDropdown.RefreshShownValue();

        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
    }

    private void InitFullscreenModes()
    {
        if (!fullscreenDropdown) return;
        
        fullscreenDropdown.ClearOptions();

        var options = new List<string>
        {
            LocalizationManager.Instance.GetText("ui_video_windowed"),
            LocalizationManager.Instance.GetText("ui_video_borderless"),
            LocalizationManager.Instance.GetText("ui_video_fullscreen")
        };

        fullscreenDropdown.AddOptions(options);

        var currentMode = VideoManager.Instance.GetFullscreenMode();

        int index = System.Array.IndexOf(VideoManager.Instance.AllowedFullscreenModes, currentMode);
        fullscreenDropdown.value = index >= 0 ? index : 1;

        fullscreenDropdown.RefreshShownValue();

        fullscreenDropdown.onValueChanged.AddListener(OnFullscreenChanged);
    }

    private void InitQuality()
    {
        if (!qualityDropdown) return;
        
        qualityDropdown.ClearOptions();

        // Use descriptive quality names that correspond to URP assets
        var options = new List<string>
        {
            LocalizationManager.Instance.GetText("ui_video_quality_low"),
            LocalizationManager.Instance.GetText("ui_video_quality_medium"), 
            LocalizationManager.Instance.GetText("ui_video_quality_high")
        };
        
        // Only add options for URP assets that actually exist
        var availableQualities = VideoManager.Instance.GetAvailableURPQualities();
        var filteredOptions = options.Where((option, index) => 
            availableQualities.Contains(index == 0 ? "Low" : index == 1 ? "Medium" : "High")).ToList();
        
        qualityDropdown.AddOptions(filteredOptions);

        // Use saved quality level instead of current Unity quality level
        qualityDropdown.value = VideoManager.Instance.GetQualityLevel();
        qualityDropdown.RefreshShownValue();

        qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
    }

    private void InitMSAA()
    {
        if (!msaaDropdown) return;
        
        msaaDropdown.ClearOptions();

        var options = new[] { 
            LocalizationManager.Instance.GetText("ui_off"), 
            "2x", 
            "4x", 
            "8x" 
        }.ToList();
        msaaDropdown.AddOptions(options);

        int current = VideoManager.Instance.GetMSAA();

        int index = current switch
        {
            0 => 0,
            2 => 1,
            4 => 2,
            8 => 3,
            _ => 1
        };

        msaaDropdown.value = index;
        msaaDropdown.RefreshShownValue();

        msaaDropdown.onValueChanged.AddListener(OnMSAAChanged);
    }

    private void InitFPS()
    {
        if (!fpsDropdown) return;
        
        fpsDropdown.ClearOptions();

        var options = new[] { 
            "30", 
            "60", 
            "90", 
            "120", 
            LocalizationManager.Instance.GetText("ui_video_fps_unlimited") 
        }.ToList();
        fpsDropdown.AddOptions(options);

        int currentFPS = VideoManager.Instance.GetFPSLimit();

        int index = currentFPS switch
        {
            30 => 0,
            60 => 1,
            90 => 2,
            120 => 3,
            -1 => 4,
            _ => 4
        };

        fpsDropdown.value = index;
        fpsDropdown.RefreshShownValue();

        fpsDropdown.onValueChanged.AddListener(OnFPSChanged);
    }

    private void InitShadows()
    {
        if (!shadowsToggle) return;
        
        shadowsToggle.isOn = VideoManager.Instance.GetShadows();
        shadowsToggle.onValueChanged.AddListener(VideoManager.Instance.SetShadows);
    }
    private void InitBloom()
    {
        if (!bloomToggle) return;

        bloomToggle.isOn = VideoManager.Instance.GetBloom();
        bloomToggle.onValueChanged.AddListener(VideoManager.Instance.SetBloom);
    }
    private void InitMotionBlur()
    {
        if (!motionBlurToggle) return;
        
        motionBlurToggle.isOn = VideoManager.Instance.GetMotionBlur();
        motionBlurToggle.onValueChanged.AddListener(VideoManager.Instance.SetMotionBlur);
    }

    #endregion

    #region Callbacks

    private void OnResolutionChanged(int index)
    {
        var res = _resolutions[index];
        VideoManager.Instance.SetResolution(res.width, res.height);
    }

    private void OnFullscreenChanged(int index)
    {
        var mode = VideoManager.Instance.AllowedFullscreenModes[index];
        VideoManager.Instance.SetFullscreenMode(mode);
    }

    private void OnQualityChanged(int index)
    {
        VideoManager.Instance.SetQualityLevel(index);
    }

    private void OnMSAAChanged(int index)
    {
        int samples = index switch
        {
            0 => 0,
            1 => 2,
            2 => 4,
            3 => 8,
            _ => 2
        };

        VideoManager.Instance.SetMSAA(samples);
    }

    private void OnFPSChanged(int index)
    {
        int fps = index switch
        {
            0 => 30,
            1 => 60,
            2 => 90,
            3 => 120,
            4 => -1, // Unlimited
            _ => -1
        };

        VideoManager.Instance.SetFPSLimit(fps);
    }

    #endregion

    #region UI Refresh

    private void RefreshUI()
    {
        RefreshResolution();
        RefreshFullscreen();
        RefreshQuality();
        RefreshMSAA();
        RefreshFPS();

        RefreshToggles();
    }
    
    private void RefreshResolution()
    {
        var saved = VideoManager.Instance.GetResolution();

        int index = _resolutions.FindIndex(r =>
            r.width == saved.width && r.height == saved.height);

        resolutionDropdown.SetValueWithoutNotify(index >= 0 ? index : 0);
        resolutionDropdown.RefreshShownValue();
    }
    
    private void RefreshFullscreen()
    {
        var mode = VideoManager.Instance.GetFullscreenMode();

        int index = System.Array.IndexOf(VideoManager.Instance.AllowedFullscreenModes, mode);

        fullscreenDropdown.SetValueWithoutNotify(index >= 0 ? index : 1);
        fullscreenDropdown.RefreshShownValue();
    }
    
    private void RefreshQuality()
    {
        qualityDropdown.SetValueWithoutNotify(VideoManager.Instance.GetQualityLevel());
        qualityDropdown.RefreshShownValue();
    }
    
    private void RefreshMSAA()
    {
        int current = VideoManager.Instance.GetMSAA();

        int index = current switch
        {
            0 => 0,
            2 => 1,
            4 => 2,
            8 => 3,
            _ => 1
        };

        msaaDropdown.SetValueWithoutNotify(index);
        msaaDropdown.RefreshShownValue();
    }
    
    private void RefreshFPS()
    {
        int current = VideoManager.Instance.GetFPSLimit();

        int index = current switch
        {
            30 => 0,
            60 => 1,
            90 => 2,
            120 => 3,
            -1 => 4,
            _ => 4
        };

        fpsDropdown.SetValueWithoutNotify(index);
        fpsDropdown.RefreshShownValue();
    }
    
    private void RefreshToggles()
    {
        shadowsToggle.SetIsOnWithoutNotify(VideoManager.Instance.GetShadows());
        bloomToggle.SetIsOnWithoutNotify(VideoManager.Instance.GetBloom());
        motionBlurToggle.SetIsOnWithoutNotify(VideoManager.Instance.GetMotionBlur());
    }

    #endregion
}