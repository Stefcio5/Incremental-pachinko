using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using BreakInfinity;
using UnityEngine;

public class Ball : UpgradeReceiver
{
    private Rigidbody rb;
    [SerializeField] private float gravityScale;
    protected override void Start()
    {
        base.Start();
        Debug.Log($"Ball value: {Value}");
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = 0;
        rb.angularDamping = 0;
    }

    private void FixedUpdate()
    {
        Vector3 gravity = (Physics.gravity * gravityScale) - Physics.gravity;
        rb.AddForce(gravity, ForceMode.Acceleration);
        Debug.Log(rb.linearVelocity);
    }
}
