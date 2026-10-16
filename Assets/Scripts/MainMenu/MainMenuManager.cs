using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private void Start()
    {
        // Conecta automaticamente os botões caso o prefab tenha perdido as referências do Inspector
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            string lowerName = btn.gameObject.name.ToLower();

            if (lowerName.Contains("start") || lowerName.Contains("play"))
            {
                btn.onClick.RemoveListener(StartGame);
                btn.onClick.AddListener(StartGame);
            }
            else if (lowerName.Contains("exit") || lowerName.Contains("quit"))
            {
                btn.onClick.RemoveListener(QuitGame);
                btn.onClick.AddListener(QuitGame);
            }
        }
    }

    public void StartGame()
    {
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
        Debug.Log("[MainMenu] Fechando o jogo...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
