using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string TextCode;
    [SerializeField] private TextMeshProUGUI TextBox;
    
    void Start()
    {
        LocalizationManager.Instance.OnLanguageChanged += SetText;
        
        SetText();
    }

    void SetText()
    {
        string localizedText = LocalizationManager.Instance.GetText(TextCode);
        if(TextBox) TextBox.text = localizedText;
    }
}
