using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[System.Serializable]

public class AxleInfoDL
{
    public WheelCollider Wheel;
    public bool motor;
    public bool steering;
}

public class DriveWheelL : MonoBehaviour
{
    public List<AxleInfoDL> axleInfos;
    public float maxMotorTorque;

    public float power;

    //WheelCollider col = null;
    public PhotonView photonView;

    //void Awake() 
    //{} 
    void Start()
    {
        //col = GetComponent<WheelCollider>();
        //Debug.Log(col);
        //ApplyLocalPositionToVisualsD(col);
        //PhotonView = PhotonView.Get(this);  
        //PhotonView = GetComponent<PhotonView>(); 
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
            foreach (AxleInfoDL axleInfo in axleInfos)
            {
                axleInfo.Wheel.motorTorque = 0.0f;
                axleInfo.Wheel.brakeTorque = 0.0f;

                if (axleInfo.motor)
                {
                    //axleInfo.Wheel.motorTorque = -10.0f;

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
                        else if (Input.GetKey(KeyCode.Space)){
                            axleInfo.Wheel.brakeTorque = -10.0f * maxMotorTorque;
                        }
                        else{
                            axleInfo.Wheel.brakeTorque = -3.0f * maxMotorTorque;
                        }
                    }
                    
                    axleInfo.Wheel.motorTorque = (-y - 2.0f * x) * maxMotorTorque;
                }
                ApplyLocalPositionToVisualsD(axleInfo.Wheel);

            }
        }
    }

    public override bool Equals(object obj)
    {
        return obj is DriveWheelL l &&
               base.Equals(obj) &&
               EqualityComparer<PhotonView>.Default.Equals(photonView, l.photonView);
    }

    public override int GetHashCode()
    {
        int hashCode = -164824088;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<PhotonView>.Default.GetHashCode(photonView);
        return hashCode;
    }
}

