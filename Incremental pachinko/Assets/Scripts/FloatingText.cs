using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private Flyweight flyweight;

    void Awake()
    {
        flyweight = GetComponent<Flyweight>();
    }

    void OnEnable()
    {
        flyweight.Despawn();
    }
}
