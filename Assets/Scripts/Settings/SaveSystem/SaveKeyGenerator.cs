using UnityEditor;
using UnityEngine;
using System.Text;

[CustomEditor(typeof(SaveManager))]
public class SaveSystemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SaveManager saveManager = (SaveManager)target;

        GUILayout.Space(10);

        if (GUILayout.Button("Generate New Encryption Key"))
        {
            string newKey = GenerateRandomKey(32);

            Undo.RecordObject(saveManager, "Generate Encryption Key");
            SetPrivateField(saveManager, "currentKey", newKey);

            EditorUtility.SetDirty(saveManager);

            Debug.Log("New encryption key generated:\n" + newKey);
        }
    }

    private string GenerateRandomKey(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder sb = new StringBuilder();
        System.Random rng = new System.Random();

        for (int i = 0; i < length; i++)
        {
            sb.Append(chars[rng.Next(chars.Length)]);
        }

        return sb.ToString();
    }

    // 🔧 Set private serialized field via reflection
    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName,
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        if (field != null)
        {
            field.SetValue(target, value);
        }
        else
        {
            Debug.LogError("Field not found: " + fieldName);
        }
    }
}