using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGame(GameData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/game.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGame()
    {
        string path = Application.persistentDataPath + "/game.fun";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}

// [System.Serializable]
// public class GameData
// {
//     public PlayerData player;
//     public LevelData level;
//     public InventoryData inventory;

//     public GameData(PlayerData player, LevelData level, InventoryData inventory)
//     {
//         this.player = player;
//         this.level = level;
//         this.inventory = inventory;
//     }
// }

[System.Serializable]
public class PlayerData
{
    public int health;
    public int score;

    public PlayerData(PlayerData player)
    {
        health = player.health;
        score = player.score;
    }
}

[System.Serializable]
public class LevelData
{
    public int currentLevel;

    public LevelData(LevelData level)
    {
        currentLevel = level.currentLevel;
    }
}

[System.Serializable]
public class InventoryData
{
    public int[] items;

    public InventoryData(InventoryData inventory)
    {
        items = inventory.items;
    }
}