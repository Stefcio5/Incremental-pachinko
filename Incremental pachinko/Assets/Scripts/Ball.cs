using System.Linq;
using BreakInfinity;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] private UpgradeConfig ballUpgrade;

    public BigDouble GetValue() =>
        UpgradeManager.Instance.GetUpgrade(ballUpgrade.upgradeName).CurrentPower;
}
