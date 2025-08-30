using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlTree : MonoBehaviour
{
    public delegate void PlayerScore(int temp);//定义委托
    public event PlayerScore GetScore;//定义得分事件，用于发出得分的消息
    void Start()
    {

    }
    void Update()//在Update()中添加Player的移动控制
    {
    }
    public void OnTriggerExit(Collider other)//设置触发器碰撞事件，一旦Player穿过了ScoreObj,就发送得分事件
    {
        if (other.gameObject.name.Equals("Cube"))//检查Player碰撞的物体是不是ScoreObj
        {
            if (GetScore != null)//检查事件是否为空，即有没有接收器订阅它
            {
                GetScore(20);//发送得分事件消息，为接收器提供参数1，实现+1分的效果
            }
        }
        if (other.gameObject.name.Equals("Cube (2)"))//检查Player碰撞的物体是不是ScoreObj
        {
            if (GetScore != null)//检查事件是否为空，即有没有接收器订阅它
            {
                GetScore(40);//发送得分事件消息，为接收器提供参数1，实现+1分的效果
            }
        }
        if (other.gameObject.name.Equals("Cube (1)"))//检查Player碰撞的物体是不是ScoreObj
        {
            if (GetScore != null)//检查事件是否为空，即有没有接收器订阅它
            {
                GetScore(20);//发送得分事件消息，为接收器提供参数1，实现+1分的效果
            }
        }
        if (other.gameObject.name.Equals("Cube (3)"))//检查Player碰撞的物体是不是ScoreObj
        {
            if (GetScore != null)//检查事件是否为空，即有没有接收器订阅它
            {
                GetScore(50);//发送得分事件消息，为接收器提供参数1，实现+1分的效果
            }
        }
    }
}
