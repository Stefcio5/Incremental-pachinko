using System.Collections;
using Mono.Cecil;
using UnityEditor.VersionControl;
using UnityEngine;

public class Flyweight : MonoBehaviour
{
    public FlyweightSettings settings;
    private FlyweightRuntimeSetSO runtimeSet;


    protected void Start()
    {
        runtimeSet = Resources.Load<FlyweightRuntimeSetSO>("FlyweightRuntimeSet");
        if (runtimeSet != null)
        {
            runtimeSet.Add(this);
        }
    }
    public void Despawn()
    {
        StartCoroutine(Despawn(settings.despawnTime));
    }

    IEnumerator Despawn(float delay)
    {
        yield return Helpers.GetWaitForSeconds(delay);
        FlyweightFactory.ReturnToPool(this);
    }
}
