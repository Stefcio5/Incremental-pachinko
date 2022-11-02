using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private Text ballPowerText;
    public PlayerData playerData;


    public double pointsPerBall;
    // Start is called before the first frame update
    void Start()
    {
        playerData = GameObject.Find("Player Data").GetComponent<PlayerData>();
        playerData.points = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //ballPowerText.text = ($"Ball power: x{BallPower()}");
    }
    public double BallPower() => 1 + playerData.ballUpgradeLevel;
}
