using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BreakInfinity;

public class Spawner : UpgradeReceiver
{
    public List<FlyweightSettings> flyweightSettings;
    private SpawnRange spawnRangeGO;
    private float timer;

    protected override void Start()
    {
        base.Start();
        //TODO: Fix after bootstrap scene
        // if (UpgradeManager.Instance != null)
        // {
        //     //InvokeRepeating("SpawnBall", (float)autoSpawnBallUpgrade.UpgradePower, (float)autoSpawnBallUpgrade.UpgradePower);
        //     _spawnRangeUpgrade = UpgradeManager.Instance.GetUpgrade(spawnRangeConfig.upgradeName);

        //     if (_spawnRangeUpgrade != null)
        //     {
        //         // Subscribe to the OnLevelChanged event.
        //         _spawnRangeUpgrade.OnLevelChanged += OnSpawnRangeUpgradeLevelChanged;
        //         // Initialize the cached value.
        //         cachedSpawnRange = _spawnRangeUpgrade.CurrentPower;
        //     }
        //     else
        //     {
        //         Debug.LogWarning("Spawn range upgrade not found!");
        //     }
        // }
        spawnRangeGO = FindFirstObjectByType<SpawnRange>();
    }

    void Update()
    {
        if (UpgradeManager.Instance.Initialized)
        {
            timer += Time.deltaTime;
            if (timer >= Value)
            {
                SpawnBall(spawnRangeGO.GetValue());
                timer = 0f;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnBall(spawnRangeGO.GetValue());
            }
        }
    }

    private void SpawnBall(BigDouble position)
    {
        var flyweight = FlyweightFactory.Spawn(flyweightSettings[0]);
        flyweight.transform.position = new Vector3(0f, 35f, Random.Range((float)-position, (float)position));
        flyweight.transform.rotation = Quaternion.identity;
        // set parent to the spawner
        flyweight.transform.SetParent(transform);
    }
}
