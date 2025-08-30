using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class VelControl8B : MonoBehaviour
{
    public string speed_up, speed_low;
    public WheelCollider lb, rb, lf, rf;
    private float dampingRate;
    public Text dampingText;
    public PhotonView photonView;

    void Start()
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            dampingRate = lb.wheelDampingRate;
            dampingText.text = string.Format("wheel damping rate:{0:N2}", dampingRate);
        }
    }

    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            if (Input.GetKeyDown(speed_up))
            {
                dampingRate -= (float)0.02;
            }
            if (Input.GetKeyDown(speed_low))
            {
                dampingRate += (float)0.02;
            }
            if (dampingRate >= 0.2)
                dampingRate = (float)0.2;
            if (dampingRate <= 0.04)
                dampingRate = (float)0.04;
            lb.wheelDampingRate = dampingRate;
            rb.wheelDampingRate = dampingRate;
            lf.wheelDampingRate = dampingRate;
            rf.wheelDampingRate = dampingRate;
            dampingText.text = string.Format("wheel damping rate:{0:N2}", dampingRate);
        }
    }
}
