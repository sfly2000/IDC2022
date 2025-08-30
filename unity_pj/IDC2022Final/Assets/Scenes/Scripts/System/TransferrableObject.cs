using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TransferrableObject : MonoBehaviourPunCallbacks  //for "common" objects that can be moved and controlled by all clients;
{
    public PhotonView pv_this;
    public float t_buff = 0;  //time buffer to prevent frequent transfer of ownership;
    private bool during_buff = false;
    private float timer = 0;

    void Update()
    {
        if(during_buff)
        {
            timer += Time.deltaTime;
        }

        if(timer >= t_buff)
        {
            timer = 0;
            during_buff = false;
        }
    }

    public void OnCollisionEnter(Collision other)  //transferring ownership to one who touches it so that it can be moved;
    {
        if(during_buff) return;
        GameObject obj = other.gameObject;
        if(obj.GetComponentInParent<PhotonView>()==null)  return;
        PhotonView pv_other = obj.GetComponentInParent<PhotonView>();
        pv_this.TransferOwnership(pv_other.Owner);
    }
}
