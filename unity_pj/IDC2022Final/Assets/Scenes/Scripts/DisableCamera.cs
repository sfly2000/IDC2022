using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DisableCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject thisCamera;
    public PhotonView photonView;

    void Start()
    {
        if (PhotonNetwork.CurrentRoom != null && photonView.IsMine == false)
        {
            thisCamera.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
