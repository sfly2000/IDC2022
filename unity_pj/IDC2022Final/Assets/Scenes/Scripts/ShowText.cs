using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowText : MonoBehaviour
{
    // Start is called before the first frame update

    public Text number;
    private int num = 7;
    public string low;
    public string high;
    void Start()
    {
        number.text = "7";
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
