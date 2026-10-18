using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(StartGame);
            startButton.onClick.AddListener(StartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(QuitGame);
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void StartGame()
    {
        SFXManager.Instance?.PlaySFX("click");

        if (SceneFlowManager.Instance != null)
        {
            SceneFlowManager.Instance.LoadGame();
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
    }

    public void QuitGame()
    {
        SFXManager.Instance?.PlaySFX("click");

        Debug.Log("[MainMenu] Fechando o jogo...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
