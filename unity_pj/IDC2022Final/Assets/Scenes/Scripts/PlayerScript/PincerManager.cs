using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PincerManager : MonoBehaviour
{
    private Collider myCollider;

    [System.Serializable]
    public class SetInputKey
    {
        public string InputKeyOpen;
        public string InputKeyClose;
    }
    public SetInputKey InputKey;

    // Start is called before the first frame update
    void Start()
    {
        myCollider = GetComponent<Collider>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(InputKey.InputKeyOpen))
        {
            myCollider.enabled = false;
        }
        else if (Input.GetKey(InputKey.InputKeyClose))
        {
            myCollider.enabled = true;
        }
    }
}
