using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    private float destroyTime = 0.5f;

    void Start()
    {
        Destroy(gameObject, destroyTime);
    }
}
