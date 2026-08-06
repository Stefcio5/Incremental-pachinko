public interface ISaveSerializer
{
    string Serialize(object obj);
    T Deserialize<T>(string data);
}
