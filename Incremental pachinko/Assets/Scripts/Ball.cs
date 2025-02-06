using System.Linq;
using BreakInfinity;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private UpgradeConfig ballUpgrade;
    private UpgradeManager _upgrades;

    private void Start() => _upgrades = FindFirstObjectByType<UpgradeManager>();

    public BigDouble GetValue() =>
        _upgrades.GetUpgrades(UpgradeType.Basic)
            .First(u => u.config == ballUpgrade)
            .CurrentPower;
}
