using System.Collections;
using UnityEngine;

public class Flyweight : MonoBehaviour
{
    public FlyweightSettings settings;

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
