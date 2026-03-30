using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LocalizationManager : BaseSettingsManager<LocalizationManager>
{
    public event Action OnLanguageChanged;

    private const string SECTION = "Localization";
    private const string KEY_LANGUAGE = "Language";

    private readonly Dictionary<string, string> _localizedText = new Dictionary<string, string>();

    public string CurrentLanguage { get; private set; } = "en";
    
    private string _pendingLanguage = null;
    public string PendingLanguage => _pendingLanguage;

    #region Initialization

    protected override void CreateDefaults()
    {
        SetValue(SECTION, KEY_LANGUAGE, "en");
    }

    protected override void OnLoaded()
    {
        CurrentLanguage = GetValue(SECTION, KEY_LANGUAGE, "en");
        LoadLanguage(CurrentLanguage);
        
        OnLanguageChanged?.Invoke();
    }

    protected override void OnReverted()
    {
        CurrentLanguage = GetValue(SECTION, KEY_LANGUAGE, "en");
        LoadLanguage(CurrentLanguage);
        
        OnLanguageChanged?.Invoke();
    }

    #endregion

    #region Language Handling

    public void SetLanguage(string languageCode)
    {
        if (languageCode == _pendingLanguage || languageCode == CurrentLanguage)
            return;

        // Set the pending value
        _pendingLanguage = languageCode;
    
        SetValue(SECTION, KEY_LANGUAGE, _pendingLanguage);
        
        // if (CurrentLanguage == languageCode)
        //     return;
        //
        // SetValue(SECTION, KEY_LANGUAGE, languageCode);
        //
        // CurrentLanguage = languageCode;
        // LoadLanguage(CurrentLanguage);
        // OnLanguageChanged?.Invoke();
    }

    private void LoadLanguage(string languageCode)
    {
        _localizedText.Clear();

        // Load all CSV TextAssets in Resources/Localization/<languageCode>
        string path = $"Localization/{languageCode}";
        TextAsset[] files = Resources.LoadAll<TextAsset>(path);

        if (files == null || files.Length == 0)
        {
            Debug.LogWarning($"No localization files found for language: {languageCode}");
            return;
        }

        foreach (var file in files)
        {
            ParseCSV(file.text);
        }

        Debug.Log($"Loaded {_localizedText.Count} localization entries for '{languageCode}'");
    }

    #endregion

    #region CSV Parsing

    private void ParseCSV(string csvText)
    {
        using (StringReader reader = new StringReader(csvText))
        {
            bool isHeader = true;

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null) break;

                if (isHeader)
                {
                    isHeader = false;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var split = line.Split(',');

                if (split.Length < 2)
                    continue;

                string key = split[0].Trim();
                string value = split[1].Trim();

                if (!_localizedText.ContainsKey(key))
                {
                    _localizedText[key] = value;
                }
                else
                {
                    Debug.LogWarning($"Duplicate localization key: {key}");
                }
            }
        }
    }

    #endregion

    #region Public API
    
    public override void Save()
    {
        base.Save();

        if (!string.IsNullOrEmpty(_pendingLanguage) && _pendingLanguage != CurrentLanguage)
        {
            CurrentLanguage = _pendingLanguage;
            LoadLanguage(CurrentLanguage);
            _pendingLanguage = null;
            OnLanguageChanged?.Invoke();
        }
    }
    
    public override void Revert()
    {
        // Discard pending language
        _pendingLanguage = null;
        
        base.Revert();
    }

    public string GetText(string key)
    {
        if (_localizedText.TryGetValue(key, out var value))
            return value;

        return $"#{key}#"; // fallback for missing keys
    }

    public bool HasKey(string key)
    {
        return _localizedText.ContainsKey(key);
    }
    
    public List<string> GetAvailableLanguages()
    {
        HashSet<string> languages = new HashSet<string>();

        // Load ALL TextAssets inside Localization (including subfolders)
        TextAsset[] allFiles = Resources.LoadAll<TextAsset>("Localization");

        foreach (var file in allFiles)
        {
            // Path looks like: Localization/en/fileName
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(file);

            // Normalize and split
            string[] parts = assetPath.Split('/');

            int locIndex = Array.IndexOf(parts, "Localization");
            if (locIndex >= 0 && parts.Length > locIndex + 1)
            {
                string language = parts[locIndex + 1];
                languages.Add(language);
            }
        }

        return new List<string>(languages);
    }

    #endregion
}