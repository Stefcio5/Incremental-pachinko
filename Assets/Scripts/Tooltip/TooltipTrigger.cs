using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    [Multiline]
    public string content;

    public bool followMouse = true;
    public RectTransform targetOverride;
    private bool _isMouseOver;

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isMouseOver = true;
        TooltipSystem.Show(content, header, followMouse, targetOverride);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isMouseOver = false;
        TooltipSystem.Hide();
    }

    public void UpdateText()
    {
        if (_isMouseOver && isActiveAndEnabled)
        {
            TooltipSystem.Refresh(content, header, followMouse, targetOverride);
        }
    }
}
