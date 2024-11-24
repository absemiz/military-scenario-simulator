using UnityEngine;

public class UAVController : MonoBehaviour
{
    public float thrustForce = 15000f; // N*s
    public float turnSpeed = 0.5f; // rad/s
    public float maxLift = 20000f; // N
    public float dragCoefficient = 0.02f; // unitless
    public float wingArea = 20f; // m^2

    private Rigidbody mRigidbody;

    void Start()
    {
        mRigidbody = GetComponent<Rigidbody>();
        mRigidbody.mass = 2223f;
    }

    void FixedUpdate()
    {
        mRigidbody.AddForce(thrustForce * Time.deltaTime * transform.forward, ForceMode.Force);

        float airspeed = mRigidbody.velocity.magnitude;

        float lift = CalculateLiftForce(airspeed);
        mRigidbody.AddForce(lift * Vector3.up, ForceMode.Force);

        Vector3 dragForce = CalculateDragForce(airspeed);
        mRigidbody.AddForce(dragForce, ForceMode.Force);

        float rollInput = -Input.GetAxis("Horizontal");
        float pitchInput = -Input.GetAxis("Vertical");

        mRigidbody.AddTorque(rollInput * Time.deltaTime * turnSpeed * transform.right, ForceMode.Force);
        mRigidbody.AddTorque(pitchInput * Time.deltaTime * turnSpeed * transform.forward, ForceMode.Force);
    }

    private float CalculateLiftForce(float airspeed)
    {
        float airDensity = 1.225f; // kg/m^3
        float liftCoefficient = 1.5f;
        return 0.5f * airDensity * airspeed * airspeed * wingArea * liftCoefficient;
    }

    private Vector3 CalculateDragForce(float airspeed)
    {
        float airDensity = 1.225f; // kg/m^3
        float dragMagnitude = 0.5f * airDensity * airspeed * airspeed * wingArea * dragCoefficient;
        return -dragMagnitude * transform.right;
    }
}
