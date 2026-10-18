using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public enum Language
    {
        Portuguese,
        English
    }

    public static LocalizationManager Instance { get; private set; }
    public static event Action OnLanguageChanged;

    [SerializeField] private Language defaultLanguage = Language.Portuguese;

    public Language CurrentLanguage { get; private set; }

    private Dictionary<string, Dictionary<Language, string>> translations;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        int savedLanguage = PlayerPrefs.GetInt("GameLanguage", (int)defaultLanguage);
        CurrentLanguage = (Language)savedLanguage;

        InitializeTranslations();
    }

    private void InitializeTranslations()
    {
        translations = new Dictionary<string, Dictionary<Language, string>>(StringComparer.OrdinalIgnoreCase);

        AddTranslation("ui.start", "Jogar", "Play");
        AddTranslation("ui.options", "Opcoes", "Options");
        AddTranslation("ui.quit", "Sair", "Quit");
        AddTranslation("ui.back", "Voltar", "Back");
        AddTranslation("ui.song", "Musica", "Music");
        AddTranslation("ui.sfx", "Efeitos", "SFX");
        AddTranslation("ui.language", "Idioma: PT", "Language: EN");
        AddTranslation("ui.gameover", "Fim de Jogo", "Game Over");
        AddTranslation("ui.restart", "Recomeçar", "Restart");
        AddTranslation("ui.menu", "Menu", "Menu");
        AddTranslation("game.score", "Pontos", "Score");
        AddTranslation("game.time", "Tempo", "Time");
    }

    public void AddTranslation(string key, string ptText, string enText)
    {
        if (!translations.ContainsKey(key))
        {
            translations[key] = new Dictionary<Language, string>();
        }

        translations[key][Language.Portuguese] = ptText;
        translations[key][Language.English] = enText;
    }

    public string GetText(string key)
    {
        if (translations != null && translations.TryGetValue(key, out var langMap))
        {
            if (langMap.TryGetValue(CurrentLanguage, out string text))
            {
                return text;
            }
        }
        return key;
    }

    public void SetLanguage(Language newLanguage)
    {
        CurrentLanguage = newLanguage;
        PlayerPrefs.SetInt("GameLanguage", (int)CurrentLanguage);
        PlayerPrefs.Save();
        OnLanguageChanged?.Invoke();
    }

    public void ToggleLanguage()
    {
        Language nextLanguage = (CurrentLanguage == Language.Portuguese) ? Language.English : Language.Portuguese;
        SetLanguage(nextLanguage);
    }
}
