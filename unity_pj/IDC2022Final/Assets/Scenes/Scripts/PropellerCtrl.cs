using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellerCtrl : MonoBehaviour
{
    public Rigidbody rb;
    private float max_force = 20;
    private float max_torque = 20;
    private float dumping = 0.5f;
    private float force = 0;
    private float input_force;
    private float torque = 0;
    private float input_torque;

    // Start is called before the first frame update
    void Start()
    {
        force = 0;
        torque = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AddForceAndTorque();
    }

    void AddForceAndTorque()
    {
        input_force = Mathf.Clamp(input_force, 0, max_force);
        input_torque = Mathf.Clamp(input_torque, -max_torque, max_torque);
        if (input_force > force)
        {
            force = Mathf.Min(force + dumping * Time.deltaTime * max_force, input_force);
        }
        else if (input_force < force)
        {
            force = Mathf.Max(force - dumping * Time.deltaTime * max_force, input_force);
        }
        if (input_torque > torque)
        {
            torque = Mathf.Min(torque + dumping * Time.deltaTime * max_torque, input_torque);
        }
        else if (input_torque < torque)
        {
            torque = Mathf.Max(torque - dumping * Time.deltaTime * max_torque, input_torque);
        }
        rb.AddRelativeForce(Vector3.up * force, ForceMode.Force);
        rb.transform.Rotate(Vector3.up * torque * -2f);
    }

    public void SetInputForce(float inputforce)
    {
        input_force = inputforce;
    }

    public void SetInputTorque(float inputtorque)
    {
        input_torque = inputtorque;
    }
}