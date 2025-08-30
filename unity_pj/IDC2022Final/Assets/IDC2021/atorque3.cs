using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class atorque3 : MonoBehaviour
{
    // Start is called before the first frame update
    Rigidbody rb;
    private float tr;
    private bool flag = true;

    public PhotonView photonView;
    void Start()
    {
        rb = GetComponent<Rigidbody>();//this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if(flag){
                tr = 10.0f;
            }
            else{
                tr = -10.0f;
            }
            
            if (Application.platform == RuntimePlatform.Android)
            {
                if (OVRInput.GetDown(OVRInput.RawButton.A))
                {
                    tr = 10.0f;
                }
                if (OVRInput.GetDown(OVRInput.RawButton.B))
                {
                    tr = -10.0f;
                }
            }
            else
            //if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetKey(KeyCode.Alpha6))
                {    
                    tr = 10.0f;
                    Debug.Log("Alpha6 has been presse");
                }
                if (Input.GetKey(KeyCode.Alpha3))
                {
                    tr = -10.0f;
                    flag = false;
                    Debug.Log("Alpha3 has been pressed");
                }
            }
           
            settq(tr);
            artorque.settq(tr);

            Vector3 t = new Vector3(4.0f *tr,4.0f * tr,4.0f * tr);
            rb.AddRelativeTorque(t);
        }
    }

    public void settq(float t)
    {
        tr = t;
    }
}
