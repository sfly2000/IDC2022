using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SevenA_pumpUAV : MonoBehaviour
{
    public float force;
    public bool origin_up;
    private Rigidbody rb;
    private ArticulationBody ab;
    public axis Axis;
    private Vector3 vector;
    public string input_up, input_down;  //key names of positive and negative input;
    private bool is_rigid = true;
    public PhotonView photonView;
    public bool minus;
    public float downYPos;
    public float upYPos;
    public float Kp;
    public float Kd;
    private bool gateStateUp;
    public float vel;
    public float pos;
    public float y_d;
    public float out_value;

    void Start()
    {
        gateStateUp = origin_up;
        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
            is_rigid = true;
        }
        else
        {
            ab = GetComponent<ArticulationBody>();
            is_rigid = false;
        }
        switch (Axis)
        {
            case axis.X:
                vector = Vector3.right;
                break;
            case axis.Y:
                vector = Vector3.up;
                break;
            case axis.Z:
                vector = Vector3.forward;
                break;
            default:
                break;
        }
    }

    void FixedUpdate()
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            if (Input.GetKey(input_up))
            {
                gateStateUp = true;
            }
            else if (Input.GetKey(input_down))
            {
                gateStateUp = false;
            }

            pos = transform.localPosition.x;
            vel = rb.velocity.x;

            if (gateStateUp)
            {
                y_d = upYPos;
            }
            else { y_d = downYPos; }

            out_value = Mathf.Clamp(Kp * (y_d - pos) + Kd * (-vel), -force, force);
            if (Mathf.Abs(y_d-pos)<0.03)
            {
                if (y_d - pos<0)
                {
                    out_value = -force;
                }
                else
                {
                    out_value = force;
                }
            }
            if (minus)
            {
                out_value = out_value * -1f;
            }
            if (is_rigid)
            {
                rb.AddRelativeForce(vector * out_value);
            }
            else
            {
                ab.AddRelativeForce(vector * out_value);
            }
        }
    }
}
