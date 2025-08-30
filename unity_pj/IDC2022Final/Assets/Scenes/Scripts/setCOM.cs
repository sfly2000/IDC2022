using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//used to set the center of mass of rigid body or articulation body
//need to create an empty object for the body as the position of COM
public class setCOM : MonoBehaviour
{
    public Transform com;
    private Vector3 vec_com;
    private Rigidbody rb;
    private ArticulationBody ab;
    // Start is called before the first frame update
    void Start()
    {
        vec_com = com.localPosition;
        if (GetComponent<Rigidbody>() != null)
        {
            rb = GetComponent<Rigidbody>();
            rb.centerOfMass = vec_com;
        }
        else
        {
            ab = GetComponent<ArticulationBody>();
            ab.centerOfMass = vec_com;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
