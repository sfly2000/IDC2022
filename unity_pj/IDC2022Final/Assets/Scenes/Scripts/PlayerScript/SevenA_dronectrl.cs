using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;
public class SevenA_dronectrl : MonoBehaviour
{

    public PropellersCtrl propellers_ctrl;
    public Rigidbody rig;//drone本身
    public PhotonView photonView;
    public GameObject cam;
    
    public bool is_working;
    public bool is_Blue;
    public bool is_Red;

    //private static float mass = 1.8f+0.2f+4*0.55f;
    public float origin_mass = 1.81f + 0.41f;
    public float now_mass = 1.81f + 0.41f;
    //9.81*weight/4
    public float original_force = 3.2f * 9.81f / 4.0f;
    private Vector3 position_old;
    //private Vector3 velocity_old;
    [System.Serializable]
    public class SetInputKey
    {
        public string InputKeyUp;
        public string InputKeyDown;
        public string InputKeyForward;
        public string InputKeyBackward;
        public string InputKeyRight;
        public string InputKeyLeft;
        public string InputKeyTurnright;
        public string InputKeyTurnleft;
        //切换状态使用 isworking
        public string InputKeyButtton;
        public string InputKeyBlue;
        public string InputKeyRed;
        public string InputKeyCancel;
    }
    public SetInputKey InputKey;

    [System.Serializable]
    public class PumpParameter
    {
        public Rigidbody pump;
        public Rigidbody pump2;
        public float force;
        public axis Axis;
        public string input_pos, input_neg;  //key names of positive and negative input;
        public bool pumpbalance=false;
        public float gain;//增益，使用到PID调节
        public float kp_pump = 0.3f;
        public float ki_pump = 0.1f;
        public float pump_posx;
        public float pump2_posx;
        public float delta_pump;
        public float integral_pump;
        //public bool dir_positive = true;  //pump pushes in positive direction;
        //public PhotonView photonView;
    }
    public PumpParameter Pump;
    private float integral_pump_old = 0f;
    public float Pump_balance(float pump_center, float body_center)
    {
        float pump_gain;
        Pump.delta_pump = pump_center - body_center;
        //Pump.integral_pump= integral_pump_old * 0.99f;
        Pump.integral_pump= integral_pump_old * 0.99f + Pump.delta_pump * Time.deltaTime;
        pump_gain = Pump.kp_pump * Pump.delta_pump + Pump.ki_pump* Pump.integral_pump;
        return pump_gain;

    }
    //private ArticulationBody ab;
    private Vector3 vector;
    private bool is_rigid = true;
    private float dir = 0f;
    private float dir2 = 0f;
    //set targets
    private float[] Target_x = { 19.915f, 19.915f, 19.915f, 19.915f,
        2.5f, 2.5f,
        //2.5f, 2.5f, 2.5f,
        19.915f,  19.915f, 19.915f, 19.915f,
        5.2f,  5.2f, 
        19.915f, 19.915f, 19.915f, 19.915f,
        2.5f, 2.5f
        //2.747f, 2.747f, 2.747f, 2.747f, 2.747f
    };
    private float[] Target_y = { 2.8f, 2.2f, 2.2f, 2.8f,
        2.8f,3f,
        //3.5f, 2.344f, 2.344f,
        2.8f, 2.2f, 2.2f, 2.8f,
        2.8f,3f,
        2.8f, 2.2f, 2.2f, 2.8f,
        2.8f,3f,
    };
    private float[] Target_z = { -5.87f, -5.87f, -5.87f, -5.87f,
        -8.763f,  -8.763f,
        //-8.763f, -8.763f, -8.763f,
        -1.33f,  -1.33f, -1.33f, -1.33f,
        -8.763f, -8.763f,
        4.01f,  4.01f, 4.01f, 4.01f,
        -6.263f,-6.263f
        };
    private float[] Target_pump = { 1f, -1f, 1f, 1f, 
        1f, -1f, 
        //1f, 1f, -1f,-1f,
        1f, -1f, 1f, 1f,
        1f,  -1f,
        1f,  -1f, 1f, 1f,
        1f,  -1f};
    private bool[] Target_pumpbalance = { false, false, true,true,
        true, false,
        //true, true, false,
        false, false,  true,true,
        true, false,
        false, false,  true,true,
        true, false};
    [System.Serializable]
    public class ShowPosForce
    {
        public float posx;
        public float posy;
        public float posz;

        //左前 左后 右前
        public float LFforce = 0f;
        public float LBforce = 0f;
        public float RFforce = 0f;
        public float RBforce = 0f;
    }
    public ShowPosForce PosForce;
    //land:2.5f
    private float[] Target_Blue_x = { -0.048f, 2.7f, 19.915f, 2.7f, 19.915f, 5.2f, 19.915f, 5.2f };
    private float[] Target_Blue_y = {3.5f, 3.5f,3.5f,3.5f, 3.5f, 3.5f, 3.5f,3.5f };
    private float[] Target_Blue_z = { 4.883f, -6.263f, - 5.87f , -8.763f , -1.33f, -8.763f, 4.01f, -6.263f};
    private float[] Target_Red_x = { };
    private float[] Target_Red_y = { };
    private float[] Target_Red_z = { };

    [System.Serializable]
    public class SetTarget
    {
        //Set expected position 
        public float expect_position_x;
        public float expect_position_y = 4f;
        public float expect_position_z;
        public float expect_velocity_x;
        //public float expect_velocity_y;
        public float expect_velocity_z;
    }
    public SetTarget Target;


    //private float position_x_old = 0f;
    private float integral_position_x_old = 0f;

    //private float position_z_old = 0f;
    private float integral_position_z_old = 0f;

    //Set expected velocity 
    private float velocity_x_old = 0f;
    private float integral_acceleration_x_old = 0f;

    private float velocity_z_old = 0f;
    private float integral_acceleration_z_old = 0f;
    //*********************************************************
    //检查是否超调过大,不过大就执行下一目标(x,y,z,pump)
    public int Target_num = 0;
    public bool cflag = false;
    public void change_target(float pos_x, float pos_y, float pos_z, float vel_x, float vel_z)//float vel_x, float vel_y, float vel_z,float pump_state
    {

        //float res = 0;
        //已经达到目标点附近,且平稳不快速
        if (Target_num != Target_x.Length - 1 && System.Math.Abs(pos_x - Target.expect_position_x) < 0.12
            && System.Math.Abs(pos_z - Target.expect_position_z) < 0.12 && System.Math.Abs(vel_x) < 0.05 && System.Math.Abs(vel_z) < 0.05)//&& System.Math.Abs(vel_x) < 0.1
        {
            ////如果气泵为打开状态，就不考虑因为树升不上去，需要让y也达到误差范围
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




    //上交时去掉
    //static FileStream File_write = new FileStream(@"D:\MCU\Unity\projects\IDC robocon1\test2.txt", FileMode.OpenOrCreate);
    //StreamWriter sw = new StreamWriter(SevenA_dronectrl.File_write);
    private int count = 0;
    //***********************************************************************************************************************************************************
    //the paramters are for PID control



    //****************************************************************************************************************************
    // for dip control
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


    [System.Serializable]
    public class DipControlParameter
    {
        public float vx_now;
        public float vz_now;
        public float ax_now;
        public float az_now;
        public float set_vx = 8f;//5
        public float set_vz = 8f;//5
        public float kp_v = 5f;
        public float ki_v = 0f;//0f
        public float kp_dip = 0.25f;
        public float ki_dip = 0.1f;//0.1f
        public float kd_dip = 0.01f;
        public float want_acc = 5f;
        public float want_vel = 8f;
    }
    public DipControlParameter DipControl;



    private float[] accx_x = new float[4];
    private float[] accx_y = new float[3];
    private float[] accz_x = new float[4];
    private float[] accz_y = new float[3];

    //Parameter for dip control
    private float delta_position_z_old = 0f;
    private float delta_position_x_old = 0f;
    private float rotation_x_old = 0f;
    private float rotation_z_old = 0f;
    private float integral_wx_old = 0f;
    private float integral_wz_old = 0f;
    private float delta_wx_old = 0f;
    private float delta_wz_old = 0f;

    private float[] diff_wx_x = new float[4];
    private float[] diff_wx_y = new float[3];
    private float[] diff_wz_x = new float[4];
    private float[] diff_wz_y = new float[3];

    //****************************************************************************************************************************
    // for horizontal rotation control

    [System.Serializable]
    public class HorizontalRotationControlParamter
    {
        public float set_eular_y = 0f;
        public float set_wy = 25f;
        public float kp_wy = 0.07f;
        public float ki_wy = 0.01f;
        public float kd_wy = 0.02f;
    }
    public HorizontalRotationControlParamter HorizontalRotationControl;
    private float eular_y_old = 0f;
    private float integral_wy_old = 0f;
    private float delta_wy_old = 0f;
    private float[] diff_wy_x = new float[4];
    private float[] diff_wy_y = new float[3];

    //****************************************************************************************************************************
    // for height control
    [System.Serializable]
    //高度控制
    public class HeightControlParameter
    {
        //public float set_height = 4f;
        public float set_vy = 4f;
        public float kp_vy = 0.18f;
        public float ki_vy = 0.02f;
        public float kd_vy = 0.05f;
    }

    public HeightControlParameter HeightControl;
    private float integral_vy_old = 0f;
    private float delta_vy_old = 0f;
    private float diff_vy_old = 0f;
    private float[] hx = new float[4];
    private float[] hy = new float[3];


    //****************************************************************************************************************************
    //Parameter for Filter
    private class FilterParamter
    {
        //you can set your filter parameter there
        public float[] px1 = { 0.00475f, 0.01425f, 0.01425f, 0.00475f };
        public float[] py1 = { -2.25f, 1.756f, -0.4683f };
        public float[] px2 = { 0.0002196f, 0.00006588f, 0.0006588f, 0.0002196f };
        public float[] py2 = { -2.7488f, 2.5282f, -0.7776f };

        public float Filter(float[] Input_x, float[] Input_y, float[] parameter_x, float[] parameter_y)
        {
            float res = 0;
            if (Input_x.Length != parameter_x.Length || Input_y.Length != parameter_y.Length)
            {
                return res;
            }
            for (int i = 0; i < Input_x.Length; i++)
            {
                res += Input_x[i] * parameter_x[i];
            }
            for (int i = 0; i < Input_y.Length; i++)
            {
                res -= Input_y[i] * parameter_y[i];
            }
            return res;
        }
    }

    private FilterParamter filter;





    //****************************************************************************************************************************
    //Write some paramter into the file for testing
    private StreamWriter streamWriter_rotation_x;
    private StreamWriter streamWriter_rotation_z;
    private StreamWriter streamWriter_vx;
    private StreamWriter streamWriter_vz;
    private StreamWriter streamWriter_height;
    private StreamWriter streamWriter_rotation_y;
    private StreamWriter streamWriter_force;

    // Start is called before the first frame update
    void Start()
    {

        count = 0;
        is_working = false;
        is_Blue = false;
        is_Red = false;
        //一定要
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            HorizontalRotationControl.set_eular_y = rig.transform.eulerAngles.y;
            position_old = rig.transform.position;
            Target.expect_position_x = rig.transform.position.x;
            Target.expect_position_y = 3.5f;
            Target.expect_position_z = rig.transform.position.z;
            Target.expect_velocity_x = 0f;
            Target.expect_velocity_z = 0f;
            Pump.pumpbalance = false;
            //Target.expect_position_x = Target_x[Target_num];
            //Target.expect_position_y = Target_y[Target_num];
            //Target.expect_position_z = Target_z[Target_num];
            //dir = Target_pump[Target_num];
            //Pump.pumpbalance = Target_pumpbalance[Target_num];
            //velocity_old = rig.transform.position;
            rotation_x_old = rig.transform.eulerAngles.x;
            rotation_z_old = rig.transform.eulerAngles.z;
            eular_y_old = rig.transform.eulerAngles.y;
            filter = new FilterParamter();
            cam.SetActive(true);
            //Pump Parameter
            if (GetComponent<Rigidbody>() != null)
            {
                //Pump.pump = GetComponent<Rigidbody>();
                //Pump.pump2 = GetComponent<Rigidbody>();
                is_rigid = true;
            }
            else
            {
                //ab = GetComponent<ArticulationBody>();
                is_rigid = false;
            }
            switch (Pump.Axis)
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
            dir = 1f;
            dir2 = dir * -1;
        }
        else
        {
            Debug.Log("photonView.IsMine == False");
            cam.SetActive(false);
        }
    }

    private void Update()//update跟当前平台的帧数有关，而FixedUpdate是真实时间，
                         //所以处理物理逻辑的时候要把代码放在FixedUpdate而不是Update.
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {//有按键按下就调整工作状态
            if (Input.GetKeyDown(InputKey.InputKeyButtton))
            {
                is_working = !is_working;
            }

            
            else if (Input.GetKeyDown(InputKey.InputKeyBlue))
            {
                is_Blue = true;
                is_Red = false;
                //记录当前所在位置为目标点，即开始时先控制住位置
                Target.expect_position_x = rig.transform.position.x;
                Target.expect_position_y = rig.transform.position.y;
                Target.expect_position_z = rig.transform.position.z;
            }
            else if (Input.GetKeyDown(InputKey.InputKeyRed))
            {
                is_Blue = false;
                is_Red = true;
                Target.expect_position_x = rig.transform.position.x;
                Target.expect_position_y = rig.transform.position.y;
                Target.expect_position_z = rig.transform.position.z;
            }
            else if (Input.GetKeyDown(InputKey.InputKeyCancel))
            {
                is_Blue = false;
                is_Red = false;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //抓树的情况下，需要让mass和origin_force改变
        //if (Pump.pumpbalance)
        //{
        //    now_mass = origin_mass + 0.5f;
        //    original_force = now_mass * 9.81f / 4f;
        //}
        ////不在抓树时
        //else
        //{
        //    now_mass = origin_mass;
        //    original_force = now_mass * 9.81f / 4f;
        //}
        count += 1;
        PosForce.posx = rig.transform.position.x;
        PosForce.posy = rig.transform.position.y;
        PosForce.posz = rig.transform.position.z;
        Pump.pump_posx = Pump.pump.transform.position.x;
        Pump.pump2_posx = Pump.pump2.transform.position.x;

        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            //****************************************************************************************************************************
            //on\off      
            if (!is_working)
            {
                PosForce.LFforce = 0f;
                PosForce.LBforce = 0f;
                PosForce.RFforce = 0f;
                PosForce.RBforce = 0f;

                propellers_ctrl.SetForce(PosForce.LFforce, PosForce.LBforce, PosForce.RFforce, PosForce.RBforce);

                return;
            }
            else
            {
                //先初始化力量大小
                PosForce.LFforce = original_force;
                PosForce.LBforce = original_force;
                PosForce.RFforce = original_force;
                PosForce.RBforce = original_force;
            }
            //Pump

            if (Input.GetKey(Pump.input_pos))
            {
                dir = 1;
            }
            else if (Input.GetKey(Pump.input_neg))
            {
                dir = -1;
            }
            //if (!Pump.dir_positive)
            //{
            //    dir *= -1;
            //}
            dir2 = dir * -1;
            
            if (is_rigid)
            {
                if (Pump.pumpbalance)
                {
                    Pump.gain = Pump_balance((Pump.pump_posx + Pump.pump2_posx) / 2f, PosForce.posx);
                }
                else
                {
                    Pump.gain = 0;
                    integral_pump_old = 0;
                }
                //rb.AddRelativeForce(vector * Pump.force * dir);
                //Pump.pump.AddRelativeForce(vector * Pump.force * dir * (1 - Pump.gain));
                //Pump.pump2.AddRelativeForce(vector * Pump.force * dir * (1 + Pump.gain));
            }
            //****************************************************************************************************************************
            //Revise the set parameter according to the input key
            float wantedRotation_x = 0;
            float wantedRotation_z = 0;

            //如果按键，要退出程序控制
            //高度上可以控制为位置式
            //高度方面是自己控速度为2m/s
            if (Input.GetKey(InputKey.InputKeyUp))
            {
                //is_Blue = false;
                //is_Red = false;
                //Target.expect_position_y = PosForce.posy;
                Target.expect_position_y += HeightControl.set_vy * Time.deltaTime;
            }

            else if (Input.GetKey(InputKey.InputKeyDown))
            {
                //is_Blue = false;
                //is_Red = false;
                // Target.expect_position_y = PosForce.posy;
                Target.expect_position_y -= HeightControl.set_vy * Time.deltaTime;
                //不能到0
                Target.expect_position_y = Mathf.Max(Target.expect_position_y, 0);
            }
            //程序控制时，z或x上可以控制为位置式，便于微调抓物体
            if (is_Blue || is_Red)
            {
                
                if (Input.GetKey(InputKey.InputKeyRight))
                {
                    //is_Blue = false;
                    //is_Red = false;

                    //Target.expect_position_x = PosForce.posx;
                    Target.expect_position_x += DipControl.set_vx * Time.deltaTime;
                    Target.expect_position_x = Mathf.Min(Target.expect_position_x, 23f);
                }

                else if (Input.GetKey(InputKey.InputKeyLeft))
                {
                    //is_Blue = false;
                    //is_Red = false;
                    //Target.expect_position_x = PosForce.posx;
                    Target.expect_position_x -= DipControl.set_vx * Time.deltaTime;
                    //修改
                    //不能到
                    Target.expect_position_x = Mathf.Max(Target.expect_position_x, -23f);
                }
                if (Input.GetKey(InputKey.InputKeyForward))
                {
                    //is_Blue = false;
                    //is_Red = false;
                    //Target.expect_position_z = PosForce.posz;
                    Target.expect_position_z += DipControl.set_vz * Time.deltaTime;
                    Target.expect_position_z = Mathf.Min(Target.expect_position_z, 12f);
                }

                else if (Input.GetKey(InputKey.InputKeyBackward))
                {
                    //is_Blue = false;
                    //is_Red = false;
                    //Target.expect_position_z = PosForce.posz;
                    Target.expect_position_z -= DipControl.set_vz * Time.deltaTime;
                    //修改
                    //不能到
                    Target.expect_position_z = Mathf.Max(Target.expect_position_z, -12f);
                }
            }
            //手动控制时，z或x上可以控制为速度式，便于追逐和避免特殊情况
            else
            {
                if (Input.GetKey(InputKey.InputKeyForward))
                {
                    Target.expect_velocity_z = Mathf.Min(Target.expect_velocity_z + DipControl.want_acc * Time.deltaTime, DipControl.set_vz);
                }
                else if (Input.GetKey(InputKey.InputKeyBackward))
                {
                    Target.expect_velocity_z = Mathf.Max(Target.expect_velocity_z - DipControl.want_acc * Time.deltaTime, -DipControl.set_vz);
                }
                else
                {
                    if (Target.expect_velocity_z > 0)
                    {
                        Target.expect_velocity_z = Mathf.Max(Target.expect_velocity_z - DipControl.want_acc * Time.deltaTime, 0);
                    }
                    else if (Target.expect_velocity_z < 0)
                    {
                        Target.expect_velocity_z = Mathf.Min(Target.expect_velocity_z + DipControl.want_acc * Time.deltaTime, 0);
                    }

                }
                //左右运动
                if (Input.GetKey(InputKey.InputKeyRight))
                {
                    //PosForce.LFforce += delta_force;
                    //PosForce.LBforce += delta_force;
                    //PosForce.RFforce += -delta_force;
                    //PosForce.RBforce += -delta_force;
                    Target.expect_velocity_x = Mathf.Min(Target.expect_velocity_x + DipControl.want_acc * Time.deltaTime, DipControl.set_vx);
                }
                else if (Input.GetKey(InputKey.InputKeyLeft))
                {
                    //PosForce.LFforce += -delta_force;
                    //PosForce.LBforce += -delta_force;
                    //PosForce.RFforce += delta_force;
                    //PosForce.RBforce += delta_force;
                    Target.expect_velocity_x = Mathf.Max(Target.expect_velocity_x - DipControl.want_acc * Time.deltaTime, -DipControl.set_vx);
                }
                else
                {
                    if (Target.expect_velocity_x > 0)
                    {
                        Target.expect_velocity_x = Mathf.Max(Target.expect_velocity_x - DipControl.want_acc * Time.deltaTime, 0);
                    }
                    else if (Target.expect_velocity_x < 0)
                    {
                        Target.expect_velocity_x = Mathf.Min(Target.expect_velocity_x + DipControl.want_acc * Time.deltaTime, 0);
                    }
                }
            }
           
            //水平面旋转的，先不管
            if (Input.GetKey(InputKey.InputKeyTurnleft))
            {
                //is_Blue = false;
                //is_Red = false;
                HorizontalRotationControl.set_eular_y -= HorizontalRotationControl.set_wy * Time.deltaTime;
            }
            else if (Input.GetKey(InputKey.InputKeyTurnright))
            {
                //is_Blue = false;
                //is_Red = false;
                HorizontalRotationControl.set_eular_y += HorizontalRotationControl.set_wy * Time.deltaTime;
            }

            if (is_Blue)
            {
                //bonus tree
                if (Input.GetKey(KeyCode.Alpha9))
                {
                    Target_num = 0;
                    Target.expect_position_x = Target_Blue_x[Target_num];
                    Target.expect_position_y = Target_Blue_y[Target_num];
                    Target.expect_position_z = Target_Blue_z[Target_num];
                    now_mass = origin_mass;
                    original_force = now_mass * 9.81f / 4f;
                }
                //bonus tree land
                else if (Input.GetKey(KeyCode.Alpha0))
                {
                    Target_num = 1;
                    Target.expect_position_x = Target_Blue_x[Target_num];
                    Target.expect_position_y = Target_Blue_y[Target_num];
                    Target.expect_position_z = Target_Blue_z[Target_num];
                    now_mass = origin_mass + 0.5f;
                    original_force = now_mass * 9.81f / 4f;
                }
                //first tree
                else if (Input.GetKey(KeyCode.Alpha1))
                {
                    Target_num = 2;
                    Target.expect_position_x = Target_Blue_x[Target_num];
                    Target.expect_position_y = Target_Blue_y[Target_num];
                    Target.expect_position_z = Target_Blue_z[Target_num];
                    now_mass = origin_mass;
                    original_force = now_mass * 9.81f / 4f;
                }
                //first tree land
                else if (Input.GetKey(KeyCode.Alpha2))
                {
                    Target_num = 3;
                    Target.expect_position_x = Target_Blue_x[Target_num];
                    Target.expect_position_y = Target_Blue_y[Target_num];
                    Target.expect_position_z = Target_Blue_z[Target_num];
                    now_mass = origin_mass + 0.5f;
                    original_force = now_mass * 9.81f / 4f;
                }
                //2 tree
                else if (Input.GetKey(KeyCode.Alpha3))
                {
                    Target_num = 4;
                    Target.expect_position_x = Target_Blue_x[Target_num];
                    Target.expect_position_y = Target_Blue_y[Target_num];
                    Target.expect_position_z = Target_Blue_z[Target_num];
                    now_mass = origin_mass;
                    original_force = now_mass * 9.81f / 4f;
                }
                //2 tree land
                else if (Input.GetKey(KeyCode.Alpha4))
                {
                    Target_num = 5;
                    Target.expect_position_x = Target_Blue_x[Target_num];
                    Target.expect_position_y = Target_Blue_y[Target_num];
                    Target.expect_position_z = Target_Blue_z[Target_num];
                    now_mass = origin_mass + 0.5f;
                    original_force = now_mass * 9.81f / 4f;
                }
                //3 tree
                else if (Input.GetKey(KeyCode.Alpha5))
                {
                    Target_num = 6;
                    Target.expect_position_x = Target_Blue_x[Target_num];
                    Target.expect_position_y = Target_Blue_y[Target_num];
                    Target.expect_position_z = Target_Blue_z[Target_num];
                    now_mass = origin_mass;
                    original_force = now_mass * 9.81f / 4f;
                }
                //3 tree land
                else if (Input.GetKey(KeyCode.Alpha6))
                {
                    Target_num = 7;
                    Target.expect_position_x = Target_Blue_x[Target_num];
                    Target.expect_position_y = Target_Blue_y[Target_num];
                    Target.expect_position_z = Target_Blue_z[Target_num];
                    now_mass = origin_mass + 0.5f;
                    original_force = now_mass * 9.81f / 4f;
                }
            }
            else if (is_Red)
            {
                //bonus
                if (Input.GetKey(KeyCode.Alpha0))
                {
                    Target_num = 0;
                    Target.expect_position_x = Target_Red_x[Target_num];
                    Target.expect_position_y = Target_Red_y[Target_num];
                    Target.expect_position_z = Target_Red_z[Target_num];
                }
                //first tree
                else if (Input.GetKey(KeyCode.Alpha1))
                {
                    Target_num = 1;
                    Target.expect_position_x = Target_Red_x[Target_num];
                    Target.expect_position_y = Target_Red_y[Target_num];
                    Target.expect_position_z = Target_Red_z[Target_num];
                }
                //first tree land
                else if (Input.GetKey(KeyCode.Alpha2))
                {
                    Target_num = 2;
                    Target.expect_position_x = Target_Red_x[Target_num];
                    Target.expect_position_y = Target_Red_y[Target_num];
                    Target.expect_position_z = Target_Red_z[Target_num];
                }
                //2 tree
                else if (Input.GetKey(KeyCode.Alpha3))
                {
                    Target_num = 3;
                    Target.expect_position_x = Target_Red_x[Target_num];
                    Target.expect_position_y = Target_Red_y[Target_num];
                    Target.expect_position_z = Target_Red_z[Target_num];
                }
                //2 tree land
                else if (Input.GetKey(KeyCode.Alpha4))
                {
                    Target_num = 4;
                    Target.expect_position_x = Target_Red_x[Target_num];
                    Target.expect_position_y = Target_Red_y[Target_num];
                    Target.expect_position_z = Target_Red_z[Target_num];
                }
                //3 tree
                else if (Input.GetKey(KeyCode.Alpha5))
                {
                    Target_num = 5;
                    Target.expect_position_x = Target_Red_x[Target_num];
                    Target.expect_position_y = Target_Red_y[Target_num];
                    Target.expect_position_z = Target_Red_z[Target_num];
                }
                //3 tree land
                else if (Input.GetKey(KeyCode.Alpha6))
                {
                    Target_num = 6;
                    Target.expect_position_x = Target_Red_x[Target_num];
                    Target.expect_position_y = Target_Red_y[Target_num];
                    Target.expect_position_z = Target_Red_z[Target_num];
                }
            }
            //****************************************************************************************************************************
            //Set the dip according to the velocity now
            //******************************
            //一层PI
            //position on z axis(forward and backward)
            //位置式
            if (is_Blue || is_Red)
            {
                PosControl.delta_position_z = (Target.expect_position_z - rig.transform.position.z);
                PosControl.delta_position_z = Mathf.Clamp(PosControl.delta_position_z, -8, 8);
                //posz的累积值
                PosControl.integral_position_z = integral_position_z_old * 0.99f + PosControl.delta_position_z * Time.deltaTime;
                PosControl.diff_pz = Mathf.Clamp((PosControl.delta_position_z - delta_position_z_old) / Time.deltaTime, -1, 1);

                Target.expect_velocity_z = PosControl.kp_p * PosControl.delta_position_z + PosControl.kd_p * PosControl.diff_pz + PosControl.ki_p * PosControl.integral_position_z;
                Target.expect_velocity_z = Mathf.Clamp(Target.expect_velocity_z, -4, 4);
                //velocity_z_old = velocity_z_now;
                integral_position_z_old = PosControl.integral_position_z;
                delta_position_z_old = PosControl.delta_position_z;
            }



            //******************************
            //velocity on z axis(forward and backward)
            //一层P
            //计算当前速度
            //因为考虑旋转，所以才要用transform的方式
            float velocity_z_now = ((rig.transform.position.z - position_old.z) * Mathf.Cos(rig.transform.eulerAngles.y * Mathf.Deg2Rad)) / Time.deltaTime;
            velocity_z_now += ((rig.transform.position.x - position_old.x) * Mathf.Sin(rig.transform.eulerAngles.y * Mathf.Deg2Rad)) / Time.deltaTime;
            DipControl.vz_now = velocity_z_now;
            //cal the parameter for pid control
            //计算目前期望加速度
            float des_acceleration_z = Mathf.Clamp(1.2f * (Target.expect_velocity_z - velocity_z_now), -5, 5);



            //一层PI
            //两帧之间的速度差/帧时间=当前加速度
            float acc_z_now = (velocity_z_now - velocity_z_old) / Time.deltaTime;
            DipControl.az_now = acc_z_now;

            for (int i = accz_x.Length - 1; i > 0; i--)
            {
                accz_x[i] = accz_x[i - 1];
            }
            accz_x[0] = acc_z_now;


            //切比雪夫滤波器
            float accz_y_out = filter.Filter(accz_x, accz_y, filter.px1, filter.py1);
            //y就是滤波后的accz, x是滤波前的
            //accz_y.Length=4,也就是说仅保留4帧的
            for (int i = accz_y.Length - 1; i > 0; i--)
            {
                accz_y[i] = accz_y[i - 1];
            }
            //滤波后的实际加速度
            accz_y[0] = accz_y_out;
            //排越先代表越后获得。把最后的那个会挤走
            //加速度微分值
            float delta_acceleration_z = des_acceleration_z - accz_y_out;
            //accz的累积值
            float integral_acceleration_z = integral_acceleration_z_old * 0.99f + delta_acceleration_z * Time.deltaTime;

            velocity_z_old = velocity_z_now;
            integral_acceleration_z_old = integral_acceleration_z;


            //****************************************************************************************************************************
            //Set the dip according to the velocity now
            //******************************
            //一层PI
            //position on z axis(forward and backward)
            //位置式
            if (is_Blue || is_Red)
            {
                PosControl.delta_position_x = (Target.expect_position_x - rig.transform.position.x);
                PosControl.delta_position_x = Mathf.Clamp(PosControl.delta_position_x, -8, 8);
                //posz的累积值
                PosControl.integral_position_x = integral_position_x_old * 0.99f + PosControl.delta_position_x * Time.deltaTime;
                PosControl.diff_px = Mathf.Clamp((PosControl.delta_position_x - delta_position_x_old) / Time.deltaTime, -1, 1);

                Target.expect_velocity_x = PosControl.kp_p * PosControl.delta_position_x + PosControl.kd_p * PosControl.diff_px + PosControl.ki_p * PosControl.integral_position_x;
                Target.expect_velocity_x = Mathf.Clamp(Target.expect_velocity_x, -4, 4);
                //velocity_z_old = velocity_z_now;
                integral_position_x_old = PosControl.integral_position_x;
                delta_position_x_old = PosControl.delta_position_x;
            }


            //******************************
            //velocity on x axis
            float velocity_x_now = -((rig.transform.position.z - position_old.z) * Mathf.Sin(rig.transform.eulerAngles.y * Mathf.Deg2Rad)) / Time.deltaTime;
            velocity_x_now += ((rig.transform.position.x - position_old.x) * Mathf.Cos(rig.transform.eulerAngles.y * Mathf.Deg2Rad)) / Time.deltaTime;
            DipControl.vx_now = velocity_x_now;
            //cal the parameter for pid control
            //不知道怎么来的1.2?,可能考虑了风阻
            float des_acceleration_x = Mathf.Clamp(1.2f * (Target.expect_velocity_x - velocity_x_now), -5, 5);

            float acc_x_now = (velocity_x_now - velocity_x_old) / Time.deltaTime;
            //writeline
            DipControl.ax_now = acc_x_now;
            //分别当前的p v a
            //sw.WriteLine(count.ToString() + "," + rig.transform.position.x.ToString() + "," + rig.transform.position.z.ToString() + "," + DipControl.vx_now.ToString() + "," + DipControl.vz_now.ToString() + "," + DipControl.ax_now.ToString() + "," + DipControl.az_now.ToString());
            for (int i = accx_x.Length - 1; i > 0; i--)
            {
                accx_x[i] = accx_x[i - 1];
            }
            accx_x[0] = acc_x_now;

            //滤波（这里本质上是把变化大的滤除了）后加速度为输出，加速度为输入，将输入序列和输出序列代入
            float accx_y_out = filter.Filter(accx_x, accx_y, filter.px1, filter.py1);
            for (int i = accx_y.Length - 1; i > 0; i--)
            {
                accx_y[i] = accx_y[i - 1];
            }
            accx_y[0] = accx_y_out;


            //滤波出来的当前加速度为accx_y_out
            float delta_acceleration_x = des_acceleration_x - accx_y_out;

            float integral_acceleration_x = integral_acceleration_x_old * 0.99f + delta_acceleration_x * Time.deltaTime;

            velocity_x_old = velocity_x_now;
            integral_acceleration_x_old = integral_acceleration_x;


            //****************************************************************************************************************************
            //Set the expected dip，最大正负30度，希望的倾角
            wantedRotation_x += DipControl.kp_v * delta_acceleration_z + DipControl.ki_v * integral_acceleration_z;
            wantedRotation_x = Mathf.Clamp(wantedRotation_x, -30f, +30f);
            wantedRotation_z -= DipControl.kp_v * delta_acceleration_x + DipControl.ki_v * integral_acceleration_x;
            wantedRotation_z = Mathf.Clamp(wantedRotation_z, -30f, +30f);

            //****************************************************************************************************************************
            //dip control
            //加速度推导出角速度所需，角速度和升降力有关系
            //计算倾角上还需要多少差值量，返回的是°为单位
            float delta_rotation_x = Mathf.DeltaAngle(rig.transform.eulerAngles.x, wantedRotation_x);
            float delta_rotation_z = Mathf.DeltaAngle(rig.transform.eulerAngles.z, wantedRotation_z);
            //0.08?
            //目标角速度
            float des_wx = Mathf.Clamp(0.08f * delta_rotation_x, -3f, 3f);
            float des_wz = Mathf.Clamp(0.08f * delta_rotation_z, -3f, 3f);
            //当前角速度
            float wx_now = 0.08f * Mathf.DeltaAngle(rotation_x_old, rig.transform.eulerAngles.x) / Time.deltaTime;
            float wz_now = 0.08f * Mathf.DeltaAngle(rotation_z_old, rig.transform.eulerAngles.z) / Time.deltaTime;
            //角速度差值，也就是
            float delta_wx = des_wx - 0.08f * Mathf.DeltaAngle(rotation_x_old, rig.transform.eulerAngles.x) / Time.deltaTime;
            float delta_wz = des_wz - 0.08f * Mathf.DeltaAngle(rotation_z_old, rig.transform.eulerAngles.z) / Time.deltaTime;
            //角速度的积分
            float integral_wx = integral_wx_old * 0.99f + delta_wx * Time.deltaTime;
            float integral_wz = integral_wz_old * 0.99f + delta_wz * Time.deltaTime;
            //角速度的微分
            float diff_wx = Mathf.DeltaAngle(delta_wx_old, delta_wx) / Time.deltaTime;
            float diff_wz = Mathf.DeltaAngle(delta_wz_old, delta_wz) / Time.deltaTime;
            rotation_x_old = rig.transform.eulerAngles.x;
            rotation_z_old = rig.transform.eulerAngles.z;
            integral_wx_old = integral_wx;
            integral_wz_old = integral_wz;
            delta_wx_old = delta_wx;
            delta_wz_old = delta_wz;

            for (int i = diff_wx_x.Length - 1; i > 0; i--)
            {
                diff_wx_x[i] = diff_wx_x[i - 1];
                diff_wz_x[i] = diff_wz_x[i - 1];
            }
            diff_wx_x[0] = diff_wx;
            diff_wz_x[0] = diff_wz;
            float diff_wx_out = filter.Filter(diff_wx_x, diff_wx_y, filter.px1, filter.py1);
            float diff_wz_out = filter.Filter(diff_wz_x, diff_wz_y, filter.px1, filter.py1);

            float col_rotation_x = DipControl.kp_dip * delta_wx + DipControl.ki_dip * integral_wx + DipControl.kd_dip * diff_wx_out;
            float col_rotation_z = DipControl.kp_dip * delta_wz + DipControl.ki_dip * integral_wz + DipControl.kd_dip * diff_wz_out;

            col_rotation_x *= original_force;
            col_rotation_z *= original_force;

            for (int i = diff_wx_y.Length - 1; i > 0; i--)
            {
                diff_wx_y[i] = diff_wx_y[i - 1];
                diff_wz_y[i] = diff_wz_y[i - 1];
            }
            diff_wx_y[0] = diff_wx_out;
            diff_wz_y[0] = diff_wz_out;


            PosForce.LFforce += -col_rotation_x - col_rotation_z;
            PosForce.LBforce += +col_rotation_x - col_rotation_z;
            PosForce.RFforce += -col_rotation_x + col_rotation_z;
            PosForce.RBforce += +col_rotation_x + col_rotation_z;

            //****************************************************************************************************************************
            //如果代码控制再考虑更改目标
            //if (is_Blue || is_Red)
            //{
            //    change_target(PosForce.posx, PosForce.posy, PosForce.posz, DipControl.vx_now, DipControl.vz_now);
            //    if (cflag)
            //    {
            //        cflag = false;
            //        Target.expect_position_x = Target_x[Target_num];
            //        Target.expect_position_y = Target_y[Target_num];
            //        Target.expect_position_z = Target_z[Target_num];
            //        dir = Target_pump[Target_num];
            //        Pump.pumpbalance = Target_pumpbalance[Target_num];
            //    }
            //}
            

            //****************************************************************************************************************************
            //rotation control(on y axis)
            float delta_eular_y = Mathf.DeltaAngle(rig.transform.eulerAngles.y, HorizontalRotationControl.set_eular_y);
            float des_wy = Mathf.Clamp(0.6f * delta_eular_y, -30f, 30f);
            float real_wy = Mathf.DeltaAngle(eular_y_old, rig.transform.eulerAngles.y) / Time.deltaTime;
            float delta_wy = Mathf.Clamp(des_wy - real_wy, -8f, 8f);
            float integral_wy = integral_wy_old * 0.95f + des_wy * Time.deltaTime;
            float diff_wy = Mathf.Clamp((delta_wy - delta_wy_old) / Time.deltaTime, -20f, 20f);


            for (int i = diff_wy_x.Length - 1; i > 0; i--)
            {
                diff_wy_x[i] = diff_wy_x[i - 1];
            }
            diff_wy_x[0] = diff_wy;

            float diff_wy_out = filter.Filter(diff_wy_x, diff_wy_y, filter.px2, filter.py2);

            float col_out_wy = HorizontalRotationControl.kp_wy * delta_wy + HorizontalRotationControl.ki_wy * integral_wy + HorizontalRotationControl.kd_wy * diff_wy_out;
            col_out_wy *= original_force;

            eular_y_old = rig.transform.eulerAngles.y;
            integral_wy_old = integral_wy;
            delta_wy_old = delta_wy;


            for (int i = diff_wy_y.Length - 1; i > 0; i--)
            {
                diff_wy_y[i] = diff_wy_y[i - 1];
            }
            diff_wy_y[0] = diff_wy_out;

            PosForce.LFforce += col_out_wy;
            PosForce.LBforce -= col_out_wy;
            PosForce.RFforce -= col_out_wy;
            PosForce.RBforce += col_out_wy;

            //****************************************************************************************************************************
            //height
            float delta_height = Target.expect_position_y - rig.transform.position.y;
            float des_vy = Mathf.Clamp(0.8f * delta_height, -5, 5);
            float delta_vy = Mathf.Clamp(des_vy - (rig.transform.position.y - position_old.y) / Time.deltaTime, -5, 5);
            float integral_vy = integral_vy_old * 0.98f + delta_vy * Time.deltaTime;
            float diff_vy = Mathf.Clamp((delta_vy - delta_vy_old) / Time.deltaTime, -10, 10);

            for (int i = hx.Length - 1; i > 0; i--)
            {
                hx[i] = hx[i - 1];
            }
            hx[0] = diff_vy;

            float diff_vy_out = filter.Filter(hx, hy, filter.px2, filter.py2);

            float col_out_vy = HeightControl.kp_vy * delta_vy + HeightControl.ki_vy * integral_vy + HeightControl.kd_vy * diff_vy_out;
            col_out_vy *= original_force;

            position_old = rig.transform.position;
            delta_vy_old = delta_vy;
            integral_vy_old = integral_vy;
            diff_vy_old = diff_vy;

            for (int i = hy.Length - 1; i > 0; i--)
            {
                hy[i] = hy[i - 1];
            }
            hy[0] = diff_vy_out;

            PosForce.LFforce += col_out_vy;
            PosForce.LBforce += col_out_vy;
            PosForce.RFforce += col_out_vy;
            PosForce.RBforce += col_out_vy;

            //****************************************************************************************************************************
            //set force
            //要在相应区间，进行限幅
            PosForce.LFforce = Mathf.Clamp(PosForce.LFforce, 0f, 20f);
            PosForce.LBforce = Mathf.Clamp(PosForce.LBforce, 0f, 20f);
            PosForce.RFforce = Mathf.Clamp(PosForce.RFforce, 0f, 20f);
            PosForce.RBforce = Mathf.Clamp(PosForce.RBforce, 0f, 20f);

            propellers_ctrl.SetForce(PosForce.LFforce, PosForce.LBforce, PosForce.RFforce, PosForce.RBforce);

        }

    }

    void OnApplicationQuit()
    {
        //sw.Close();
    }


}