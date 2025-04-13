using UnityEngine;

public class BallFlyweight : Flyweight
{
    new BallFlyweightSettings settings => (BallFlyweightSettings)base.settings;
}
