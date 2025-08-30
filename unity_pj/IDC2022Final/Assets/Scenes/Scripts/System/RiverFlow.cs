using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverFlow : MonoBehaviour
{
    public Transform endpoint;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.tag == "Flowable")
        {
            col.transform.position = Vector3.MoveTowards(col.transform.position, endpoint.position, speed * Time.deltaTime);
        }
    }
}
