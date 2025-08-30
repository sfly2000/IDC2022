using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class atorque : MonoBehaviour
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
                tr = 0.0f;
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
                if (Input.GetKey(KeyCode.Alpha4))
                {    
                    tr = 10.0f;
                    Debug.Log("Alpha4 has been presse");
                }
                if (Input.GetKey(KeyCode.Alpha1))
                {
                    tr = -10.0f;
                    flag = false;
                    Debug.Log("Alpha1 has been pressed");
                }
            }
           
            

            settq(tr);
            artorque.settq(tr);

            Vector3 t = new Vector3(2 * tr,2 * tr,2 * tr);
            rb.AddRelativeTorque(t);
        }
    }

    public void settq(float t)
    {
        tr = t;
    }
}
