using UnityEngine;

public class SpawnRange : UpgradeReceiver
{
    private GameObject _spawnRangeObject;
    protected override void Start()
    {
        base.Start();
        _spawnRangeObject = gameObject;
        SetObjectScale();
        OnSpawnRangeUpgradeLevelChanged();
        Debug.Log(_spawnRangeObject.name);
        Debug.Log(_spawnRangeObject.transform.localScale);
    }

    private void OnSpawnRangeUpgradeLevelChanged()
    {
        upgradePower.onValueChanged += SetObjectScale;
    }
    private void SetObjectScale()
    {
        var scale = upgradePower.FinalValue * 2;
        _spawnRangeObject.transform.localScale = new Vector3(_spawnRangeObject.transform.localScale.x, _spawnRangeObject.transform.localScale.y, (float)scale);
    }
}
