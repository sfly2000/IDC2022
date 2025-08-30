using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class JointManagerG6 : MonoBehaviour
{
    public  HingeJoint sideJoint;
    public  JointLimits jointLimit;
    
    [System.Serializable]
    public class SetInputKey
    {
        public string InputKeyk;
        public string InputKeyj;
    }
    public SetInputKey InputKey;
    // Start is called before the first frame update
    void Start()
    {
        sideJoint = GetComponent<HingeJoint>();
        jointLimit.max = 60;
        jointLimit.min = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(InputKey.InputKeyk))
        {
            jointLimit.max = 60;
            jointLimit.min = 0;
            sideJoint.limits = jointLimit;
        }
        else if (Input.GetKey(InputKey.InputKeyj))
        {
            jointLimit.max = 10;
            jointLimit.min = 0;
            sideJoint.limits = jointLimit;
        }
        
    }
}
