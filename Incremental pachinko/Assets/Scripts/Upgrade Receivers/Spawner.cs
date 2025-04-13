using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BreakInfinity;

public class Spawner : UpgradeReceiver
{
    public List<BallFlyweightSettings> ballFlyweightSettings;
    public Transform holder;
    private SpawnRange spawnRangeGO;
    private float timer;
    private ColorfulBalls colorfulBalls;

    private void Awake()
    {
        colorfulBalls = GetComponent<ColorfulBalls>();
    }

    protected override void Start()
    {
        base.Start();
        spawnRangeGO = FindFirstObjectByType<SpawnRange>();
    }

    void Update()
    {
        if (UpgradeManager.Instance.Initialized)
        {
            timer += Time.deltaTime;
            if (timer >= Value)
            {
                SpawnBall(spawnRangeGO.GetValue());
                timer = 0f;
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnBall(spawnRangeGO.GetValue());
            }
        }
    }

    private void SpawnBall(BigDouble position)
    {
        BallFlyweightSettings ballFlyweightSettings = colorfulBalls.GetRandomBallFlyweightSettings();
        var flyweight = FlyweightFactory.Spawn(ballFlyweightSettings);
        flyweight.transform.position = new Vector3(0f, 35f, Random.Range((float)-position, (float)position));
        flyweight.transform.rotation = Quaternion.identity;
        // set parent to the spawner
        flyweight.transform.SetParent(holder);
    }

    private BallFlyweightSettings GetRandomBall()
    {
        // Get a random index from the list of ball flyweight settings
        int randomIndex = Random.Range(0, ballFlyweightSettings.Count);
        // Return the ball flyweight settings at that index
        return ballFlyweightSettings[randomIndex];
    }
}
