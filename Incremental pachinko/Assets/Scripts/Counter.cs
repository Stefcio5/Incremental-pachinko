using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    public Text CounterText;

    static int Points = 0;
    [SerializeField]
    private int CountMultiplier;

    private void Start()
    {
        Points = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        Points += 1 * CountMultiplier;
        CounterText.text = "Points : " + Points;
        Destroy(other.gameObject, 2f);
    }
}
