using UnityEngine;

public class JsonSaveSerializer : ISaveSerializer
{
    public string Serialize(object obj) => JsonUtility.ToJson(obj);
    public T Deserialize<T>(string data) => JsonUtility.FromJson<T>(data);
}
