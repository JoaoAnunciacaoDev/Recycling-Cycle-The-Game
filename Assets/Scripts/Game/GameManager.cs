using UnityEngine.SceneManagement;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private CanvasGroup gameOverPanel;
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private TMPro.TextMeshProUGUI finalTimeLabel;
    [SerializeField] private TMPro.TextMeshProUGUI finalScoreLabel;

    [Header("Settings")]
    [SerializeField] private int maxLives = 3;
    [SerializeField] private float fadeDuration = 1.0f;

    public int currentLives { get; private set; }
    public int currentScore {get; private set;}

    private void Awake()
    {
        currentLives = maxLives;
        currentScore = 0;

        if (gameOverPanel)
        {
            gameOverPanel.alpha = 0f;
            gameOverPanel.interactable = false;
            gameOverPanel.blocksRaycasts = false;
            gameOverPanel.gameObject.SetActive(false);
        }
    }

    public void AddScore(int score)
    {
        currentScore += score;
    }

    public void TakeDamage(int damage)
    {
        currentLives = Mathf.Max(currentLives - damage, 0);

        if (currentLives == 0)
            GameOver();
    }

    private void GameOver()
    {
        int minutes = Mathf.FloorToInt(gameTimer.ElapsedTime / 60f);
        int seconds = Mathf.FloorToInt(gameTimer.ElapsedTime % 60f);

        finalTimeLabel.text = $"{minutes:00}:{seconds:00}";
        finalScoreLabel.text = $"Score: {currentScore}";
        gameOverPanel.gameObject.SetActive(true);

        Time.timeScale = 0f;
        StartCoroutine(FadeInGameOverRoutine());
    }

    private System.Collections.IEnumerator FadeInGameOverRoutine()
    {
        gameOverPanel.gameObject.SetActive(true);
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            gameOverPanel.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        gameOverPanel.alpha = 1f;
        gameOverPanel.interactable = true;
        gameOverPanel.blocksRaycasts = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MenuGame()
    {
        StopAllCoroutines();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
