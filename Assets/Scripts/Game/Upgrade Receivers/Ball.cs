using BreakInfinity;
using UnityEngine;

public class Ball : UpgradeReceiver
{
    [SerializeField] private float _gravityScale;
    private BigDouble _ballMultiplier;
    private Rigidbody _rb;
    private Color _ballColor;
    Vector3 gravity = new Vector3(0, -20f, 0);

    public Color BallColor { get => _ballColor; }

    protected override void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("Rigidbody component not found on the ball.");
        }
        _rb.maxLinearVelocity = 30f;
    }
    protected override void Start()
    {
        base.Start();
        Physics.gravity = gravity;
    }

    public void Init(BallFlyweightSettings settings)
    {
        _ballMultiplier = settings.multiplier;
        _ballColor = settings.color;
    }

    private void OnEnable()
    {
        _rb.maxLinearVelocity = 30f;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    public override BigDouble GetUpgradeValue()
    {
        return _ballMultiplier * upgradePower.FinalValue;
    }

    private void FixedUpdate()
    {
        _rb.AddForce(Vector3.down * _gravityScale, ForceMode.Acceleration);

        Vector3 vel = _rb.linearVelocity;
        vel.x = Mathf.Clamp(vel.x, -10f, 10f);
        vel.y = Mathf.Clamp(vel.y, -20f, 10f);
        vel.z = Mathf.Clamp(vel.z, -10f, 10f);
        _rb.linearVelocity = new Vector3(vel.x, vel.y, vel.z);
    }
}
