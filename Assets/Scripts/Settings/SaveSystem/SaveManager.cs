using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    
    private string saveFolder;
    
    // Current key (used for saving new files)
    [SerializeField] private string currentKey = "GENERATE_ME";

    // Old keys (for loading old saves)
    [SerializeField] private List<string> oldKeys;

    private const int CURRENT_VERSION = 1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        saveFolder = Path.Combine(Application.persistentDataPath, "Saves");

        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
    }

    #region Core

    // SAVE
    public void Save(SaveData data, int slot)
    {
        data.version = CURRENT_VERSION;

        string json = JsonUtility.ToJson(data);
        string encrypted = Encrypt(json, currentKey);

        File.WriteAllText(GetPath(slot), encrypted);
    }

    // LOAD
    public SaveData Load(int slot)
    {
        string path = GetPath(slot);

        if (!File.Exists(path))
            return null;

        string encrypted = File.ReadAllText(path);

        if (TryDecrypt(encrypted, currentKey, out SaveData data))
            return UpgradeSave(data);

        if (oldKeys != null)
        {
            foreach (var key in oldKeys)
            {
                if (TryDecrypt(encrypted, key, out data))
                {
                    Debug.Log("Loaded save with old key. Re-saving with new key...");
                    Save(data, slot);
                    return UpgradeSave(data);
                }
            }
        }

        Debug.LogError("Failed to decrypt save file!");
        return null;
    }

    // Try decrypt safely
    private bool TryDecrypt(string encrypted, string key, out SaveData data)
    {
        data = null;

        try
        {
            string json = Decrypt(encrypted, key);
            data = JsonUtility.FromJson<SaveData>(json);

            return data != null;
        }
        catch
        {
            return false;
        }
    }

    // VERSION UPGRADE
    private SaveData UpgradeSave(SaveData data)
    {
        if (data.version < 2)
        {
            // Example upgrade
            data.gold = 0;
            data.version = 2;
        }

        return data;
    }

    // ENCRYPT
    private string Encrypt(string plainText, string key)
    {
        byte[] keyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;
            aes.GenerateIV();

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                ms.Write(aes.IV, 0, aes.IV.Length);

                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    // DECRYPT
    private string Decrypt(string cipherText, string key)
    {
        byte[] fullData = Convert.FromBase64String(cipherText);
        byte[] keyBytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(key));

        using (Aes aes = Aes.Create())
        {
            aes.Key = keyBytes;

            byte[] iv = new byte[16];
            Array.Copy(fullData, iv, iv.Length);
            aes.IV = iv;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(fullData, iv.Length, fullData.Length - iv.Length))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var sr = new StreamReader(cs))
            {
                return sr.ReadToEnd();
            }
        }
    }
    
    #endregion

    #region Systems

    public bool SaveExists(int slot)
    {
        return File.Exists(GetPath(slot));
    }
    
    public void DeleteSave(int slot)
    {
        string path = GetPath(slot);

        if (File.Exists(path))
            File.Delete(path);
    }

    #endregion

    #region Helpers
    
    private string GetPath(int slot)
    {
        return Path.Combine(saveFolder, $"save_{slot}.dat");
    }

    #endregion
}