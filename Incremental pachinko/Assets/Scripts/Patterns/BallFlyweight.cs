using UnityEngine;

public class BallFlyweight : Flyweight
{
    new BallFlyweightSettings settings => (BallFlyweightSettings)base.settings;
    private Ball ball;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        if (ball == null)
        {
            ball = GetComponent<Ball>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
    }
    //TODO: Check MaterialPropertyBlock for performance
    public override void Init()
    {
        ball.Init(settings);
        meshRenderer.material = settings.material;
    }
}
