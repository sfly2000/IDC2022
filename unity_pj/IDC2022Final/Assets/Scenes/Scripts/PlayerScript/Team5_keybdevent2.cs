using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Team5_keybdevent2 : MonoBehaviour
{
    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    
    private static extern void Keybd_event(
          byte bvk,//虚拟键值 ESC键对应的是27
          byte bScan,//0
          int dwFlags,//0为按下，1按住，2释放
          int dwExtraInfo//0
          );
    // Update is called once per frame
    // private bool is_work = false;

    // void Start()
    // {
    //     Keybd_event(82, 0, 0, 0);
    //     Keybd_event(87, 0, 1, 0);
    // }
    void Update()
    {
    
    // 
        // if (Input.GetKeyDown(KeyCode.O))
        //  {
        //     Keybd_event(85, 0, 0, 0);
        //     Keybd_event(73, 0, 2, 0);
        //  }
        //  if (Input.GetKeyDown(KeyCode.P))
        //  {
        //     Keybd_event(85, 0, 2, 0);
        //     Keybd_event(73, 0, 0, 0);
        //  }
         if (Input.GetKeyDown(KeyCode.M))
         {
            Keybd_event(72, 0, 0, 0);
            Keybd_event(74, 0, 2, 0);
         }
         if (Input.GetKeyDown(KeyCode.N))
         {
            Keybd_event(72, 0, 2, 0);
            Keybd_event(74, 0, 0, 0);
         }
    //    if (is_work)
    //    {
    //         print ("y坐标为："+this.transform.localPosition.y);
            
    //         if (this.transform.localPosition.y-set_y>1)
    //         {
    //         Keybd_event(69, 0, 0, 0);
    //         Keybd_event(81, 0, 2, 0);
            
    //         }
    //         if (this.transform.localPosition.y-set_y<-1)
    //         {
    //         Keybd_event(81, 0, 0, 0);
    //         Keybd_event(69, 0, 2, 0);
    //         }
    //    }
       
    //    print ("X坐标为："+this.transform.localPosition.x);
        
        // Keybd_event(87, 0, 1, 0);
        // if (stop1==0)
        // {
        //     Keybd_event(65, 0, 0, 0);
        //     Debug.Log("aaa");
            
        // }
        // if (Input.GetKey(KeyCode.G))
        // {
        //     stop1=1;
        // }
        // if (stop1==1)
        // {
        //     Keybd_event(65, 0, 2, 0);
        //     Debug.Log("bbb");
            
        // }
        //  if (Input.GetKey(KeyCode.H))
        // {
        //     stop1=0;
        // }
        

    }
}

