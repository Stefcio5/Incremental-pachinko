using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
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
        // Wait for UpgradeManager and DataController to be ready
        yield return new WaitUntil(() => UpgradeManager.Instance != null && DataController.Instance != null);

        // Wait one more frame to ensure everything is initialized
        yield return null;

        UpgradeManager.Instance.OnUpgradesChanged += UpdateUI;
        _initialized = true;
        UpdateUI();

        Debug.Log($"UpgradeSystemUI initialized for type: {_upgradeType}");
    }

    private void OnDestroy()
    {
        if (UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnUpgradesChanged -= UpdateUI;
        }
    }

    private void UpdateUI()
    {
        if (!_initialized)
        {
            Debug.LogWarning("Trying to update UI before initialization");
            return;
        }

        var upgrades = UpgradeManager.Instance.GetUpgrades(_upgradeType);

        Debug.Log($"Updating UI for {_upgradeType}, found {upgrades.Count()} upgrades");

        // Deactivate all existing UpgradeUIs
        foreach (var upgradeUI in _upgradeUIs)
        {
            if (upgradeUI != null)
                upgradeUI.gameObject.SetActive(false);
        }

        // Clean up null references
        _upgradeUIs.RemoveAll(ui => ui == null);

        // Activate and update only the required UpgradeUIs
        int i = 0;
        foreach (var upgrade in upgrades)
        {
            if (i < _upgradeUIs.Count)
            {
                _upgradeUIs[i].gameObject.SetActive(true);
                _upgradeUIs[i].SetUpgrade(upgrade);
            }
            else
            {
                var upgradeUIObject = Instantiate(_upgradeUIPrefab, _upgradeListContainer);
                var upgradeUI = upgradeUIObject.GetComponent<UpgradeUI>();
                if (upgradeUI != null)
                {
                    upgradeUI.SetUpgrade(upgrade);
                    _upgradeUIs.Add(upgradeUI);
                }
                else
                {
                    Debug.LogError($"UpgradeUI component not found on prefab");
                }
            }
            i++;
        }
    }
}
