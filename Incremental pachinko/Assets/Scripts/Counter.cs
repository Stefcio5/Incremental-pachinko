using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public Text CounterText;

    static int Count = 0;
    [SerializeField]
    private int CountMultiplier;

    private void Start()
    {
        Count = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Count += 1 * CountMultiplier;
        CounterText.text = "Count : " + Count;
        Destroy(other.gameObject, 2f);
    }
    private IEnumerator DestroyBall(GameObject ball)
    {
        yield return new WaitForSeconds(2f);
        Destroy(ball);
    }
}
