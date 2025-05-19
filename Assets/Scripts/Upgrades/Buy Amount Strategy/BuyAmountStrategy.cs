using System.Runtime.CompilerServices;
using BreakInfinity;
using UnityEngine;


public abstract class BuyAmountStrategy : ScriptableObject
{
    public abstract BigDouble GetBuyAmount(Upgrade upgrade);
    public abstract BigDouble GetCost(Upgrade upgrade);
}
