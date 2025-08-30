using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Controller_sky : MonoBehaviour
{
    // Start is called before the first frame update
    private float

            tx,
            ty,
            tz,
            fx,
            fy,
            fz;

    private Rigidbody rb;

    private PhotonView photonView;

    public GameObject camera1;

    public GameObject camera2;

    public GameObject camera3;

    public Vector3 center = new Vector3(0f, 0f, 0f);

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.centerOfMass = center;
        photonView = this.GetComponent<PhotonView>();
        camera1.SetActive(false);
        camera2.SetActive(false);
        camera3.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKey(KeyCode.B))
        {
            camera1.SetActive(true);
            camera2.SetActive(false);
            camera3.SetActive(false);
        }

        if (Input.GetKey(KeyCode.V))
        {
            camera1.SetActive(false);
            camera2.SetActive(true);
            camera3.SetActive(false);
        }

        if (Input.GetKey(KeyCode.C))
        {
            camera1.SetActive(false);
            camera2.SetActive(false);
            camera3.SetActive(true);
        }

        fx = 0.0f;
        fy = 15.0f;
        fz = 0.0f;
        tx = 0.0f;
        ty = 0.0f;
        tz = 0.0f;
        if (Input.GetKey(KeyCode.H)) fx = 50.0f;
        if (Input.GetKey(KeyCode.K)) fx = (-50.0f);
        if (Input.GetKey(KeyCode.Y)) fy = 50.0f;
        if (Input.GetKey(KeyCode.I)) fy = -50.0f;
        if (Input.GetKey(KeyCode.J)) fz = 50.0f;
        if (Input.GetKey(KeyCode.U)) fz = -50.0f;

        if (Input.GetKey(KeyCode.D)) tx = 50.0f;
        if (Input.GetKey(KeyCode.A)) tx = -50.0f;
        if (Input.GetKey(KeyCode.W)) ty = 50.0f;
        if (Input.GetKey(KeyCode.X)) ty = -50.0f;
        if (Input.GetKey(KeyCode.E)) tz = 50.0f;
        if (Input.GetKey(KeyCode.Z)) tz = -50.0f;

        Vector3 f = new Vector3(fx, fy, fz);

        //rb.AddRelativeForce(f);
        rb.AddForce (f);
        Vector3 t = new Vector3(tx, ty, tz);

        //rb.AddRelativeTorque(t);
        rb.AddTorque (t);
    }
}
