using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<FlyweightSettings> flyweightSettings;
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private UpgradeScriptableObject autoSpawnBallUpgrade;
    [SerializeField]
    private UpgradeScriptableObject spawnRangeUpgrade;
    private float timer;

    private void Start()
    {
        //InvokeRepeating("SpawnBall", (float)autoSpawnBallUpgrade.UpgradePower, (float)autoSpawnBallUpgrade.UpgradePower);
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
            flyweight.transform.SetParent(transform);
            flyweight.transform.position = new Vector3(0f, 35f, Random.Range((float)-spawnRangeUpgrade.UpgradePower, (float)spawnRangeUpgrade.UpgradePower));
            flyweight.transform.rotation = Quaternion.identity;
        }
    }

    private void SpawnBall()
    {
        Instantiate(ballPrefab, new Vector3(0f, 35f, Random.Range((float)-spawnRangeUpgrade.UpgradePower, (float)spawnRangeUpgrade.UpgradePower)), Quaternion.identity);
    }
}
