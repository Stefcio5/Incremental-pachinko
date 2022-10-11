using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Counter : MonoBehaviour
{
    public Text pointsText;  
    public PlayerData playerData;
    
    [SerializeField]
    private int CountMultiplier;

    public Controller controller;

    private void Start()
    {
        playerData = GameObject.Find("Player Data").GetComponent<PlayerData>();
        controller = GameObject.Find("Controller").GetComponent<Controller>();
    }
    public void Update()
    {
        pointsText.text = "Points : " + playerData.points.ToString("F0");
    }

    private void OnTriggerEnter(Collider other)
    {
        playerData.points += controller.BallPower() * CountMultiplier;
        //pointsText.text = "Points : " + playerData.points;
        Destroy(other.gameObject, 2f);
    }
}
