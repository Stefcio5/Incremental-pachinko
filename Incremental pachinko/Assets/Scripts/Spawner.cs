using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private UpgradeScriptableObject autoSpawnBallUpgrade;
    [SerializeField]
    private UpgradeScriptableObject spawnRangeUpgrade;

    private void Start()
    {
        InvokeRepeating("SpawnBall", (float)autoSpawnBallUpgrade.UpgradePower, (float)autoSpawnBallUpgrade.UpgradePower);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnBall();
        }
    }

    private void SpawnBall()
    {
        Instantiate(ballPrefab, new Vector3(0f, 35f, Random.Range((float)-spawnRangeUpgrade.UpgradePower, (float)spawnRangeUpgrade.UpgradePower)), Quaternion.identity);
    }
}
