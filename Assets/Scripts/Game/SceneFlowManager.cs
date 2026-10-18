using System.Collections;
using UnityEngine;

public class SceneFlowManager : MonoBehaviour
{
    public static SceneFlowManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject mainMenuPrefab;
    [SerializeField] private GameObject gamePrefab;

    [Header("Transição Fade")]
    [SerializeField] private float fadeDuration = 0.35f;
    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private CanvasGroup fadeCanvasGroup;

    private GameObject currentInstance;
    private bool isTransitioning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsureFadeOverlay();
#if UNITY_STANDALONE && !UNITY_EDITOR
        if (Screen.width > Screen.height)
        {
            int targetHeight = Mathf.Min(Display.main.systemHeight - 100, 960);
            int targetWidth = Mathf.RoundToInt(targetHeight * (9f / 16f));
            Screen.SetResolution(targetWidth, targetHeight, FullScreenMode.Windowed);
        }
#endif
    }

    private void Start()
    {
        if (mainMenuPrefab != null)
        {
            InstantiateMainMenu();
        }
    }

    private void EnsureFadeOverlay()
    {
        if (fadeCanvasGroup != null) return;

        GameObject fadeCanvasObj = new GameObject("TransitionFadeCanvas");
        fadeCanvasObj.transform.SetParent(transform);

        Canvas canvas = fadeCanvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        fadeCanvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        fadeCanvasGroup = fadeCanvasObj.AddComponent<CanvasGroup>();
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;

        GameObject imgObj = new GameObject("FadeScreenImage");
        imgObj.transform.SetParent(fadeCanvasObj.transform, false);

        UnityEngine.UI.Image img = imgObj.AddComponent<UnityEngine.UI.Image>();
        img.color = fadeColor;

        RectTransform rt = img.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
    }

    public void LoadMainMenu()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(() => InstantiateMainMenu()));
    }

    public void LoadGame()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(() => InstantiateGame()));
    }

    public void RestartGame()
    {
        if (isTransitioning) return;
        StartCoroutine(TransitionRoutine(() => InstantiateGame()));
    }

    private IEnumerator TransitionRoutine(System.Action loadAction)
    {
        isTransitioning = true;
        Time.timeScale = 1f;

        EnsureFadeOverlay();

        fadeCanvasGroup.blocksRaycasts = true;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeDuration);
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f;

        if (currentInstance != null)
        {
            Destroy(currentInstance);
        }

        yield return null;

        loadAction?.Invoke();

        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsed / fadeDuration));
            yield return null;
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.blocksRaycasts = false;
        isTransitioning = false;
    }

    private void InstantiateMainMenu()
    {
        if (mainMenuPrefab == null) return;

        currentInstance = Instantiate(mainMenuPrefab);
    }

    private void InstantiateGame()
    {
        if (gamePrefab == null) return;

        currentInstance = Instantiate(gamePrefab);
    }
}
