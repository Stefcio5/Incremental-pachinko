using UnityEngine;

public class Spawner : MonoBehaviour
{
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
            SpawnBall();
            timer = 0f;
        }
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
