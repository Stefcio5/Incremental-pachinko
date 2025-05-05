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

    public static Flyweight Spawn(FlyweightSettings settings)
    {
        var flyweight = Instance.GetPoolFor(settings)?.Get();
        flyweight.settings = settings;
        return flyweight;
    }

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
    public static void ClearAllPools()
    {
        foreach (var pool in Instance._pools.Values)
        {
            pool.Clear();
        }
        Instance._pools.Clear();
    }

    public static void ClearPool(FlyweightType type)
    {
        if (Instance._pools.TryGetValue(type, out var pool))
        {
            pool.Clear();
            Instance._pools.Remove(type);
        }
    }
}
