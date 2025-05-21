using UnityEngine;
using BreakInfinity;

public class Spawner : UpgradeReceiver
{
    [SerializeField] private Transform holder;
    private SpawnRange spawnRangeGO;
    private float timer;
    private float manualSpawnTimer;
    [SerializeField] private float spawnInterval;
    private ColorfulBalls colorfulBalls;

    protected override void Awake()
    {
        colorfulBalls = GetComponent<ColorfulBalls>();
    }

    protected override void OnUpgradeInitialized()
    {
        base.OnUpgradeInitialized();
        spawnRangeGO = FindFirstObjectByType<SpawnRange>();

    }

    void Update()
    {
        if (UpgradeManager.Instance.Initialized)
        {
            timer += Time.deltaTime;
            if (timer >= GetUpgradeValue())
            {
                SpawnBall(spawnRangeGO.GetUpgradeValue());
                timer = 0f;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                manualSpawnTimer += Time.deltaTime;
                if (manualSpawnTimer >= spawnInterval)
                {
                    SpawnBall(spawnRangeGO.GetUpgradeValue());
                    manualSpawnTimer = 0f;
                }
            }
        }
    }

    private void SpawnBall(BigDouble position)
    {
        BallFlyweightSettings ballFlyweightSettings = colorfulBalls.GetRandomBallFlyweightSettings();
        var flyweight = FlyweightFactory.Spawn(ballFlyweightSettings);
        flyweight.Init();
        flyweight.transform.position = new Vector3(0f, 35f, Random.Range((float)-position, (float)position));
        flyweight.transform.rotation = Quaternion.identity;
        flyweight.transform.SetParent(holder);
    }
}
