using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    [Header("Configurações do Shake")]
    [SerializeField] private float shakeFade = 9.0f;
    [SerializeField] private float defaultStrength = 0.35f;

    private Vector3 initialPosition;
    private float shakeStrength = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        initialPosition = transform.localPosition;
    }

    public void Shake(float initialStrength)
    {
        shakeStrength = Mathf.Max(shakeStrength, initialStrength);
    }

    public void Shake()
    {
        Shake(defaultStrength);
    }

    private void LateUpdate()
    {
        if (shakeStrength > 0.02f)
        {
            shakeStrength = Mathf.Lerp(shakeStrength, 0f, shakeFade * Time.unscaledDeltaTime);

            Vector2 randomOffset = RandomOffset();
            transform.localPosition = initialPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);
        }
        else if (transform.localPosition != initialPosition)
        {
            transform.localPosition = initialPosition;
            shakeStrength = 0f;
        }
    }

    private Vector2 RandomOffset()
    {
        return new Vector2(
            Random.Range(-shakeStrength, shakeStrength),
            Random.Range(-shakeStrength, shakeStrength)
        );
    }
}
