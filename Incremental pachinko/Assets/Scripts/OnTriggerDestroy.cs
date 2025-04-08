using UnityEngine;

public class OnTriggerDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Flyweight>(out var flyweight))
        {
            flyweight.Despawn();
        }
    }
}
