using System;
using Cysharp.Threading.Tasks;
using System.Threading;

public interface IManager : IGameSystem
{
    void Shutdown();
    void Reset();
    event Action OnShutdown;
    event Action OnReset;
}
