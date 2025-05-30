using System.Collections.Generic;
using System.Text;
using BreakInfinity;
using UnityEngine;

public class PlayerPrefsDataRepository : IDataRepository
{
    private const string SaveKey = "GameData";
    private const int CurrentVersion = 1;

    public void Save(GameData data)
    {
        try
        {
            var serializableData = new SerializableGameData(data);
            serializableData.version = CurrentVersion;
            var json = JsonUtility.ToJson(serializableData);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save data: {e.Message}");
        }
    }

    public GameData Load()
    {
        if (!PlayerPrefs.HasKey(SaveKey)) return new GameData();

        try
        {
            var json = PlayerPrefs.GetString(SaveKey);
            var serializableData = JsonUtility.FromJson<SerializableGameData>(json);

            // Handle versioning and data migration here if needed
            if (serializableData.version != CurrentVersion)
            {
                Debug.LogWarning("Data version mismatch. Performing data migration.");
                // Perform data migration logic here if needed
                // Example: MigrateData(serializableData, serializableData.version, CurrentVersion);
            }

            return serializableData.ToGameData();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}");
            return new GameData();
        }
    }
}

[System.Serializable]
public class SerializableGameData
{
    public string points;
    public string totalPoints;
    public string prestigePoints;
    public string upgradeLevels;
    public int version;

    public SerializableGameData() { }

    public SerializableGameData(GameData data)
    {
        points = data.points.ToString();
        totalPoints = data.totalPoints.ToString();
        prestigePoints = data.prestigePoints.ToString();
        upgradeLevels = SerializeDictionary(data.upgradeLevels);
        version = 1;
    }

    public GameData ToGameData()
    {
        return new GameData(
            BigDouble.Parse(points),
            BigDouble.Parse(totalPoints),
            BigDouble.Parse(prestigePoints),
            DeserializeDictionary(upgradeLevels)
        );
    }

    private static string SerializeDictionary(Dictionary<string, BigDouble> dict)
    {
        var sb = new StringBuilder();
        foreach (var pair in dict)
        {
            if (sb.Length > 0)
            {
                sb.Append(",");
            }
            sb.Append(Escape(pair.Key));
            sb.Append(":");
            sb.Append(Escape(pair.Value.ToString()));
        }
        return sb.ToString();
    }

    private static Dictionary<string, BigDouble> DeserializeDictionary(string str)
    {
        var dict = new Dictionary<string, BigDouble>();
        if (string.IsNullOrEmpty(str)) return dict;

        var pairs = str.Split(',');
        foreach (var pair in pairs)
        {
            var parts = pair.Split(':');
            if (parts.Length != 2) continue;

            var key = Unescape(parts[0]);
            var value = Unescape(parts[1]);

            dict[key] = BigDouble.Parse(value);
        }
        return dict;
    }

    private static string Escape(string str)
    {
        return str.Replace("\\", "\\\\").Replace(",", "\\,").Replace(":", "\\:");
    }

    private static string Unescape(string str)
    {
        return str.Replace("\\,", ",").Replace("\\:", ":").Replace("\\\\", "\\");
    }
}