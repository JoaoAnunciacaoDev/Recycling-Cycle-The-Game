using UnityEngine;
using UnityEngine.InputSystem;

public class MenuParallax : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;

    public float offsetMultiplier = 1f;
    public float smoothTime = 0.3f;

    private RectTransform rectTransform;
    private Vector2 initialAnchoredPosition;
    private Vector3 initialWorldPosition;
    private Vector2 uiVelocity;
    private Vector3 worldVelocity;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    private void Start()
    {
        if (rectTransform != null)
        {
            Canvas.ForceUpdateCanvases();
            initialAnchoredPosition = rectTransform.anchoredPosition;
        }
        else
        {
            initialWorldPosition = transform.position;
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector3 viewportPoint = mainCamera.ScreenToViewportPoint(mousePos);
        Vector2 centeredOffset = new Vector2(viewportPoint.x - 0.5f, viewportPoint.y - 0.5f);

        if (rectTransform != null)
        {
            Vector2 targetPos = initialAnchoredPosition + (centeredOffset * offsetMultiplier);
            rectTransform.anchoredPosition = Vector2.SmoothDamp(
                rectTransform.anchoredPosition,
                targetPos,
                ref uiVelocity,
                smoothTime,
                Mathf.Infinity,
                Time.unscaledDeltaTime
            );
        }
        else
        {
            Vector3 targetPos = initialWorldPosition + ((Vector3)centeredOffset * offsetMultiplier);
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPos,
                ref worldVelocity,
                smoothTime,
                Mathf.Infinity,
                Time.unscaledDeltaTime
            );
        }
    }
}
