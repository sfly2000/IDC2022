using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PositionUI8B: MonoBehaviour
{

    public Text positionUI;
    private Transform robotPosition;
    private float x, y, z;
    private string str;
    public PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            robotPosition = this.transform;
            x = robotPosition.position.x;
            y = robotPosition.position.y;
            z = robotPosition.position.z;
            str = string.Format("x={0:N2},y={1:N2},z={2:N2}", x, y, z);
            positionUI.text = str;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.CurrentRoom == null || photonView.IsMine)
        {
            robotPosition = this.transform;
            x = robotPosition.position.x;
            y = robotPosition.position.y;
            z = robotPosition.position.z;
            str = string.Format("x={0:N2},y={1:N2},z={2:N2}", x, y, z);
            positionUI.text = str;
        }
    }
}