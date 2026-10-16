using UnityEngine;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject mainMenuPrefab;
    [SerializeField] private GameObject gamePrefab;

    private GameObject currentInstance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        LoadMainMenu();
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        if (mainMenuPrefab != null)
        {
            currentInstance = Instantiate(mainMenuPrefab);

            MainMenuManager menuManager = currentInstance.GetComponentInChildren<MainMenuManager>();
            if (menuManager == null)
            {
                currentInstance.AddComponent<MainMenuManager>();
            }
        }
    }

    public void LoadGame()
    {
        Time.timeScale = 1f;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        if (gamePrefab != null)
        {
            // Instancia na raiz da cena
            currentInstance = Instantiate(gamePrefab);
        }
    }

    public void RestartGame()
    {
        LoadGame();
    }
}
