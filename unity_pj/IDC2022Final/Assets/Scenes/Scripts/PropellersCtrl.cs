using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropellersCtrl : MonoBehaviour
{
    // Start is called before the first frame update
    public PropellerCtrl LF;
    public PropellerCtrl LB;
    public PropellerCtrl RF;
    public PropellerCtrl RB;
    public Rigidbody rig;

    private float LFforce;
    private float LBforce;
    private float RFforce;
    private float RBforce;

    void Start()
    {
        LFforce = 0f;
        LBforce = 0f;
        RFforce = 0f;
        RBforce = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        LF.SetInputForce(LFforce);
        LB.SetInputForce(LBforce);
        RF.SetInputForce(RFforce);
        RB.SetInputForce(RBforce);
        LF.SetInputTorque(LFforce);
        LB.SetInputTorque(-LBforce);
        RF.SetInputTorque(-RFforce);
        RB.SetInputTorque(RBforce);

        rig.AddRelativeTorque((LFforce - LBforce - RFforce + RBforce) * Vector3.up);
    }

    public void SetForce(float Input_LFforce, float Input_LBforce, float Input_RFforce, float Input_RBforce)
    {
        LFforce = Input_LFforce;
        LBforce = Input_LBforce;
        RFforce = Input_RFforce;
        RBforce = Input_RBforce;
    }
}
