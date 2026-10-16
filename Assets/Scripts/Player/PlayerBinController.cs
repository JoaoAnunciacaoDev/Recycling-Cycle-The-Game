using UnityEngine;

public class PlayerBinController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Collider2D collider2D;
    [SerializeField] private float smoothSpeed = 10f;
    [SerializeField] private TrashType type;

    [SerializeField] private TrashConfig trashConfig;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private RectTransform touchBar;
    [SerializeField] private RectTransform topBar;

    [Header("Inclinação")]
    [SerializeField] private float maxTiltAngle = 15f;
    [SerializeField] private float tiltSensitivity = 8f;
    [SerializeField] private float tiltSmoothSpeed = 12f;

    [Header("Squash and Stretch")]
    [SerializeField] private float squashDuration = 0.22f;
    [SerializeField] private Vector3 squashScale = new Vector3(1.25f, 0.75f, 1f);
    [SerializeField] private Vector3 stretchScale = new Vector3(0.9f, 1.15f, 1f);

    private InputSystem_Actions inputActions;
    private bool isDragging;
    private Vector3 targetPosition;
    private Vector3 initialScale;
    private float targetTiltAngle;
    private Coroutine squashCoroutine;

    public TrashType Type => type;   

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        if (mainCamera == null) mainCamera = Camera.main;
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        if (collider2D == null) collider2D = GetComponent<Collider2D>();
        if (gameManager == null) gameManager = FindAnyObjectByType<GameManager>();
    }

    private void Start()
    {
        initialScale = transform.localScale;
        UpdateSprite();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        ChangeTrashType();

        Vector2 pointerPosition = inputActions.Player.PointerPosition.ReadValue<Vector2>();

        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(
                pointerPosition.x,
                pointerPosition.y,
                -mainCamera.transform.position.z
            )
        );

        if (inputActions.Player.PointerPress.WasPressedThisFrame())
        {
            Vector2 point = new Vector2(
                worldPosition.x,
                worldPosition.y
            );

            isDragging = collider2D.OverlapPoint(point);

            if (isDragging)
            {
                targetPosition = worldPosition;
            }
        }

        if (isDragging)
        {
            targetPosition = ClampToBounds(new Vector3(
                worldPosition.x,
                worldPosition.y,
                transform.position.z
            ));

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                smoothSpeed * Time.deltaTime
            );

            float deltaX = targetPosition.x - transform.position.x;
            targetTiltAngle = Mathf.Clamp(-deltaX * tiltSensitivity, -maxTiltAngle, maxTiltAngle);
        }
        else
        {
            targetTiltAngle = 0f;
        }

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetTiltAngle);
        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            targetRotation,
            tiltSmoothSpeed * Time.deltaTime
        );

        if (inputActions.Player.PointerPress.WasReleasedThisFrame())
        {
            isDragging = false;
        }
    }

    private void ChangeTrashType()
    {
        bool typeChanged = false;

        if (inputActions.Player.SelectPaper.WasPressedThisFrame())
        {
            type = TrashType.Paper;
            typeChanged = true;
        }
            
        if (inputActions.Player.SelectPlastic.WasPressedThisFrame())
        {
            type = TrashType.Plastic;
            typeChanged = true;
        }
            
        if (inputActions.Player.SelectGlass.WasPressedThisFrame())
        {
            type = TrashType.Glass;
            typeChanged = true;
        }
            
        if (inputActions.Player.SelectMetal.WasPressedThisFrame())
        {
            type = TrashType.Metal;
            typeChanged = true;
        }
            
        if (inputActions.Player.SelectOrganic.WasPressedThisFrame())
        {
            type = TrashType.Organic;
            typeChanged = true;
        }
        
        if (typeChanged)
            UpdateSprite();
    }

    public void SetTrashType(TrashType newType)
    {
        type = newType;
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (sr != null && trashConfig != null)
            sr.sprite = trashConfig.GetBinSpriteForType(type);
    }

    private Vector3 ClampToBounds(Vector3 position)
    {
        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        float halfObjectWidth = collider2D.bounds.extents.x;
        float halfObjectHeight = collider2D.bounds.extents.y;

        float minX = -halfWidth + halfObjectWidth;
        float maxX = halfWidth - halfObjectWidth;

        float minY;

        if (touchBar != null)
        {
            Vector3[] bottomCorners = new Vector3[4];
            touchBar.GetWorldCorners(bottomCorners);
            minY = mainCamera.ScreenToWorldPoint(
                new Vector3(0f, bottomCorners[1].y, -mainCamera.transform.position.z)
            ).y + halfObjectHeight;
        }
        else
        {
            minY = -halfHeight + halfObjectHeight;
        }

        float maxY;

        if (topBar != null)
        {
            Vector3[] topCorners = new Vector3[4];
            topBar.GetWorldCorners(topCorners);
            maxY = mainCamera.ScreenToWorldPoint(
                new Vector3(0f, topCorners[0].y, -mainCamera.transform.position.z)
            ).y - halfObjectHeight;
        }
        else
        {
            maxY = halfHeight - halfObjectHeight;
        }

        return new Vector3(
            Mathf.Clamp(position.x, minX, maxX),
            Mathf.Clamp(position.y, minY, maxY),
            position.z
        );
    }

    private void TryCollectTrash(Collider2D other)
    {
        Trash trash = other.GetComponent<Trash>();

        if (trash == null) return;
        if (trash.Type != type) return;

        gameManager.AddScore(1);
        PlaySquashAndStretch();
        Destroy(trash.gameObject);
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryCollectTrash(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryCollectTrash(other);
    }

    private void PlaySquashAndStretch()
    {
        if (squashCoroutine != null)
            StopCoroutine(squashCoroutine);

        squashCoroutine = StartCoroutine(SquashAndStretchRoutine());
    }

    private System.Collections.IEnumerator SquashAndStretchRoutine()
    {
        Vector3 targetSquash = new Vector3(
            initialScale.x * squashScale.x,
            initialScale.y * squashScale.y,
            initialScale.z
        );

        Vector3 targetStretch = new Vector3(
            initialScale.x * stretchScale.x,
            initialScale.y * stretchScale.y,
            initialScale.z
        );

        float halfDuration = squashDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < halfDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (halfDuration * 0.5f);
            transform.localScale = Vector3.Lerp(initialScale, targetSquash, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration * 0.5f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / (halfDuration * 0.5f);
            transform.localScale = Vector3.Lerp(targetSquash, targetStretch, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            transform.localScale = Vector3.Lerp(targetStretch, initialScale, t);
            yield return null;
        }

        transform.localScale = initialScale;
        squashCoroutine = null;
    }
}