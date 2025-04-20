using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using BreakInfinity;

public class Spawner : UpgradeReceiver
{
    public Transform holder;
    private SpawnRange spawnRangeGO;
    private float timer;
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
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnBall(spawnRangeGO.GetUpgradeValue());
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
}
