using TMPro;
using UnityEngine;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string translationKey;
    [SerializeField] private TextMeshProUGUI textComponent;

    private void Awake()
    {
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }
    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    private void Start()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (textComponent == null || string.IsNullOrEmpty(translationKey)) return;

        if (LocalizationManager.Instance != null)
        {
            textComponent.text = LocalizationManager.Instance.GetText(translationKey);
        }
    }

    public void SetKey(string newKey)
    {
        translationKey = newKey;
        UpdateText();
    }
}
