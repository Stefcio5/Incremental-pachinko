using UnityEngine;

public class BallFlyweight : Flyweight
{
    new BallFlyweightSettings settings => (BallFlyweightSettings)base.settings;
    private Ball _ball;
    private MeshRenderer _meshRenderer;

    private void Awake()
    {
        if (_ball == null)
        {
            _ball = GetComponent<Ball>();
        }
        if (_meshRenderer == null)
        {
            _meshRenderer = GetComponent<MeshRenderer>();
        }
    }
    //TODO: Check MaterialPropertyBlock for performance
    public override void Init()
    {
        _ball.Init(settings);
        _meshRenderer.material = settings.material;
    }
}
