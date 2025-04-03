using UnityEngine;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] tabs;
    [SerializeField]
    private Image[] tabButtons;
    [SerializeField]
    private Sprite inactiveTabBG, activeTabBG;
    [SerializeField]
    private Vector2 inactiveTabSize, activeTabSize;

    public void SwitchToTab(int tabID)
    {
        for (int i = 0; i < tabs.Length; i++)
        {
            if (i == tabID)
            {
                tabs[i].SetActive(true);
                tabButtons[i].sprite = activeTabBG;
            }
            else
            {
                tabs[i].SetActive(false);
                tabButtons[i].sprite = inactiveTabBG;
            }
        }
    }
}
