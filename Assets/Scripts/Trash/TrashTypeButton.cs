using TMPro;
using UnityEngine;

public class TrashTypeButton : MonoBehaviour
{
    [SerializeField] private PlayerBinController playerBinController;
    [SerializeField] private TrashType type;
    [SerializeField] private GameObject shortcutText;

    private void Start()
    {
        if (IsMobile())
        {
            if (shortcutText != null)
            {
                shortcutText.SetActive(false);
            }
            else
            {
                TextMeshProUGUI tmpText = GetComponentInChildren<TextMeshProUGUI>(true);
                if (tmpText != null)
                {
                    tmpText.gameObject.SetActive(false);
                }
            }
        }
    }

    private bool IsMobile()
    {
#if UNITY_ANDROID || UNITY_IOS
        return true;
#else
        return Application.isMobilePlatform;
#endif
    }

    public void SelectType()
    {
        playerBinController.SetTrashType(type);
    }
}
