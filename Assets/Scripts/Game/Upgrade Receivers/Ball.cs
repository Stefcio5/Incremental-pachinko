using BreakInfinity;
using UnityEngine;

public class Ball : UpgradeReceiver
{
    [SerializeField] private float _gravityScale;
    private BigDouble _ballMultiplier;
    private Rigidbody _rb;
    protected override void Start()
    {
        base.Start();
    }

    public void Init(BallFlyweightSettings settings)
    {
        _ballMultiplier = settings.multiplier;
    }

    private void OnEnable()
    {
        if (_rb == null)
        {
            _rb = GetComponent<Rigidbody>();
            Debug.Log("Got rigidbody component");
            _rb.linearDamping = 0;
            _rb.angularDamping = 0;
        }
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public override BigDouble GetUpgradeValue()
    {
        return _ballMultiplier * upgradePower.FinalValue;
    }

    private void FixedUpdate()
    {
        Vector3 gravity = (Physics.gravity * _gravityScale) - Physics.gravity;
        _rb.AddForce(gravity, ForceMode.Acceleration);
    }
}
