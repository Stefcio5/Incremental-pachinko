using UnityEngine;

[CreateAssetMenu(fileName = "FlyweightSettings", menuName = "Flyweight/Flyweight Settings")]
public class FlyweightSettings : ScriptableObject
{
    public FlyweightType type;
    public GameObject prefab;
    public float despawnTime = 2f;


    public Flyweight Create()
    {
        var flyweight = Instantiate(prefab).AddComponent<Flyweight>();
        flyweight.settings = this;
        flyweight.gameObject.SetActive(false);
        flyweight.gameObject.name = prefab.name;
        return flyweight;
    }

    public void OnGet(Flyweight flyweight)
    {
        flyweight.gameObject.SetActive(true);
    }
    public void OnRelease(Flyweight flyweight)
    {
        flyweight.gameObject.SetActive(false);
    }
    public void OnDestroyPoolObject(Flyweight flyweight)
    {
        Destroy(flyweight.gameObject);
    }
}

public enum FlyweightType
{
    Ball,
    FloatingUI
}
