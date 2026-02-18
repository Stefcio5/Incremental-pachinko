using BreakInfinity;
using UnityEngine;

public class Ball : UpgradeReceiver
{
    [SerializeField] private float _gravityScale;

    private BigDouble _ballMultiplier;
    private Rigidbody _rb;
    private Color _ballColor;
    private int _ballID;

    public int BallID => _ballID;
    public Color BallColor => _ballColor;

    private readonly Vector3 _gravity = new Vector3(0, -20f, 0);
    private const float MaxLinearVelocity = 30f;
    private const float MaxVelocityXZ = 10f;
    private const float MaxVelocityYDown = -20f;
    private const float MaxVelocityYUp = 10f;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();

        if (_rb is null)
        {
            Debug.LogError($"[{nameof(Ball)}] Rigidbody component not found.");
            return;
        }

        _rb.maxLinearVelocity = MaxLinearVelocity;
    }

    protected override void Start()
    {
        base.Start();
        Physics.gravity = _gravity;
    }

    public void Init(BallFlyweightSettings settings)
    {
        _ballMultiplier = settings.multiplier;
        _ballColor = settings.color;
        _ballID = settings.ID;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (_rb is null)
        {
            return;
        }

        _rb.maxLinearVelocity = MaxLinearVelocity;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (_rb is null)
        {
            return;
        }

        // Apply additional gravity force for precise ball physics
        _rb.AddForce(Vector3.down * _gravityScale, ForceMode.Acceleration);

        // Clamp velocity to reasonable ranges
        Vector3 vel = _rb.linearVelocity;
        vel.x = Mathf.Clamp(vel.x, -MaxVelocityXZ, MaxVelocityXZ);
        vel.y = Mathf.Clamp(vel.y, MaxVelocityYDown, MaxVelocityYUp);
        vel.z = Mathf.Clamp(vel.z, -MaxVelocityXZ, MaxVelocityXZ);
        _rb.linearVelocity = vel;
    }

    public override BigDouble GetUpgradeValue()
    {
        return _ballMultiplier * upgradePower.FinalValue;
    }
}
