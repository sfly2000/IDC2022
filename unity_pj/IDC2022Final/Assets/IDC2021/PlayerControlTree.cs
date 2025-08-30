using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlTree : MonoBehaviour
{
    public delegate void PlayerScore(int temp);//����ί��
    public event PlayerScore GetScore;//����÷��¼������ڷ����÷ֵ���Ϣ
    void Start()
    {

    }
    void Update()//��Update()�����Player���ƶ�����
    {
    }
    public void OnTriggerExit(Collider other)//���ô�������ײ�¼���һ��Player������ScoreObj,�ͷ��͵÷��¼�
    {
        if (other.gameObject.name.Equals("Cube"))//���Player��ײ�������ǲ���ScoreObj
        {
            if (GetScore != null)//����¼��Ƿ�Ϊ�գ�����û�н�����������
            {
                GetScore(20);//���͵÷��¼���Ϣ��Ϊ�������ṩ����1��ʵ��+1�ֵ�Ч��
            }
        }
        if (other.gameObject.name.Equals("Cube (2)"))//���Player��ײ�������ǲ���ScoreObj
        {
            if (GetScore != null)//����¼��Ƿ�Ϊ�գ�����û�н�����������
            {
                GetScore(40);//���͵÷��¼���Ϣ��Ϊ�������ṩ����1��ʵ��+1�ֵ�Ч��
            }
        }
        if (other.gameObject.name.Equals("Cube (1)"))//���Player��ײ�������ǲ���ScoreObj
        {
            if (GetScore != null)//����¼��Ƿ�Ϊ�գ�����û�н�����������
            {
                GetScore(20);//���͵÷��¼���Ϣ��Ϊ�������ṩ����1��ʵ��+1�ֵ�Ч��
            }
        }
        if (other.gameObject.name.Equals("Cube (3)"))//���Player��ײ�������ǲ���ScoreObj
        {
            if (GetScore != null)//����¼��Ƿ�Ϊ�գ�����û�н�����������
            {
                GetScore(50);//���͵÷��¼���Ϣ��Ϊ�������ṩ����1��ʵ��+1�ֵ�Ч��
            }
        }
    }
}
