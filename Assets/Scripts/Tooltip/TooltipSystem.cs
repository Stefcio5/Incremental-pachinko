using System.Collections;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    private static TooltipSystem _instance;
    public Tooltip tooltip;

    public void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void Show(string content, string header = "", bool followMouse = true, RectTransform target = null)
    {
        _instance.tooltip.SetText(content, header, followMouse, target);
        _instance.tooltip.gameObject.SetActive(true);
    }
    public static void Hide()
    {
        _instance.tooltip.gameObject.SetActive(false);
    }

    public static void Refresh(string content, string header = "", bool followMouse = true, RectTransform target = null)
    {
        _instance.tooltip.SetText(content, header, followMouse, target);
    }
}
