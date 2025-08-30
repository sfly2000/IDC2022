using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


[System.Serializable]

public class Bosh_slope
{
    public WheelCollider Wheel;
    public bool motor;
    public bool steering;
    public Rigidbody rb;//cylinder
}

public class SevenA_bosh_pospd : MonoBehaviour
{
    [System.Serializable]
    public class SetKey
    {
        public string InputKeyUp;
        public string InputKeyDown;
        public bool up_flag;
    }
    public SetKey InputKey;
    //****************************************************************************************************************************
    // for pos control

    [System.Serializable]
    public class PosControlParameter
    {
        public float aimup = 50f;
        public float aimdown = 0f;
        public float aim = 0f;
        public float kp_p = 0.2f;
        public float ki_p = 0.01f;
        public float kd_p = 0.05f;
        public float delta_position_rx;
        public float integral_rx;
        public float diff_rx;
        //public float delta_position_z;
        //public float integral_position_z;
        //public float diff_pz;
    }
    //0.5 0.03 0.2
    //0.4 0.01 0.4
    public PosControlParameter PosControl;

    public List<Bosh_slope> axleInfos;
    public float maxMotorTorque;
    public float gain;
    public float motorTorque;
    public float brakeTorque;
    //public float power;

    //WheelCollider col = null;

    public PhotonView photonView;

    void Start()
    {
        InputKey.up_flag = false;
        //col = GetComponent<WheelCollider>();
        //Debug.Log(col);
        //ApplyLocalPositionToVisualsD(col);
        //photonView = PhotonView.Get(this);
        //photonView = GetComponent<PhotonView>(); 
    }
    void Update()
    {

    }


    // 対応する視覚的なホイールを見つけます
    // Transform を正しく適用します
    public void ApplyLocalPositionToVisualsD(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
        {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        //Debug.Log(rotation);
        visualWheel.transform.rotation = rotation;
        visualWheel.transform.Rotate(0f, 0f, 90f);
    }

    public void FixedUpdate()
    {
        float motor = maxMotorTorque; //* Input.GetAxis("Vertical");


        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            if (Input.GetKey(InputKey.InputKeyUp))
            {//run
                InputKey.up_flag = true;
                PosControl.aim = PosControl.aimup;
            }
            else if (Input.GetKey(InputKey.InputKeyDown))
            {
                InputKey.up_flag = false;
                PosControl.aim = PosControl.aimdown;
            }
            //if (InputKey.up_flag)
            //{
                motorTorque = 0f;
                //brakeTorque = -1f * maxMotorTorque;
                brakeTorque = 0f;

                Transform Wheel = axleInfos[0].Wheel.transform.GetChild(0);
                //float wheel_rotate=Wheel.transform.eulerAngles.x / Mathf.PI * 180f;
                //float wheel_rotate = Wheel.transform.eulerAngles.x;
                //magnitude返回向量长度，目标点到(0,0,0)的距离
                //angularVelocity返回弧度制
                float wheel_rotate = axleInfos[0].rb.transform.eulerAngles.x / 180f * Mathf.PI;
                float wheel_rotate_v = axleInfos[0].rb.angularVelocity.x;
                PosControl.delta_position_rx = PosControl.aim / 180f * Mathf.PI - wheel_rotate;
                if (PosControl.delta_position_rx < -Mathf.PI)
                {
                    PosControl.delta_position_rx += 2 * Mathf.PI;
                }
                PosControl.diff_rx = wheel_rotate_v;
                gain = Mathf.Clamp(PosControl.kp_p * PosControl.delta_position_rx + PosControl.ki_p * (-1f * PosControl.diff_rx), -1, 1);
                motorTorque = -1f*gain * maxMotorTorque;
            //}
            //else   //pd控制，试图回到down位置
            //{
            //    motorTorque = 0f;
            //    //brakeTorque = -1f * maxMotorTorque;
            //    brakeTorque = 0f;

            //    Transform Wheel = axleInfos[0].Wheel.transform.GetChild(0);
            //    //float wheel_rotate=Wheel.transform.eulerAngles.x / Mathf.PI * 180f;
            //    //float wheel_rotate = Wheel.transform.eulerAngles.x;
            //    //magnitude返回向量长度，目标点到(0,0,0)的距离
            //    //angularVelocity返回弧度制
            //    float wheel_rotate = axleInfos[0].rb.transform.eulerAngles.x / 180f * Mathf.PI;
            //    float wheel_rotate_v = axleInfos[0].rb.angularVelocity.x;
            //    PosControl.delta_position_rx = PosControl.aimdown / 180f * Mathf.PI - wheel_rotate;
            //    if (PosControl.delta_position_rx < -Mathf.PI)
            //    {
            //        PosControl.delta_position_rx += 2 * Mathf.PI;
            //    }
            //    PosControl.diff_rx = wheel_rotate_v;
            //    gain = Mathf.Clamp(PosControl.kp_p * PosControl.delta_position_rx + PosControl.ki_p * (-1f * PosControl.diff_rx), -1, 1);
            //    motorTorque = gain * maxMotorTorque;
            //    //Debug.Log(wheel_rotate);
            //}
            foreach (Bosh_slope axleInfo in axleInfos)
            {
                if (axleInfo.motor)
                {
                    axleInfo.Wheel.motorTorque = motorTorque;
                    axleInfo.Wheel.brakeTorque = brakeTorque;
                    //axleInfo.Wheel.motorTorque = -1f * maxMotorTorque;
                }
                ApplyLocalPositionToVisualsD(axleInfo.Wheel);
            }
        }
    }
}

