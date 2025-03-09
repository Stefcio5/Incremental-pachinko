using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BreakInfinity;
using UnityEngine;

public class Ball : UpgradeReceiver
{
    private BigDouble ballValue;

    protected override void Start()
    {
        base.Start();
        Debug.Log($"Ball value: {ballValue}");
    }

    protected override void HandlePowerChanged()
    {
        ballValue = upgrade.CurrentPower;
    }

    public override BigDouble GetCurrentValue() => ballValue;

}
