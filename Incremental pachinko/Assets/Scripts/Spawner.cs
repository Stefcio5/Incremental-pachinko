using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BreakInfinity;

public class Spawner : MonoBehaviour
{
    public List<FlyweightSettings> flyweightSettings;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private UpgradeScriptableObject autoSpawnBallUpgrade;
    [SerializeField]
    private UpgradeScriptableObject spawnRangeUpgrade;
    [SerializeField]
    private UpgradeConfig spawnRangeConfig;
    private float timer;

    private void Start()
    {
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
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= autoSpawnBallUpgrade.UpgradePower)
        {
            //SpawnBall();
            timer = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //SpawnBall();
            var flyweight = FlyweightFactory.Spawn(flyweightSettings[0]);
            var spawnRange = UpgradeManager.Instance.GetUpgrade(spawnRangeConfig.upgradeName).CurrentPower;
            flyweight.transform.position = new Vector3(0f, 35f, Random.Range((float)-spawnRange, (float)spawnRange));
            flyweight.transform.rotation = Quaternion.identity;
        }
    }

    private void SpawnBall()
    {
        Instantiate(ballPrefab, new Vector3(0f, 35f, Random.Range((float)-spawnRangeUpgrade.UpgradePower, (float)spawnRangeUpgrade.UpgradePower)), Quaternion.identity);
    }
}
