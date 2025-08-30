using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;


public class o3A_DriveWheelR : MonoBehaviour
{
    [System.Serializable]
    public class AxleInfoDR
    {
        public WheelCollider Wheel;
        public bool motor;
        public bool steering;
    }

    public bool isManual = true;
    public List<AxleInfoDR> axleInfos;
    public float maxMotorTorque;

    public float power;

    //WheelCollider col = null;

    public PhotonView photonView;

    void Start()
    {
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
        if (!isManual)
        {
            return;
        }

        float motor = maxMotorTorque; //* Input.GetAxis("Vertical");


        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {

            foreach (AxleInfoDR axleInfo in axleInfos)
            {
                if (axleInfo.motor)
                {
                    axleInfo.Wheel.motorTorque = 0.0f;
                    axleInfo.Wheel.brakeTorque = 0.0f;
                    ///axleInfo.Wheel.motorTorque = (power+stickR.x)*maxMotorTorque*stickR.y;
                    //axleInfo.Wheel.motorTorque = -10.0f;
                    //axleInfo.Wheel.motorTorque = (stickR.y+2.0f*stickR.x)*maxMotorTorque;
                    float x, y;
                    x = 0.0f; y = 0.0f;

                    if (Application.platform == RuntimePlatform.Android)
                    {
                        // 左手のアナログスティックの向きを取得
                        Vector2 stickL = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
                        // 右手のアナログスティックの向きを取得
                        Vector2 stickR = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

                        x = stickR.x;
                        y = stickR.y;
                    }
                    else
                    //if (Application.platform == RuntimePlatform.WindowsPlayer)
                    {
                        x = 1.0f * Input.GetAxis("Horizontal");
                        y = 1.0f * Input.GetAxis("Vertical");
                        if (x != 0 || y != 0)
                        {
                            axleInfo.Wheel.brakeTorque = 0;
                        }
                        else if (Input.GetKey(KeyCode.Space))
                        {
                            Brake(axleInfo.Wheel, 10.0f);
                        }
                        else
                        {
                            Brake(axleInfo.Wheel, 3.0f);

                        }
                    }

                    ApplyTorque(axleInfo.Wheel, x, y);
                }
                ApplyLocalPositionToVisualsD(axleInfo.Wheel);

            }
        }
    }

    private void Brake(WheelCollider wheelCollider, float amount)
    {
        wheelCollider.brakeTorque = -amount * maxMotorTorque;
    }

    // x for turn, y for forward
    private void ApplyTorque(WheelCollider wheelCollider, float x, float y)
    {
        wheelCollider.motorTorque = (-y + 2.0f * x) * maxMotorTorque;  //根据左右两个轮子的差值（x）实现转向
    }

    // x for turn, y for forward
    public void ApplyTorqueAllWheels(float x, float y)
    {
        foreach (AxleInfoDR axleInfo in axleInfos)
        {
            axleInfo.Wheel.brakeTorque = 0;
            ApplyTorque(axleInfo.Wheel, x, y);
            ApplyLocalPositionToVisualsD(axleInfo.Wheel);
        }
    }

    public void BrakeAllWheels(float amount)
    {
        foreach(AxleInfoDR axleInfo in axleInfos)
        {
            Brake(axleInfo.Wheel, amount);
            ApplyLocalPositionToVisualsD(axleInfo.Wheel);
        }
    }
}

