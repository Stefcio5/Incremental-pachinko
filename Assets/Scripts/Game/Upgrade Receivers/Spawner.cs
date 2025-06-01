using UnityEngine;
using BreakInfinity;
using DG.Tweening;

public class Spawner : UpgradeReceiver
{
    [SerializeField] private Transform _holder;
    [SerializeField] private SpawnRange _spawnRangeGO;
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

        _spawnRangeGO.ChangeSpawnRangeColor(ballFlyweightSettings.color);
        _spawnRangeGO.DoPunchScale(ballFlyweightSettings.ID);

        var flyweight = FlyweightFactory.Spawn(ballFlyweightSettings);
        flyweight.gameObject.transform.position = new Vector3(0, 35.5f, Random.Range((float)-position, (float)position));
        flyweight.transform.SetParent(_holder);
        flyweight.transform.rotation = Quaternion.identity;
        flyweight.Init();
    }

}
