using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private string hoverSound = "btn_hover";
    [SerializeField] private string clickSound = "click";

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(hoverSound))
        {
            SFXManager.Instance?.PlaySFX(hoverSound);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!string.IsNullOrEmpty(clickSound))
        {
            SFXManager.Instance?.PlaySFX(clickSound);
        }
    }
}
