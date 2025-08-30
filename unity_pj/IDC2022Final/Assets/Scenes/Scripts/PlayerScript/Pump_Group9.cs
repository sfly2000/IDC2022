using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Pump_Group9 : MonoBehaviour
{
    public float force;
    private Rigidbody rb;
    private ArticulationBody ab;
    public axis Axis;
    private Vector3 vector;
    public string input_pos, input_neg;  //key names of positive and negative input;
    private bool is_rigid = true;
    public bool dir_positive = true;  //pump pushes in positive direction;
    public PhotonView photonView;

    void Start()
    {
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
            float dir = 0;
            if(Input.GetKey(input_pos))
            {
                dir = 1;
            }
            else if(Input.GetKey(input_neg))
            {
                dir = -1;
            }
            if (!dir_positive) dir *= -1;

            if (is_rigid)
            {
                rb.AddRelativeForce(vector * force * dir);
                //rb.AddForce(-Physics.gravity);
            }
            else
            {
                ab.AddRelativeForce(vector * force * dir);
                //ab.AddForce(-Physics.gravity);
            }
        }
    }
}
