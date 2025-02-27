using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    static readonly Dictionary<float, WaitForSeconds> WaitForSeconds = new();

    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
        if (WaitForSeconds.TryGetValue(seconds, out var waitForSeconds)) return waitForSeconds;

        var newWaitForSeconds = new WaitForSeconds(seconds);
        WaitForSeconds.Add(seconds, newWaitForSeconds);
        return WaitForSeconds[seconds];
    }

}
