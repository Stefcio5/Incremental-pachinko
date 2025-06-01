using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    private bool _isMouseOver;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _isMouseOver = true;
        TooltipSystem.Show(content, header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isMouseOver = false;
        TooltipSystem.Hide();
    }

    private void Update()
    {
        if (_isMouseOver)
        {
            TooltipSystem.Refresh(content, header);
        }
    }
}
