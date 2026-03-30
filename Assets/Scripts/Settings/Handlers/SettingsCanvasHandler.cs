using UnityEngine;

public class SettingsCanvasHandler : BaseCanvasHandler
{
    [SerializeField] private GameObject settingsButtonGroup;
    [SerializeField] private GameObject videoSettingsGroup;
    [SerializeField] private GameObject audioSettingsGroup;
    [SerializeField] private GameObject controlsSettingsGroup;
    [SerializeField] private GameObject accessabilitySettingsGroup;
    [SerializeField] private GameObject confirmSettingsWindow;
    
    public GameObject SettingsButtonGroup => settingsButtonGroup;
    public GameObject VideoSettingsGroup => videoSettingsGroup;
    public GameObject AudioSettingsGroup => audioSettingsGroup;
    public GameObject ControlsSettingsGroup => controlsSettingsGroup;
    public GameObject AccessabilitySettingsGroup => accessabilitySettingsGroup;
    public GameObject ConfirmSettingsWindow => confirmSettingsWindow;
}
