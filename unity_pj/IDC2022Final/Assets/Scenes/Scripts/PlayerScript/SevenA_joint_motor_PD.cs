using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
/*public enum axis
{
    X,
    Y,
    Z
};*/

public class SevenA_joint_motor_PD : MonoBehaviour
{
    public float maxTorque;
    private Rigidbody rb;
    private ArticulationBody ab;
    public axis Axis;
    private Vector3 vector;
    public string input_grip, input_rls;  //key name of positive and negative input;
    private bool is_rigid = true;
    public PhotonView photonView;
    private Quaternion theta;
    public float gripAngle;
    public float releaseAngle;
    public float Kp;
    public float Kd;
    private bool gripState;
    private float theta_d;
    private float omega;
    public float out_value; 
    


    void Start()
    {
        gripState = true;
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
            if(Input.GetKey(input_grip))
            {
                gripState=true;
            }
            else if(Input.GetKey(input_rls))
            {
                gripState=false;
            }

            //theta = transform.localRotation.eulerAngles;
            theta = rb.transform.rotation;
            omega = rb.angularVelocity.x;
            //Debug.Log(theta.x);
            //Debug.Log(Mathf.DeltaAngle(theta_d, theta.x));
            if (gripState) {
                theta_d = gripAngle;
                out_value = -maxTorque;
            } else 
            {
                theta_d = releaseAngle;
                out_value = maxTorque;
            }

            //out_value = Mathf.Clamp(Kp*Mathf.DeltaAngle(theta.x, theta_d) +Kd*(-omega),-maxTorque,maxTorque);
            
            if (is_rigid)
            {
                
                rb.AddRelativeTorque(vector * out_value);
            }
            else
            {
                ab.AddRelativeTorque(vector * out_value);
            }
        }  
    }
}
