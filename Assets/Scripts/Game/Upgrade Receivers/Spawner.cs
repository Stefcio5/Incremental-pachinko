using UnityEngine;
using BreakInfinity;
using DG.Tweening;

public class Spawner : UpgradeReceiver
{
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private BallSpawnCounterSO _ballSpawnCounter;
    [SerializeField] private Transform _holder;
    [SerializeField] private SpawnRange _spawnRangeGO;
    [SerializeField] private float _spawnInterval = 0.1f;

    private ColorfulBalls _colorfulBalls;
    private float _timer;
    private float _manualSpawnTimer;
    private bool _isManualSpawning;
    private bool _isReady;

    protected override void Awake()
    {
        base.Awake();
        _colorfulBalls = GetComponent<ColorfulBalls>();

        if (_colorfulBalls is null)
        {
            Debug.LogWarning($"[{nameof(Spawner)}] {nameof(ColorfulBalls)} component not found.");
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (_inputReader is not null)
        {
            _inputReader.SpawnBallEvent += HandleSpawnBallInput;
            _inputReader.SpawnBallCancelEvent += HandleSpawnBallCancel;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (_inputReader is not null)
        {
            _inputReader.SpawnBallEvent -= HandleSpawnBallInput;
            _inputReader.SpawnBallCancelEvent -= HandleSpawnBallCancel;
        }
    }

    protected override void OnUpgradeInitialized()
    {
        base.OnUpgradeInitialized();
        _isReady = true;
    }

    private void Update()
    {
        if (!_isReady || _colorfulBalls is null)
        {
            return;
        }

        // Automatic spawn timer
        _timer += Time.deltaTime;
        if (_timer >= GetUpgradeValue() && _ballSpawnCounter.Add())
        {
            SpawnBall(_spawnRangeGO.GetUpgradeValue());
            _timer = 0f;
        }

        // Manual spawn timer (on hold)
        if (_isManualSpawning)
        {
            _manualSpawnTimer += Time.deltaTime;
            if (_manualSpawnTimer >= _spawnInterval && _ballSpawnCounter.Add())
            {
                SpawnBall(_spawnRangeGO.GetUpgradeValue());
                _manualSpawnTimer = 0f;
            }
        }
    }

    private void HandleSpawnBallInput()
    {
        _isManualSpawning = true;
        _manualSpawnTimer = 0f;
    }

    private void HandleSpawnBallCancel()
    {
        _isManualSpawning = false;
        _manualSpawnTimer = 0f;
    }

    private void SpawnBall(BigDouble position)
    {
        BallFlyweightSettings ballSettings = _colorfulBalls.GetRandomBallFlyweightSettings();

        _spawnRangeGO.ChangeSpawnRangeColor(ballSettings.color);
        _spawnRangeGO.DoPunchScale(ballSettings.ID);

        var flyweight = FlyweightFactory.Spawn(ballSettings);
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
