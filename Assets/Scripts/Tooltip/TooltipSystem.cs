using UnityEditor.Search;
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

    public static void Show(string content, string header = "")
    {
        _instance.tooltip.SetText(content, header);
        _instance.tooltip.gameObject.SetActive(true);
    }
    public static void Hide()
    {
        _instance.tooltip.gameObject.SetActive(false);
    }

    public static void Refresh(string content, string header = "")
    {
        _instance.tooltip.SetText(content, header);
    }
}
