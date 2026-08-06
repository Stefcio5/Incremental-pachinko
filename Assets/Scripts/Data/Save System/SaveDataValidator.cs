using System;
using System.Collections.Generic;
using BreakInfinity;
using UnityEngine;

public class SaveDataValidator
{
    private const string SaveKey = "GameData";

    public void ValidateSave(string expectedData)
    {
        if (PlayerPrefs.GetString(SaveKey) != expectedData)
        {
            throw new Exception("Save validation failed - data mismatch");
        }
    }

    public void ValidateLoadedData(GameData data)
    {
        if (data.points < 0) data.points = 0;
        if (data.totalPoints < 0) data.totalPoints = 0;
        if (data.prestigePoints < 0) data.prestigePoints = 0;
        data.upgradeLevels ??= new Dictionary<string, BigDouble>();
    }
}
