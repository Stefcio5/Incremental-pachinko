using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BallFlyweight", menuName = "Flyweight/Ball Flyweight Settings")]
public class BallFlyweightSettings : FlyweightSettings
{
    public BigDouble multiplier = 1;
    public float spawnChance;
    public float spawnChanceincrement;
    public Material material;
    private Ball ball;


    public override Flyweight Create()
    {
        var flyweight = Instantiate(prefab).AddComponent<BallFlyweight>();
        flyweight.settings = this;
        flyweight.gameObject.name = name;
        flyweight.GetComponent<Renderer>().material = material;
        ball = flyweight.GetComponent<Ball>();
        flyweight.gameObject.SetActive(false);
        Debug.Log($"Created {flyweight.name} with multiplier {multiplier}");
        return flyweight;
    }

    public override void OnGet(Flyweight flyweight)
    {
        base.OnGet(flyweight);
        ball.Init(this);
    }
}
