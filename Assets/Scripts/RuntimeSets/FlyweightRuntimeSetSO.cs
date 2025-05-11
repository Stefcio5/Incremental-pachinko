using UnityEngine;

[CreateAssetMenu(fileName = "FlyweightRuntimeSet", menuName = "RuntimeSets/Flyweight Runtime Set")]
public class FlyweightRuntimeSetSO : RuntimeSetSO<Flyweight>
{
    public void ReturnAllFlyweightsToPool()
    {
        foreach (var flyweight in items)
        {
            if (flyweight.gameObject.activeSelf)
            {
                FlyweightFactory.ReturnToPool(flyweight);
            }
        }
    }
}