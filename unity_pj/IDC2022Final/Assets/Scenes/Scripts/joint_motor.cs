using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public enum axis
{
    X,
    Y,
    Z
};

public class joint_motor : MonoBehaviour
{
    public float torque;
    private Rigidbody rb;
    private ArticulationBody ab;
    public axis Axis;
    private Vector3 vector;
    public string input_pos, input_neg;  //key name of positive and negative input;
    private bool is_rigid = true;
    public PhotonView photonView;


    void Start()
    {
        if(GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
            is_rigid = true;
        }
        else
        {
            ab = GetComponent<ArticulationBody>();
            is_rigid = false;
        }
        switch(Axis)
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
        if(PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            float turn = 0;
            if(Input.GetKey(input_pos))
            {
                turn = 1;
            }
            else if(Input.GetKey(input_neg))
            {
                turn = -1;
            }

            if (is_rigid)
            {
                rb.AddRelativeTorque(vector * torque * turn);
            }
            else
            {
                ab.AddRelativeTorque(vector * torque * turn);
            }
        }  
    }
}
