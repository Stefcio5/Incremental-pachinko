using UnityEngine;

[CreateAssetMenu(fileName = "FlyweightSettings", menuName = "Flyweight/Flyweight Settings")]
public class FlyweightSettings : ScriptableObject
{
    public FlyweightType type;
    public GameObject prefab;
    public float despawnTime = 2f;


    public virtual Flyweight Create()
    {
        var flyweight = Instantiate(prefab).AddComponent<Flyweight>();
        flyweight.settings = this;
        flyweight.gameObject.SetActive(false);
        flyweight.gameObject.name = prefab.name;
        return flyweight;
    }

    public virtual void OnGet(Flyweight flyweight)
    {
        flyweight.gameObject.SetActive(true);
    }
    public virtual void OnRelease(Flyweight flyweight)
    {
        flyweight.gameObject.SetActive(false);
    }
    public virtual void OnDestroyPoolObject(Flyweight flyweight)
    {
        Destroy(flyweight.gameObject);
    }
}
