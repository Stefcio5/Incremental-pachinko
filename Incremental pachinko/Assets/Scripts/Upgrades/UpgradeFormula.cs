using BreakInfinity;
using UnityEngine;

public abstract class UpgradeFormula : ScriptableObject
{
    public abstract BigDouble Calculate(BigDouble baseValue, BigDouble level);
}


