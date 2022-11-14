using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private DataScriptableObject data;
    [SerializeField]
    private UpgradeScriptableObject upgradeScriptableObject;
    [SerializeField]
    private UpgradeScriptableObject boxUpgradeScriptableObject;

    private void OnTriggerEnter(Collider other)
    {
        //playerData.points += controller.BallPower() * CountMultiplier;
        //pointsText.text = "Points : " + playerData.points;
        
            data.AddPoints((upgradeScriptableObject.upgradeLevel + 1) * boxUpgradeScriptableObject.upgradePower);
            Destroy(other.gameObject, 2f);
    }
}
