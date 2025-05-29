using System.Collections;
using UnityEngine;

public class Flyweight : MonoBehaviour
{
    public FlyweightSettings settings;
    private FlyweightRuntimeSetSO runtimeSet;


    protected virtual void Start()
    {
        runtimeSet = Resources.Load<FlyweightRuntimeSetSO>("FlyweightRuntimeSet");
        if (runtimeSet != null)
        {
            runtimeSet.Add(this);
        }
    }

    public virtual void Init()
    {

    }
    public virtual void Despawn()
    {
        StartCoroutine(DespawnCoroutine(settings.despawnTime));
    }

    private IEnumerator DespawnCoroutine(float delay)
    {
        yield return Helpers.GetWaitForSeconds(delay);
        FlyweightFactory.ReturnToPool(this);
    }
}
