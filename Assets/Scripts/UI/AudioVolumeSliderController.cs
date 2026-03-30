using UnityEngine;
using UnityEngine.UI;

public class AudioVolumeSliderController : MonoBehaviour
{
    public enum VolumeType
    {
        Master,
        Music,
        SFX,
        Voice
    }

    [SerializeField] private Slider slider;
    [SerializeField] private VolumeType volumeType;
    [SerializeField] private float stepSize = 0.05f;

    private void Start()
    {
        if (slider == null)
            slider = GetComponent<Slider>();
        
        slider.value = GetVolume();
        slider.onValueChanged.AddListener(OnSliderChanged);
        AudioManager.Instance.OnSettingsReverted += UpdateSlider;
    }

    private void OnDestroy()
    {
        slider.onValueChanged.RemoveListener(OnSliderChanged);
        AudioManager.Instance.OnSettingsReverted -= UpdateSlider;
    }
    
    private void UpdateSlider()
    {
        slider.SetValueWithoutNotify(GetVolume());
    }
    
    private void OnSliderChanged(float value)
    {
        float steppedValue = SnapToStep(value);
    
        // Update slider visually to snapped value (without loop)
        slider.SetValueWithoutNotify(steppedValue);
        
        SetVolume(steppedValue);
    }
    
    private float SnapToStep(float value)
    {
        if (stepSize <= 0f) return value;

        float stepped = Mathf.Round(value / stepSize) * stepSize;
        return Mathf.Clamp01(stepped);
    }

    private float GetVolume()
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                return AudioManager.Instance.GetMasterVolume();
            case VolumeType.Music:
                return AudioManager.Instance.GetMusicVolume();
            case VolumeType.SFX:
                return AudioManager.Instance.GetSFXVolume();
            case VolumeType.Voice:
                return AudioManager.Instance.GetVoiceVolume();
            default:
                return 1f;
        }
    }

    private void SetVolume(float value)
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                AudioManager.Instance.SetMasterVolume(value);
                break;
            case VolumeType.Music:
                AudioManager.Instance.SetMusicVolume(value);
                break;
            case VolumeType.SFX:
                AudioManager.Instance.SetSFXVolume(value);
                break;
            case VolumeType.Voice:
                AudioManager.Instance.SetVoiceVolume(value);
                break;
        }
    }

    private void PlayTestSound()
    {
        switch (volumeType)
        {
            case VolumeType.Music:
                AudioManager.Instance.TestMusicClip();
                break;
            case VolumeType.SFX:
                AudioManager.Instance.TestSFXClip();
                break;
            case VolumeType.Voice:
                AudioManager.Instance.TestVoiceClip();
                break;
        }
    }
}
