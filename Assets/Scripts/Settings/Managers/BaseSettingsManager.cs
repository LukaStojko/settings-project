using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class BaseSettingsManager<T> : MonoBehaviour where T : BaseSettingsManager<T>
{
    public static T Instance { get; private set; }

    protected static string FilePath => Path.Combine(Application.persistentDataPath, "settings.ini");

    protected Dictionary<string, Dictionary<string, string>> data =
        new Dictionary<string, Dictionary<string, string>>();

    protected Dictionary<string, Dictionary<string, string>> loadedData =
        new Dictionary<string, Dictionary<string, string>>();

    public bool IsDirty { get; protected set; } = false;
    
    public event Action OnSettingsLoaded;
    public event Action OnSettingsReverted;

    #region Unity Lifecycle

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = (T)this;
        DontDestroyOnLoad(gameObject);
        Load();
    }

    protected virtual void Start()
    {
        
    }

    #endregion

    #region Core Methods

    public virtual void Load()
    {
        // Load shared settings file if it exists
        Dictionary<string, Dictionary<string, string>> sharedData = new Dictionary<string, Dictionary<string, string>>();
        
        if (File.Exists(FilePath))
        {
            sharedData = ParseSettingsFile();
        }
        else
        {
            CreateDefaults();
        }

        data.Clear();
        loadedData.Clear();

        // Copy relevant data for this manager
        string managerSection = GetManagerSection();
        if (sharedData.ContainsKey(managerSection))
        {
            data[managerSection] = new Dictionary<string, string>(sharedData[managerSection]);
        }

        // Save if something was added
        //if (IsDirty)
        //    Save();

        loadedData = DeepCopy(data);
        IsDirty = false;

        OnLoaded();
        OnSettingsLoaded?.Invoke();
    }

    private Dictionary<string, Dictionary<string, string>> ParseSettingsFile()
    {
        var result = new Dictionary<string, Dictionary<string, string>>();
        string currentSection = "";

        foreach (var line in File.ReadAllLines(FilePath))
        {
            string trimmed = line.Trim();

            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith(";"))
                continue;

            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                currentSection = trimmed.Substring(1, trimmed.Length - 2);

                if (!result.ContainsKey(currentSection))
                    result[currentSection] = new Dictionary<string, string>();

                continue;
            }

            var split = trimmed.Split('=');
            if (split.Length != 2) continue;

            string key = split[0].Trim();
            string value = split[1].Trim();

            if (!result.ContainsKey(currentSection))
                result[currentSection] = new Dictionary<string, string>();

            result[currentSection][key] = value;
        }

        return result;
    }

    public virtual void Save()
    {
        // Load existing data to preserve other managers' sections
        Dictionary<string, Dictionary<string, string>> existingData = new Dictionary<string, Dictionary<string, string>>();
        
        if (File.Exists(FilePath))
        {
            existingData = ParseSettingsFile();
        }

        // Update this manager's section
        string managerSection = GetManagerSection();
        if (data.ContainsKey(managerSection))
        {
            existingData[managerSection] = new Dictionary<string, string>(data[managerSection]);
        }

        // Write all sections back to file
        using (StreamWriter writer = new StreamWriter(FilePath))
        {
            foreach (var section in existingData)
            {
                writer.WriteLine($"[{section.Key}]");

                foreach (var pair in section.Value)
                {
                    writer.WriteLine($"{pair.Key}={pair.Value}");
                }

                writer.WriteLine();
            }
        }

        loadedData = DeepCopy(data);
        IsDirty = false;

        Debug.Log($"Settings saved for {managerSection}.");
    }

    public virtual void Revert()
    {
        data = DeepCopy(loadedData);
        IsDirty = false;

        OnReverted();
        OnSettingsReverted?.Invoke();
    }

    #endregion

    #region Get/Set Helpers

    protected void SetValue(string section, string key, string value)
    {
        if (!data.ContainsKey(section))
            data[section] = new Dictionary<string, string>();

        if (!data[section].ContainsKey(key) || data[section][key] != value)
        {
            data[section][key] = value;
            IsDirty = true;
        }
    }

    protected string GetValue(string section, string key, string defaultValue = "")
    {
        if (data.ContainsKey(section) && data[section].ContainsKey(key))
            return data[section][key];

        return defaultValue;
    }

    protected int GetInt(string section, string key, int defaultValue = 0)
    {
        return int.TryParse(GetValue(section, key), out int result) ? result : defaultValue;
    }

    protected float GetFloat(string section, string key, float defaultValue = 0f)
    {
        return float.TryParse(GetValue(section, key), out float result) ? result : defaultValue;
    }

    protected bool GetBool(string section, string key, bool defaultValue = false)
    {
        return bool.TryParse(GetValue(section, key), out bool result) ? result : defaultValue;
    }

    #endregion

    #region Utilities

    private Dictionary<string, Dictionary<string, string>> DeepCopy(
        Dictionary<string, Dictionary<string, string>> original)
    {
        var copy = new Dictionary<string, Dictionary<string, string>>();

        foreach (var section in original)
        {
            copy[section.Key] = new Dictionary<string, string>(section.Value);
        }

        return copy;
    }

    protected virtual string GetManagerSection()
    {
        return typeof(T).Name.Replace("Manager", "");
    }

    #endregion

    #region Abstract Hooks

    protected abstract void CreateDefaults();
    protected virtual void OnLoaded() { }
    protected virtual void OnReverted() { }

    #endregion
}