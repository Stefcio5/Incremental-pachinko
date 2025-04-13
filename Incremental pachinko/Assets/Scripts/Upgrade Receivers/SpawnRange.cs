using System;
using BreakInfinity;
using UnityEngine;

public class SpawnRange : UpgradeReceiver
{
    private GameObject spawnRangeObject;
    protected override void Start()
    {
        base.Start();
        spawnRangeObject = gameObject;
        SetObjectScale();
        OnSpawnRangeUpgradeLevelChanged();
        Debug.Log(spawnRangeObject.name);
        Debug.Log(spawnRangeObject.transform.localScale);
    }

    private void OnSpawnRangeUpgradeLevelChanged()
    {
        upgrade.OnLevelChanged += (u) => SetObjectScale();
    }
    private void SetObjectScale()
    {
        var scale = Value * 2;
        //set the scale of the GameObject to the new scale on z axis
        spawnRangeObject.transform.localScale = new Vector3(spawnRangeObject.transform.localScale.x, spawnRangeObject.transform.localScale.y, (float)scale);
    }
}
