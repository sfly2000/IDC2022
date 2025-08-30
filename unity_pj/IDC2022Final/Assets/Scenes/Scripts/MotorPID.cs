using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorPID : MonoBehaviour
{
    public float MaxDegree = 90;
    public float MinDegree = 0;
    public float NowDegree;
    public float SetDegree = 45;

    public float Kp;
    public float Ki;
    public float Kd;

    [SerializeField]
    private GameObject TargetObject;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
