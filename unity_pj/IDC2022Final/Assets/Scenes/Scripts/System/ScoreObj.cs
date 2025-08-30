using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScoreInfo
{
    public string TargetAreaName;
    public int ScoreValue;
    public bool isRed;
}

public class ScoreObj : MonoBehaviour
{
    public delegate void PlayerScore(int score, bool isRed);
    public event PlayerScore GetScore, GetScoreFail;
    public List<ScoreInfo> ScoreInfos;
    public void OnTriggerEnter(Collider other)
    {   

        foreach (ScoreInfo i in ScoreInfos)
        {
            if (other.name.Equals(i.TargetAreaName))
            {
                if (GetScore != null)
                {
                    GetScore(i.ScoreValue, i.isRed);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        foreach (ScoreInfo i in ScoreInfos)
        {
            if (other.name.Equals(i.TargetAreaName))
            {
                if (GetScoreFail != null)
                {
                    GetScoreFail(i.ScoreValue, i.isRed);
                }
            }
        }
    }
}

