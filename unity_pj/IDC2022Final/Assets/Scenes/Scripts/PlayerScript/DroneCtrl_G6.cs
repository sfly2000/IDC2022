using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DroneCtrl_G6 : MonoBehaviour
{
    public PropellersCtrl propellers_ctrl;
    public Rigidbody rig;
    public PhotonView photonView;
    public GameObject cam;

    //public Vector3 position_target = Vector3.new(0f, 10f, 0f);
    public float DistenceSlow = 5f ;
    public float MaxVSlow = 0.4f ;

    public bool is_working;
    public float original_force = 4.5f;
    public float rotationLimit = 45.0f;

    //
        
    [System.Serializable]
    public class SetTarget
    {
        public float target0_x = 0;
        public float target0_y = 2;
        public float target0_z = -6.2f;

        public float target1_x = -20;
        public float target1_y = 2;
        public float target1_z = -6.2f;

        public float target2_x = -20;
        public float target2_y = 2;
        public float target2_z = -1.2f;

        public float target3_x = -20;
        public float target3_y = 2;
        public float target3_z = 3.8f;
    }
    public SetTarget target;
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
        public string InputKeyButtton;
        public string InputKey1;
        public string InputKey2;
        public string InputKey3;
        public string InputKey4;
        public string InputKey5;
        public string InputKey6;
        public string InputKey7;
        public string InputKey8;
    }
    public SetInputKey InputKey;

    //***********************************************************************************************************************************************************
    //the paramters are for PID control

    private Vector3 position_old;
    private float LFforce = 0f;
    private float LBforce = 0f;
    private float RFforce = 0f;
    private float RBforce = 0f;

    //****************************************************************************************************************************
    // for dip control

    [System.Serializable]
    public class DipControlParameter
    {
        public float set_vx = 4;
        public float set_vz = 2;
        public float kp_v = 5f;
        public float ki_v = 0f;
        public float kp_dip = 0.4f;
        public float ki_dip = 0.1f;
        public float kd_dip = 0.2f;
    }
    public DipControlParameter DipControl;

    //Set expected velocity 
    private float expect_velocity_z;
    private float velocity_z_old = 0f;
    private float integral_acceleration_z_old = 0f;

    private float expect_velocity_x;
    private float velocity_x_old = 0f;
    private float integral_acceleration_x_old = 0f;

    private float[] accx_x = new float[4];
    private float[] accx_y = new float[3];
    private float[] accz_x = new float[4];
    private float[] accz_y = new float[3];

    //Parameter for dip control
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
    public class HeightControlParameter
    {
        public float set_height = 3f;
        public float set_vy = 2f;
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
        is_working = false;
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            HorizontalRotationControl.set_eular_y = rig.transform.eulerAngles.y;
            position_old = rig.transform.position;
            rotation_x_old = rig.transform.eulerAngles.x;
            rotation_z_old = rig.transform.eulerAngles.z;
            eular_y_old = rig.transform.eulerAngles.y;
            filter = new FilterParamter();
            cam.SetActive(true);
        }
        else
        {
            Debug.Log("photonView.IsMine == False");
            cam.SetActive(false);
        }

        //Set the correct target based on the starting area.
        if(this.transform.localPosition.x > 0)
        {
            target.target0_x = 0;
            target.target1_x = +20;
            target.target2_x = +20;
            target.target3_x = +20;
        }
        else
        {
            target.target0_x = 0;
            target.target1_x = -20;
            target.target2_x = -20;
            target.target3_x = -20;
        }
    }

    private void Update()
    {
        //print("X坐标为：" + this.transform.localPosition.x);

        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            if (Input.GetKeyDown(InputKey.InputKeyButtton))
            {
                is_working = !is_working;
            }

            /*
            if (Input.GetKeyDown(KeyCode.T))
            {
                cam.SetActive(false);
            }
            */
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            //****************************************************************************************************************************
            //on\off      
            if (!is_working)
            {
                LFforce = 0f;
                LBforce = 0f;
                RFforce = 0f;
                RBforce = 0f;

                propellers_ctrl.SetForce(LFforce, LBforce, RFforce, RBforce);

                return;
            }
            else
            {
                LFforce = original_force;
                LBforce = original_force;
                RFforce = original_force;
                RBforce = original_force;
            }

            //****************************************************************************************************************************
            //Revise the set parameter according to the input key
            float wantedRotation_x = 0;
            float wantedRotation_z = 0;

            if (Input.GetKey(InputKey.InputKeyUp))
            {
                HeightControl.set_height += HeightControl.set_vy * Time.deltaTime;
            }

            else if (Input.GetKey(InputKey.InputKeyDown))
            {
                HeightControl.set_height -= HeightControl.set_vy * Time.deltaTime;
                HeightControl.set_height = Mathf.Max(HeightControl.set_height, -5);
            }
            else if (Input.GetKey(InputKey.InputKey1))
            {
                HeightControl.set_height = 2.1f;
            }
            else if (Input.GetKey(InputKey.InputKey2))
            {
                HeightControl.set_height = 5.8f;
            }
            else if (Input.GetKey(InputKey.InputKey3))
            {
                HeightControl.set_height = 5f;
            }


            //Forward
            if (Input.GetKey(InputKey.InputKeyForward))
            {
                expect_velocity_z = DipControl.set_vz;
                //expect_velocity_z = Mathf.Min(expect_velocity_z + 3f * Time.deltaTime, DipControl.set_vz);
            }
            else if (Input.GetKey(InputKey.InputKeyBackward))
            {
                expect_velocity_z = -DipControl.set_vz;
                //expect_velocity_z = Mathf.Max(expect_velocity_z - 3f * Time.deltaTime, -DipControl.set_vz);
            }
            else if (Input.GetKey(InputKey.InputKey6))
            {
                //expect_velocity_z = Mathf.Max(expect_velocity_z - 3f * Time.deltaTime, -DipControl.set_vx);
                expect_velocity_z = MaxVSlow * DipControl.set_vz / DistenceSlow * (target.target1_z - this.transform.localPosition.z);
                expect_velocity_z = Mathf.Clamp(expect_velocity_z, -DipControl.set_vz, DipControl.set_vz);
            }
            else if (Input.GetKey(InputKey.InputKey7))
            {
                //expect_velocity_z = Mathf.Max(expect_velocity_z - 3f * Time.deltaTime, -DipControl.set_vx);
                expect_velocity_z = MaxVSlow * DipControl.set_vz / DistenceSlow * (target.target2_z - this.transform.localPosition.z);
                expect_velocity_z = Mathf.Clamp(expect_velocity_z, -DipControl.set_vz, DipControl.set_vz);
            }
            else if (Input.GetKey(InputKey.InputKey8))
            {
                //expect_velocity_z = Mathf.Max(expect_velocity_z - 3f * Time.deltaTime, -DipControl.set_vx);
                expect_velocity_z = MaxVSlow * DipControl.set_vz / DistenceSlow * (target.target3_z - this.transform.localPosition.z);
                expect_velocity_z = Mathf.Clamp(expect_velocity_z, -DipControl.set_vz, DipControl.set_vz);
            }

            else
                {
                //breaks

                if (expect_velocity_z > 0)
                {
                    expect_velocity_z = 0;
                    //expect_velocity_z = Mathf.Max(expect_velocity_z - 3f * Time.deltaTime, 0);
                }
                else if (expect_velocity_z < 0)
                {
                    expect_velocity_z = 0;
                    //expect_velocity_z = Mathf.Min(expect_velocity_z + 3f * Time.deltaTime, 0);
                }

            }

            //movingsideways

            if (Input.GetKey(InputKey.InputKeyRight))
            {
                //expect_velocity_x = Mathf.Min(expect_velocity_x + 3f * Time.deltaTime, DipControl.set_vx);
                expect_velocity_x = DipControl.set_vx;
            }
            else if (Input.GetKey(InputKey.InputKeyLeft))
            {
                //expect_velocity_x = Mathf.Max(expect_velocity_x - 3f * Time.deltaTime, -DipControl.set_vx);
                expect_velocity_x = -DipControl.set_vx;
            }
            else if (Input.GetKey(InputKey.InputKey5))
            {
                //expect_velocity_x = Mathf.Max(expect_velocity_x - 3f * Time.deltaTime, -DipControl.set_vx);
                expect_velocity_x = MaxVSlow * DipControl.set_vx / DistenceSlow * (target.target1_x - this.transform.localPosition.x);
                expect_velocity_x = Mathf.Clamp(expect_velocity_x, -DipControl.set_vx, DipControl.set_vx); 
            }
            else
            {
                if (expect_velocity_x > 0)
                {
                    //expect_velocity_x = Mathf.Max(expect_velocity_x - 3f * Time.deltaTime, 0);
                    expect_velocity_x = 0;
                }
                else if (expect_velocity_x < 0)
                {
                    //expect_velocity_x = Mathf.Min(expect_velocity_x + 3f * Time.deltaTime, 0);
                    expect_velocity_x = 0;
                }
            }

            if (Input.GetKey(InputKey.InputKeyTurnleft))
            {
                HorizontalRotationControl.set_eular_y -= HorizontalRotationControl.set_wy * Time.deltaTime;
            }
            else if (Input.GetKey(InputKey.InputKeyTurnright))
            {
                HorizontalRotationControl.set_eular_y += HorizontalRotationControl.set_wy * Time.deltaTime;
            }

            //****************************************************************************************************************************
            //Set the dip according to the velocity now

            //******************************
            //velocity on z axis(forward and backward)

            float velocity_z_now = ((rig.transform.position.z - position_old.z) * Mathf.Cos(rig.transform.eulerAngles.y * Mathf.Deg2Rad)) / Time.deltaTime;
            velocity_z_now += ((rig.transform.position.x - position_old.x) * Mathf.Sin(rig.transform.eulerAngles.y * Mathf.Deg2Rad)) / Time.deltaTime;

            //cal the parameter for pid control
            float des_acceleration_z = Mathf.Clamp(1.2f * (expect_velocity_z - velocity_z_now), -5, 5);
            float acc_z_now = (velocity_z_now - velocity_z_old) / Time.deltaTime;

            for (int i = accz_x.Length - 1; i > 0; i--)
            {
                accz_x[i] = accz_x[i - 1];
            }
            accz_x[0] = acc_z_now;
            float accz_y_out = filter.Filter(accz_x, accz_y, filter.px1, filter.py1);
            for (int i = accz_y.Length - 1; i > 0; i--)
            {
                accz_y[i] = accz_y[i - 1];
            }
            accz_y[0] = accz_y_out;

            float delta_acceleration_z = des_acceleration_z - accz_y_out;
            float integral_acceleration_z = integral_acceleration_z_old * 0.99f + delta_acceleration_z * Time.deltaTime;
            velocity_z_old = velocity_z_now;
            integral_acceleration_z_old = integral_acceleration_z;

            //******************************
            //velocity on x axis
            float velocity_x_now = -((rig.transform.position.z - position_old.z) * Mathf.Sin(rig.transform.eulerAngles.y * Mathf.Deg2Rad)) / Time.deltaTime;
            velocity_x_now += ((rig.transform.position.x - position_old.x) * Mathf.Cos(rig.transform.eulerAngles.y * Mathf.Deg2Rad)) / Time.deltaTime;

            //cal the parameter for pid control
            float des_acceleration_x = Mathf.Clamp(1.2f * (expect_velocity_x - velocity_x_now), -5, 5);

            float acc_x_now = (velocity_x_now - velocity_x_old) / Time.deltaTime;

            for (int i = accx_x.Length - 1; i > 0; i--)
            {
                accx_x[i] = accx_x[i - 1];
            }
            accx_x[0] = acc_x_now;
            float accx_y_out = filter.Filter(accx_x, accx_y, filter.px1, filter.py1);
            for (int i = accx_y.Length - 1; i > 0; i--)
            {
                accx_y[i] = accx_y[i - 1];
            }
            accx_y[0] = accx_y_out;

            float delta_acceleration_x = des_acceleration_x - accx_y_out;
            float integral_acceleration_x = integral_acceleration_x_old * 0.99f + delta_acceleration_x * Time.deltaTime;

            velocity_x_old = velocity_x_now;
            integral_acceleration_x_old = integral_acceleration_x;

            //Set the expected dip
            wantedRotation_x += DipControl.kp_v * delta_acceleration_z + DipControl.ki_v * integral_acceleration_z;
            wantedRotation_x = Mathf.Clamp(wantedRotation_x, -1 * rotationLimit, +1 * rotationLimit);
            wantedRotation_z -= DipControl.kp_v * delta_acceleration_x + DipControl.ki_v * integral_acceleration_x;
            wantedRotation_z = Mathf.Clamp(wantedRotation_z, -1 * rotationLimit, +1 * rotationLimit);

            //****************************************************************************************************************************
            //dip control
            float delta_rotation_x = Mathf.DeltaAngle(rig.transform.eulerAngles.x, wantedRotation_x);
            float delta_rotation_z = Mathf.DeltaAngle(rig.transform.eulerAngles.z, wantedRotation_z);

            float des_wx = Mathf.Clamp(0.08f * delta_rotation_x, -3f, 3f);
            float des_wz = Mathf.Clamp(0.08f * delta_rotation_z, -3f, 3f);
            float wx_now = 0.08f * Mathf.DeltaAngle(rotation_x_old, rig.transform.eulerAngles.x) / Time.deltaTime;
            float wz_now = 0.08f * Mathf.DeltaAngle(rotation_z_old, rig.transform.eulerAngles.z) / Time.deltaTime;
            float delta_wx = des_wx - 0.08f * Mathf.DeltaAngle(rotation_x_old, rig.transform.eulerAngles.x) / Time.deltaTime;
            float delta_wz = des_wz - 0.08f * Mathf.DeltaAngle(rotation_z_old, rig.transform.eulerAngles.z) / Time.deltaTime;
            float integral_wx = integral_wx_old * 0.99f + delta_wx * Time.deltaTime;
            float integral_wz = integral_wz_old * 0.99f + delta_wz * Time.deltaTime;
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

            LFforce += -col_rotation_x - col_rotation_z;
            LBforce += +col_rotation_x - col_rotation_z;
            RFforce += -col_rotation_x + col_rotation_z;
            RBforce += +col_rotation_x + col_rotation_z;

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

            LFforce += col_out_wy;
            LBforce -= col_out_wy;
            RFforce -= col_out_wy;
            RBforce += col_out_wy;

            //****************************************************************************************************************************
            //height
            float delta_height = HeightControl.set_height - rig.transform.position.y;
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

            LFforce += col_out_vy;
            LBforce += col_out_vy;
            RFforce += col_out_vy;
            RBforce += col_out_vy;

            //****************************************************************************************************************************
            //set force
            LFforce = Mathf.Clamp(LFforce, -20f, 20f);
            LBforce = Mathf.Clamp(LBforce, -20f, 20f);
            RFforce = Mathf.Clamp(RFforce, -20f, 20f);
            RBforce = Mathf.Clamp(RBforce, -20f, 20f);

            propellers_ctrl.SetForce(LFforce, LBforce, RFforce, RBforce);

        }

    }

    void OnApplicationQuit()
    {

    }


}
