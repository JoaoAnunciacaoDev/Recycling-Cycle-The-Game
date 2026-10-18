using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Painéis")]
    [SerializeField] private GameObject mainMenuContainer;
    [SerializeField] private GameObject optionsContainer;

    [Header("Controles de Volume")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Botões")]
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button backButton;

    [Header("Idioma (Opcional)")]
    [SerializeField] private Button languageButton;
    [SerializeField] private TMPro.TextMeshProUGUI languageButtonText;

    private void Start()
    {
        SetupSliders();
        SetupButtons();
    }

    private void OnEnable()
    {
        LocalizationManager.OnLanguageChanged += UpdateLanguageButtonText;
        UpdateLanguageButtonText();
    }

    private void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateLanguageButtonText;
    }

    private void SetupSliders()
    {
        if (musicSlider != null)
        {
            float savedMusicVol = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
            if (MusicManager.Instance != null) savedMusicVol = MusicManager.Instance.GetVolume();

            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.value = savedMusicVol;

            musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            float savedSfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);
            if (SFXManager.Instance != null) savedSfxVol = SFXManager.Instance.GetVolume();

            sfxSlider.minValue = 0f;
            sfxSlider.maxValue = 1f;
            sfxSlider.value = savedSfxVol;

            sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }
    }

    private void SetupButtons()
    {
        if (optionsButton != null)
        {
            optionsButton.onClick.RemoveListener(OpenOptions);
            optionsButton.onClick.AddListener(OpenOptions);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(CloseOptions);
            backButton.onClick.AddListener(CloseOptions);
        }

        if (languageButton != null)
        {
            languageButton.onClick.RemoveListener(OnLanguageButtonClicked);
            languageButton.onClick.AddListener(OnLanguageButtonClicked);
            UpdateLanguageButtonText();
        }
    }

    private void OnLanguageButtonClicked()
    {
        SFXManager.Instance?.PlaySFX("click");
        LocalizationManager.Instance?.ToggleLanguage();
    }

    private void UpdateLanguageButtonText()
    {
        if (languageButtonText != null && LocalizationManager.Instance != null)
        {
            languageButtonText.text = LocalizationManager.Instance.GetText("ui.language");
        }
    }

    public void OpenOptions()
    {
        SFXManager.Instance?.PlaySFX("click");

        if (mainMenuContainer != null) mainMenuContainer.SetActive(false);
        if (optionsContainer != null) optionsContainer.SetActive(true);
    }

    public void CloseOptions()
    {
        SFXManager.Instance?.PlaySFX("click");

        if (optionsContainer != null) optionsContainer.SetActive(false);
        if (mainMenuContainer != null) mainMenuContainer.SetActive(true);
    }

    public void OnMusicVolumeChanged(float value)
    {
        MusicManager.Instance?.SetVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        SFXManager.Instance?.SetVolume(value);
    }
}
