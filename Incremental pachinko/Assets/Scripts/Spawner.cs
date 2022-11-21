using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject ballPrefab;
    [SerializeField]
    private UpgradeScriptableObject autoSpawnBallUpgrade;

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
        Instantiate(ballPrefab, new Vector3(0f, 35f, Random.Range(-1f, 1f)), Quaternion.identity);
    }
}
