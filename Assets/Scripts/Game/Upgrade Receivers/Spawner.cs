using UnityEngine;
using BreakInfinity;
using DG.Tweening;

public class Spawner : UpgradeReceiver
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private BallSpawnCounterSO _ballSpawnCounter;
    [SerializeField] private Transform _holder;
    [SerializeField] private SpawnRange _spawnRangeGO;
    private float _timer;
    private float _manualSpawnTimer;
    private bool _isManualSpawning;
    [SerializeField] private float _spawnInterval;
    private ColorfulBalls _colorfulBalls;

    protected override void Awake()
    {
        _colorfulBalls = GetComponent<ColorfulBalls>();
    }

    private void OnEnable()
    {
        _inputReader.SpawnBallEvent += OnSpawnBall;
        _inputReader.SpawnBallCancelEvent += OnSpawnBallCancel;
    }

    private void OnDisable()
    {
        _inputReader.SpawnBallEvent -= OnSpawnBall;
        _inputReader.SpawnBallCancelEvent -= OnSpawnBallCancel;
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
            if (_timer >= GetUpgradeValue() && _ballSpawnCounter.Add())
            {
                SpawnBall(_spawnRangeGO.GetUpgradeValue());
                _timer = 0f;
            }

            if (_isManualSpawning)
            {
                _manualSpawnTimer += Time.deltaTime;
                if (_manualSpawnTimer >= _spawnInterval && _ballSpawnCounter.Add())
                {
                    SpawnBall(_spawnRangeGO.GetUpgradeValue());
                    _manualSpawnTimer = 0f;
                }
            }

            // Obsolete
            // if (Input.GetKey(KeyCode.Space))
            // {
            //     _manualSpawnTimer += Time.deltaTime;
            //     if (_manualSpawnTimer >= _spawnInterval)
            //     {
            //         SpawnBall(_spawnRangeGO.GetUpgradeValue());
            //         _manualSpawnTimer = 0f;
            //     }
            // }
        }
    }

    private void OnSpawnBall()
    {
        _isManualSpawning = true;
        _manualSpawnTimer = 0f;
    }

    private void OnSpawnBallCancel()
    {
        _isManualSpawning = false;
        _manualSpawnTimer = 0f;
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

    public override BigDouble GetUpgradeValue()
    {
        return upgradePower.FinalValue;
    }
}
