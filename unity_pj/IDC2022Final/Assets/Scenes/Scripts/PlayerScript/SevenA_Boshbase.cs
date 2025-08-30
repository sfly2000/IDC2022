using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;



[System.Serializable]
public class Bosh_motor
{
    public WheelCollider Wheel;
    public bool motor;
    public bool steering;
    public bool left;
}

public class SevenA_Boshbase : MonoBehaviour
{
    [System.Serializable]
    public class SetKey
    {
        public string InputKeyForward;
        public string InputKeyBackward;
        //public string InputKeyRight;
        //public string InputKeyLeft;
        public string InputKeyTurnright;
        public string InputKeyTurnleft;
        //�л�״̬ʹ�� isworking
        public string InputKeyButtton;
    }
    public SetKey InputKey;
    //****************************************************************************************************************************
    // for pos control
    [System.Serializable]
    public class PosControlParameter
    {
        public float kp_p = 0.4f;
        public float ki_p = 0.01f;
        public float kd_p = 0.4f;
        public float delta_position_x;
        public float integral_position_x;
        public float diff_px;
        public float delta_position_z;
        public float integral_position_z;
        public float diff_pz;
    }
    //0.5 0.03 0.2
    //0.4 0.01 0.4
    public PosControlParameter PosControl;
    private Vector3 position_old;
    private float delta_position_z_old = 0f;
    private float delta_position_x_old = 0f;
    private float rotation_y_old = 0f;
    
    //***************************************
    //λ�ú�������ʾ
    [System.Serializable]
    public class ShowPosForce
    {
        public float posx;
        public float posy;
        public float posz;

        //�� ��
        public float Ltorque = 0f;
        public float Rtorque = 0f;
        public float lineTorque;
        public float rotateTorque;
        public float brakeTorque;
    }
    public ShowPosForce PosForce;
    

    //***************************************
    //set targets
    [System.Serializable]
    public class SetTarget
    {
        //Set expected position 
        public float expect_position_x;
        //public float expect_position_y = 4f;
        public float expect_position_z;
        public float expect_rotation_y;
        //public float expect_velocity_x;
        public float expect_position_forward;
        public float expect_velocity_forward;
        public float expect_velocity_wy;

        //�ֶ�����
        public float set_vforward=2f;
        public float set_vwy=10f;
        //public float expect_velocity_z;
    }
    public SetTarget Target;
    public int Target_num = 0;
    public bool cflag = false;
    /*
    public void change_target(float pos_x, float pos_y, float pos_z, float vel_x, float vel_z)//float vel_x, float vel_y, float vel_z,float pump_state
    {

        //float res = 0;
        //�Ѿ��ﵽĿ��㸽��,��ƽ�Ȳ�����
        if (Target_num != Target_x.Length - 1 && System.Math.Abs(pos_x - Target.expect_position_x) < 0.12
            && System.Math.Abs(pos_z - Target.expect_position_z) < 0.12 && System.Math.Abs(vel_x) < 0.05 && System.Math.Abs(vel_z) < 0.05)//&& System.Math.Abs(vel_x) < 0.1
        {
            ////�������Ϊ��״̬���Ͳ�������Ϊ��������ȥ����Ҫ��yҲ�ﵽ��Χ
            //if (pump_state == -1f)
            //{
            if (System.Math.Abs(pos_y - Target.expect_position_y) < 0.1)
            {
                if (pos_x != Target_x[Target_num + 1] || pos_y != Target_y[Target_num + 1] || pos_z != Target_z[Target_num + 1])
                {
                    Target_num += 1;
                    cflag = true;
                }
            }
            //}
        }
        //return change_flag;
    }
    */
    //**************************************
    //����boshmotors
    public List<Bosh_motor> Boshmotors;
    //**************************************
    //����
    public float maxMotorTorque;
    public PhotonView photonView;
    public bool is_working;
    public Rigidbody rig;//����

    //void Awake() 
    //{} 
    void Start()
    {
        is_working = true;
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            position_old = rig.transform.position;
            Target.expect_position_x = rig.transform.position.x;
            Target.expect_rotation_y = rig.transform.rotation.y;
            //Target.expect_position_y = 4f;
            Target.expect_position_z = rig.transform.position.z;
            Target.expect_position_forward = 0f;
            Target.expect_velocity_forward = 0f;
            Target.expect_velocity_wy = 0f;
            //Target.expect_velocity_z = 0f;
            //Target.expect_position_x = Target_x[Target_num];
            //Target.expect_position_y = Target_y[Target_num];
            //Target.expect_position_z = Target_z[Target_num];
            //dir = Target_pump[Target_num];
            //Pump.pumpbalance = Target_pumpbalance[Target_num];
            //velocity_old = rig.transform.position;
            rotation_y_old = rig.transform.eulerAngles.y;
            //rotation_z_old = rig.transform.eulerAngles.y;
            //filter = new FilterParamter();
            //cam.SetActive(true);
        }
        else
        {
            Debug.Log("photonView.IsMine == False");
            //cam.SetActive(false);
        }
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {//�а������¾͵�������״̬
            if (Input.GetKeyDown(InputKey.InputKeyButtton))
            {
                is_working = !is_working;
            }
        }
    }


    // ���ꤹ��ҕҙ�Ĥʥۥ��`���Ҋ�Ĥ��ޤ�
    // Transform ���������m�ä��ޤ�
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
            //****************************************************************************************************************************
            //on\off      
            if (!is_working)
            {
                PosForce.Ltorque = 0f;
                PosForce.Rtorque = 0f;
                
            }
            
            
            if (Input.GetKey(InputKey.InputKeyForward))
            {//��ǰ
                Target.expect_position_forward = Target.expect_position_forward + Target.set_vforward * Time.deltaTime;
                PosForce.lineTorque = 1f;
                PosForce.rotateTorque = 0f;
                PosForce.brakeTorque = 0f;
            }
            else if (Input.GetKey(InputKey.InputKeyBackward))
            {
                Target.expect_position_forward = Target.expect_position_forward - Target.set_vforward * Time.deltaTime;
                PosForce.lineTorque = -1f;
                PosForce.rotateTorque = 0f;
                PosForce.brakeTorque = 0f;
            }
            //��y����ת
            else if (Input.GetKey(InputKey.InputKeyTurnleft))
            {
                Target.expect_rotation_y -= Target.set_vwy * Time.deltaTime;
                PosForce.lineTorque = 0f;
                PosForce.rotateTorque = 1f;
                PosForce.brakeTorque = 0f;

            }
            else if (Input.GetKey(InputKey.InputKeyTurnright))
            {
                Target.expect_rotation_y += Target.set_vwy * Time.deltaTime;
                PosForce.lineTorque = 0f;
                PosForce.rotateTorque = -1f;
                PosForce.brakeTorque = 0f;
            }
            else
            {
                foreach (Bosh_motor Boshmotor in Boshmotors)
                {
                    Boshmotor.Wheel.motorTorque = 0f * maxMotorTorque;
                    Boshmotor.Wheel.brakeTorque = -1.0f * maxMotorTorque;//����Ϊ��
                    ApplyLocalPositionToVisualsD(Boshmotor.Wheel);
                }
                return;
            }
            //��Ȼ�Ż�����������
            foreach (Bosh_motor Boshmotor in Boshmotors)
            {
                if (Boshmotor.left)
                {
                    PosForce.Ltorque = (-PosForce.lineTorque - PosForce.rotateTorque)*maxMotorTorque;
                    Boshmotor.Wheel.motorTorque = PosForce.Ltorque;
                }
                else
                {
                    PosForce.Rtorque = (-PosForce.lineTorque + PosForce.rotateTorque) * maxMotorTorque;
                    Boshmotor.Wheel.motorTorque = PosForce.Rtorque;
                }
                Boshmotor.Wheel.brakeTorque = PosForce.brakeTorque *maxMotorTorque;
                ApplyLocalPositionToVisualsD(Boshmotor.Wheel);
            }

        }
        /*
            if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            foreach (Bosh_motor Boshmotor in Boshmotors)
            {
                Boshmotor.Wheel.motorTorque = 0.0f;
                Boshmotor.Wheel.brakeTorque = 0.0f;
                
                if (Boshmotor.motor)
                {
                    //axleInfo.Wheel.motorTorque = -10.0f;

                    float x, y;
                    x = 0.0f; y = 0.0f;

                    if (Application.platform == RuntimePlatform.Android)
                    {
                        // ���֤Υ��ʥ����ƥ��å����򤭤�ȡ��
                        Vector2 stickL = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);
                        // ���֤Υ��ʥ����ƥ��å����򤭤�ȡ��
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
                            Boshmotor.Wheel.brakeTorque = 0;
                        }
                        else if (Input.GetKey(KeyCode.Space))
                        {
                            Boshmotor.Wheel.brakeTorque = -10.0f * maxMotorTorque;
                        }
                        else
                        {
                            Boshmotor.Wheel.brakeTorque = -3.0f * maxMotorTorque;
                        }
                    }

                    Boshmotor.Wheel.motorTorque = (-y - 2.0f * x) * maxMotorTorque;
                }
                ApplyLocalPositionToVisualsD(Boshmotor.Wheel);
                
        }
        */
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

