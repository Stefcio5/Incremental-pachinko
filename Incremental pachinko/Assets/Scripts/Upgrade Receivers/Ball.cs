using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BreakInfinity;
using UnityEngine;

public class Ball : UpgradeReceiver
{
    [SerializeField] private float gravityScale;
    public BigDouble ballMultiplier;
    private Rigidbody rb;
    protected override void Start()
    {
        base.Start();
        Debug.Log($"Ball value: {upgradePower.FinalValue}");
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0;
        rb.angularDamping = 0;
    }

    public void Init(BallFlyweightSettings settings)
    {
        ballMultiplier = settings.multiplier;
    }

    private void OnEnable()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
            Debug.Log("Got rigidbody component");
        }
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override BigDouble GetUpgradeValue()
    {
        return ballMultiplier * upgradePower.FinalValue;
    }

    private void FixedUpdate()
    {
        Vector3 gravity = (Physics.gravity * gravityScale) - Physics.gravity;
        rb.AddForce(gravity, ForceMode.Acceleration);
    }
}
