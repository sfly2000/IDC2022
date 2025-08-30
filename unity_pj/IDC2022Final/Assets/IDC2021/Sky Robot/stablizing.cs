using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class stablizing : MonoBehaviour
{
    private Rigidbody rb;
    private PhotonView photonView;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        photonView = this.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;

        Quaternion deltaQuat = Quaternion.FromToRotation(rb.transform.up, Vector3.up);

        Vector3 axis;
        float angle;
        deltaQuat.ToAngleAxis(out angle, out axis);

        float dampenFactor = 10.0f; // this value requires tuning
        rb.AddTorque(-rb.angularVelocity * dampenFactor, ForceMode.Acceleration);

        float adjustFactor = 10.0f; // this value requires tuning
        rb.AddTorque(axis.normalized * angle * adjustFactor, ForceMode.Acceleration);
    }
}