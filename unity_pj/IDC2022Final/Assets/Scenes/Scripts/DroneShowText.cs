using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DroneShowText : MonoBehaviour
{   
    public Text number;
    private int num = 0;
    public string low;
    public string high;
    // Start is called before the first frame update
    void Start()
    {
        number.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(low))
        {
            num -= 1;
            number.text = num.ToString();
        }
        if (Input.GetKeyDown(high))
        {
            num += 1;
            number.text = num.ToString();
        }
    }
}
