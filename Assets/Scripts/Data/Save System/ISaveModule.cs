using System;

public interface ISaveModule
{
    string ModuleName { get; }
    int Version { get; }
    string Serialize();
    void Deserialize(string data);
    void OnMigrate(int fromVersion, int toVersion);
}