using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class team5keybdevent : MonoBehaviour
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
        if (Input.GetKeyDown(KeyCode.U))
         {
            Keybd_event(79, 0, 0, 0);
            Keybd_event(80, 0, 2, 0);
         }
         if (Input.GetKeyDown(KeyCode.I))
         {
            Keybd_event(79, 0, 2, 0);
            Keybd_event(80, 0, 0, 0);
         }
        //  if (Input.GetKeyDown(KeyCode.V))
        //  {
        //     Keybd_event(77, 0, 0, 0);
        //     Keybd_event(78, 0, 2, 0);
        //  }
         if (Input.GetKeyDown(KeyCode.B))
         {
            Keybd_event(77, 0, 2, 0);
            Keybd_event(78, 0, 0, 0);
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


