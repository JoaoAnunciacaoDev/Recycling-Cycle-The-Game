using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [SerializeField] private GameTimer gameTimer;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Trash trashPrefab;
    [SerializeField] private TrashConfig trashConfig;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float initialSpawnInterval = 2f;
    [SerializeField] private float minimumSpawnInterval = 0.5f;
    [SerializeField] private float difficultyIncreaseTime = 30f;
    [SerializeField] private float horizontalPadding = 0.5f;
    [SerializeField] private float initialFallSpeed = 3f;
    [SerializeField] private float maximumFallSpeed = 5f;

    private float timer;

    private void Awake()
    {
        FindCamera();
    }

    private void FindCamera()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindAnyObjectByType<Camera>();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;
        float spawnInterval = GetSpawnInterval();

        if (timer < spawnInterval) return;

        SpawnTrash();
        timer -= spawnInterval;
    }

    private void SpawnTrash()
    {
        FindCamera();
        if (mainCamera == null) return;

        float gameAreaHalfWidth = 2.8125f;
        float halfWidth = Mathf.Min(mainCamera.orthographicSize * mainCamera.aspect, gameAreaHalfWidth);
        float randomX = Random.Range(-halfWidth + horizontalPadding, halfWidth - horizontalPadding);

        Vector3 spawnPosition = new Vector3(
            randomX,
            transform.position.y,
            transform.position.z
        );

        Trash trash = Instantiate(
            trashPrefab,
            spawnPosition,
            Quaternion.identity,
            transform
        );

        TrashType randomType = GetRandomTrashType();
        trash.SetFallSpeed(GetRandomFallSpeed());
        trash.SetTrashType(randomType);

        if (trashConfig != null)
        {
            Sprite trashSprite = trashConfig.GetTrashSpriteForType(randomType);
            Color trashColor = trashConfig.GetColorForType(randomType);
            trash.SetVisuals(trashSprite, trashColor);
        }
    }

    private float GetSpawnInterval()
    {
        float progress = Mathf.Clamp01(
            gameTimer.ElapsedTime / difficultyIncreaseTime
        );

        return Mathf.Lerp(
            initialSpawnInterval,
            minimumSpawnInterval,
            progress
        );
    }

    private float GetRandomFallSpeed()
    {
        float progress = Mathf.Clamp01(
            gameTimer.ElapsedTime / difficultyIncreaseTime
        );

        return Mathf.Lerp(
            initialFallSpeed,
            maximumFallSpeed,
            progress
        );
    }

    private TrashType GetRandomTrashType()
    {
        return (TrashType)Random.Range(
            0,
            System.Enum.GetValues(typeof(TrashType)).Length
        );
    }
}
