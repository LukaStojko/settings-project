using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageDropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;

    private List<string> _languages = new List<string>();

    private void Awake()
    {
        if (dropdown == null)
            dropdown = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        PopulateDropdown();

        dropdown.onValueChanged.AddListener(OnDropdownChanged);
        LocalizationManager.Instance.OnLanguageChanged += RefreshDropdown;
        LocalizationManager.Instance.OnSettingsReverted += RefreshDropdown;
    }

    private void OnDestroy()
    {
        dropdown.onValueChanged.RemoveListener(OnDropdownChanged);
        LocalizationManager.Instance.OnLanguageChanged -= RefreshDropdown;
        LocalizationManager.Instance.OnSettingsReverted -= RefreshDropdown;
    }

    private void PopulateDropdown()
    {
        dropdown.ClearOptions();
        _languages = LocalizationManager.Instance.GetAvailableLanguages();

        List<string> localizedOptions = new List<string>();
        int selectedIndex = 0;

        for (int i = 0; i < _languages.Count; i++)
        {
            string code = _languages[i];

            string localizedName = LocalizationManager.Instance.GetText($"game_language_{code}");
            localizedOptions.Add(localizedName);

            // Use pending language if set, otherwise current
            if (code == (LocalizationManager.Instance.PendingLanguage ?? LocalizationManager.Instance.CurrentLanguage))
                selectedIndex = i;
        }

        dropdown.AddOptions(localizedOptions);
        dropdown.value = selectedIndex;
        dropdown.RefreshShownValue();
    }

    private void RefreshDropdown()
    {
        // Rebuild to update localized names
        PopulateDropdown();
    }

    private void OnDropdownChanged(int index)
    {
        if (index < 0 || index >= _languages.Count)
            return;

        string selectedLanguage = _languages[index];
        LocalizationManager.Instance.SetLanguage(selectedLanguage);
    }
}