using BreakInfinity;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerUpConfig", menuName = "Scriptable Objects/PowerUpConfig")]
public class PowerUpConfig : ScriptableObject
{
    public string Name;
    public BigDouble Multiplier;
    public float Duration;
    public BigDoubleSO Target;
}
