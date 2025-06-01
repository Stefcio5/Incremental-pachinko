using UnityEngine;

[CreateAssetMenu(fileName = "FloatingTextFlyweight", menuName = "Flyweight/Floating Text Flyweight Settings")]
public class FloatingTextFlyweightSettings : FlyweightSettings
{
    public override Flyweight Create()
    {
        var go = Instantiate(prefab);
        go.name = prefab.name;
        go.SetActive(false);

        var flyweight = go.GetComponent<FloatingTextFlyweight>();
        flyweight.settings = this;
        return flyweight;
    }
}