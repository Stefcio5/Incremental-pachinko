using UnityEngine;

[CreateAssetMenu(fileName = "TooltipText", menuName = "Scriptable Objects/TooltipText")]
public class TooltipText : ScriptableObject
{
    public string tooltipText;

    public void SetTooltipText(string text)
    {
        tooltipText = text;
    }
}
