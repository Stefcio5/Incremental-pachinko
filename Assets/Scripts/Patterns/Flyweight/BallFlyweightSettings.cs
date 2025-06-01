using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "BallFlyweight", menuName = "Flyweight/Ball Flyweight Settings")]
public class BallFlyweightSettings : FlyweightSettings
{
    public BigDouble multiplier = 1;
    public float spawnChance;
    public float spawnChanceincrement;
    public Material material;
    public Color color;
    public int ID;


    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        go.SetActive(false);

        var flyweight = go.AddComponent<BallFlyweight>();
        flyweight.settings = this;
        return flyweight;
    }

    public override void OnGet(Flyweight flyweight)
    {
        base.OnGet(flyweight);
    }
}
