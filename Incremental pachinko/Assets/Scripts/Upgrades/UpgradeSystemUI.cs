using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class UpgradeSystemUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _upgradeListContainer;
    [SerializeField] private GameObject _upgradeUIPrefab;
    [SerializeField] private UpgradeType _upgradeType;

    private List<UpgradeUI> _upgradeUIs = new();
    private bool _initialized;

    private void Awake()
    {
        StartCoroutine(InitializeWhenReady());
    }

    private IEnumerator InitializeWhenReady()
    {
        // Wait for UpgradeManager and DataController to be ready.
        yield return new WaitUntil(() => UpgradeManager.Instance != null && DataController.Instance != null);
        // Wait another frame to ensure everything is fully initialized.
        yield return null;

        //UpgradeManager.Instance.OnUpgradesChanged += UpdateUI;
        _initialized = true;
        UpdateUI();

        Debug.Log($"UpgradeSystemUI initialized for type: {_upgradeType}");
    }

    private void OnDestroy()
    {
        if (UpgradeManager.Instance != null)
        {
            //UpgradeManager.Instance.OnUpgradesChanged -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (!_initialized)
        {
            Debug.LogWarning("Trying to update UI before initialization");
            return;
        }

        // Get the upgrades once and cache the count.
        var upgrades = UpgradeManager.Instance.GetUpgrades(_upgradeType).ToList();
        int upgradeCount = upgrades.Count;

        Debug.Log($"Updating UI for {_upgradeType}, found {upgradeCount} upgrades");

        // Ensure we have exactly as many UpgradeUI objects as upgrades.
        // If there are fewer UI elements than upgrades, instantiate the missing ones.
        for (int i = 0; i < upgradeCount; i++)
        {
            UpgradeUI ui;

            if (i < _upgradeUIs.Count)
            {
                ui = _upgradeUIs[i];
            }
            else
            {
                // Instantiate a new UI element and add it to the list.
                var uiObject = Instantiate(_upgradeUIPrefab, _upgradeListContainer);
                ui = uiObject.GetComponent<UpgradeUI>();
                if (ui == null)
                {
                    Debug.LogError("UpgradeUI component not found on prefab");
                    continue;
                }
                _upgradeUIs.Add(ui);
            }
            // Activate and update the UI.
            if (ui != null)
            {
                ui.gameObject.SetActive(true);
                ui.SetUpgrade(upgrades[i]);
            }
        }

        // Deactivate any extra UpgradeUIs not needed.
        for (int i = upgradeCount; i < _upgradeUIs.Count; i++)
        {
            if (_upgradeUIs[i] != null)
            {
                _upgradeUIs[i].gameObject.SetActive(false);
            }
        }
    }
}
