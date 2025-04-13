using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string header;
    public string content;
    private bool isMouseOver;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
        TooltipSystem.Show(content, header);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
        TooltipSystem.Hide();
    }

    private void Update()
    {
        if (isMouseOver)
        {
            TooltipSystem.Refresh(content, header);
        }
    }
}
