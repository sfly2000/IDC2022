using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class o3A_DriveWheelCtrl : MonoBehaviour
{
    [Range(0f, 1f)]
    public float turnSpeed;
    [Range(0f, 1f)]
    public float forwardSpeed;
    [Range(0f, 1f)]
    public float backwardSpeed;
    public float brakeSpeed;

    private o3A_DriveWheelL leftWheel;
    private o3A_DriveWheelR rightWheel;

    private bool isEnd = false;

    void Start()
    {
        leftWheel = GameObject.FindObjectOfType<o3A_DriveWheelL>();
        rightWheel = GameObject.FindObjectOfType<o3A_DriveWheelR>();
        leftWheel.isManual = false;
        rightWheel.isManual = false;
    }

    void FixedUpdate()
    {
        if (isEnd) return;

        if (this.transform.position.z >= -4f)
        {
            Forward();
        }
        else
        {
            Stop();
            leftWheel.isManual = true;
            rightWheel.isManual = true;
            isEnd = true;
        }
    }

    void TurnLeft()
    {
        leftWheel.ApplyTorqueAllWheels(-turnSpeed, 0f);
        rightWheel.ApplyTorqueAllWheels(-turnSpeed, 0f);
    }

    void TurnRight()
    {
        leftWheel.ApplyTorqueAllWheels(turnSpeed, 0f);
        rightWheel.ApplyTorqueAllWheels(turnSpeed, 0f);
    }

    void Forward()
    {
        leftWheel.ApplyTorqueAllWheels(0f, forwardSpeed);
        rightWheel.ApplyTorqueAllWheels(0f, forwardSpeed);
    }

    void Backward()
    {
        leftWheel.ApplyTorqueAllWheels(0f, -backwardSpeed);
        rightWheel.ApplyTorqueAllWheels(0f, -backwardSpeed);
    }

    void Stop()
    {
        leftWheel.ApplyTorqueAllWheels(0f, 0f);
        rightWheel.ApplyTorqueAllWheels(0f, 0f);
        leftWheel.BrakeAllWheels(brakeSpeed);
        rightWheel.BrakeAllWheels(brakeSpeed);
    }
}
