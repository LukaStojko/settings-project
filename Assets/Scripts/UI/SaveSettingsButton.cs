using UnityEngine;

public class SaveSettingsButton : MonoBehaviour
{
    public void SaveSettings()
    {
        AudioManager.Instance.Save();
        VideoManager.Instance.Save();
        LocalizationManager.Instance.Save();
    }
}