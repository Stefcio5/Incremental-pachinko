using UnityEngine;
using BreakInfinity;

public class Spawner : UpgradeReceiver
{
    [SerializeField] private Transform _holder;
    private SpawnRange _spawnRangeGO;
    private float _timer;
    private float _manualSpawnTimer;
    [SerializeField] private float _spawnInterval;
    private ColorfulBalls _colorfulBalls;

    protected override void Awake()
    {
        _colorfulBalls = GetComponent<ColorfulBalls>();
    }

    protected override void OnUpgradeInitialized()
    {
        base.OnUpgradeInitialized();
        _spawnRangeGO = FindFirstObjectByType<SpawnRange>();

    }

    void Update()
    {
        if (UpgradeManager.Instance.Initialized)
        {
            _timer += Time.deltaTime;
            if (_timer >= GetUpgradeValue())
            {
                SpawnBall(_spawnRangeGO.GetUpgradeValue());
                _timer = 0f;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                _manualSpawnTimer += Time.deltaTime;
                if (_manualSpawnTimer >= _spawnInterval)
                {
                    SpawnBall(_spawnRangeGO.GetUpgradeValue());
                    _manualSpawnTimer = 0f;
                }
            }
        }
    }

    private void SpawnBall(BigDouble position)
    {
        BallFlyweightSettings ballFlyweightSettings = _colorfulBalls.GetRandomBallFlyweightSettings();
        var flyweight = FlyweightFactory.Spawn(ballFlyweightSettings);
        flyweight.Init();
        flyweight.transform.position = new Vector3(0f, 35f, Random.Range((float)-position, (float)position));
        flyweight.transform.rotation = Quaternion.identity;
        flyweight.transform.SetParent(_holder);
    }
}
