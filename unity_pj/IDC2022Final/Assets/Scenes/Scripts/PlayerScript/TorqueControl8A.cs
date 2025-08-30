using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TorqueControl8A : MonoBehaviour
{
    public string torque_up, torque_low;
    public joint_motor catcherJoint;
    private float torque;
    public Text torqueText;
    public PhotonView photonView;

    void Start()
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            torque = catcherJoint.torque;
            torqueText.text = string.Format("catcher torque:{0:N1}", torque);
        }
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            if (Input.GetKeyDown(torque_up))
            {
                torque -= (float)0.2;
            }
            if (Input.GetKeyDown(torque_low))
            {
                torque += (float)0.2;
            }
            if (torque >= 6.5)
                torque = (float)6.5;
            if (torque <= 4.5)
                torque = (float)4.5;
            catcherJoint.torque = torque;
            torqueText.text = string.Format("catcher torque:{0:N1}", torque);
        }
    }
}
