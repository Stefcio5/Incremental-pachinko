using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private int CountMultiplier;
    [SerializeField]
    private DataScriptableObject data;
    [SerializeField]
    private UpgradeScriptableObject upgradeScriptableObject;
    public Text pointsText;  
    public PlayerData playerData;
    

    public Controller controller;

    private void Start()
    {
        //playerData = GameObject.Find("Player Data").GetComponent<PlayerData>();
        //controller = GameObject.Find("Controller").GetComponent<Controller>();
    }
    public void Update()
    {
        //pointsText.text = "Points : " + playerData.points.ToString("F0");
    }

    private void OnTriggerEnter(Collider other)
    {
        //playerData.points += controller.BallPower() * CountMultiplier;
        //pointsText.text = "Points : " + playerData.points;
        
            data.AddPoints((upgradeScriptableObject.level + 1) * CountMultiplier);
            Destroy(other.gameObject, 2f);
    }
}
