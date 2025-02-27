using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FlyweightFactory : PersistentSingleton<FlyweightFactory>
{
    [SerializeField] bool collectionCheck = true;
    [SerializeField] int defaultCapacity = 100;
    [SerializeField] int maxPoolSize = 1000;
    readonly Dictionary<FlyweightType, IObjectPool<Flyweight>> _pools = new();

    protected override void Awake()
    {
        base.Awake();
    }

    public static Flyweight Spawn(FlyweightSettings settings) => Instance.GetPoolFor(settings)?.Get();
    public static void ReturnToPool(Flyweight flyweight) => Instance.GetPoolFor(flyweight.settings)?.Release(flyweight);

    IObjectPool<Flyweight> GetPoolFor(FlyweightSettings settings)
    {
        IObjectPool<Flyweight> pool;

        if (_pools.TryGetValue(settings.type, out pool)) return pool;

        pool = new ObjectPool<Flyweight>(
            settings.Create,
            settings.OnGet,
            settings.OnRelease,
            settings.OnDestroyPoolObject,
            collectionCheck,
            defaultCapacity,
            maxPoolSize
        );
        _pools.Add(settings.type, pool);
        return pool;
    }
}
