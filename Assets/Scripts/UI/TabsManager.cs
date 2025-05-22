using UnityEngine;
using UnityEngine.UI;

public class TabsManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _tabs;
    [SerializeField] private Image[] _tabButtons;
    [SerializeField] private Sprite _inactiveTabBG, _activeTabBG;
    [SerializeField] private Vector2 _inactiveTabSize, _activeTabSize;

    public void SwitchToTab(int tabID)
    {
        for (int i = 0; i < _tabs.Length; i++)
        {
            if (i == tabID)
            {
                _tabs[i].SetActive(true);
                _tabButtons[i].sprite = _activeTabBG;
            }
            else
            {
                _tabs[i].SetActive(false);
                _tabButtons[i].sprite = _inactiveTabBG;
            }
        }
    }
}
