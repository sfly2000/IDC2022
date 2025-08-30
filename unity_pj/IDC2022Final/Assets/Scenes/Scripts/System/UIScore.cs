using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIScore : MonoBehaviour
{
    public List<ScoreObj> objs;
    public int TotalScoreRed, TotalScoreBlue;
    public Text ScoreUIRed, ScoreUIBlue;
    void Start()
    {
        foreach (ScoreObj i_obj in objs){
            i_obj.GetScore += Obj_GetScore;
            i_obj.GetScoreFail += Obj_GetScoreFail;
        }
    }

    private void Obj_GetScore(int score, bool isRed)
    {
        if (isRed)
        {
            TotalScoreRed += score;
            ScoreUIRed.text = TotalScoreRed.ToString();
        }
        else
        {
            TotalScoreBlue += score;
            ScoreUIBlue.text = TotalScoreBlue.ToString();
        }
    }
    private void Obj_GetScoreFail(int score, bool isRed)
    {
        if (isRed)
        {
            TotalScoreRed -= score;
            ScoreUIRed.text = TotalScoreRed.ToString();
        }
        else
        {
            TotalScoreBlue -= score;
            ScoreUIBlue.text = TotalScoreBlue.ToString();
        }
    }
    void Update()
    {

    }
    /*
    public int ScoreRed
    {
        get
        {
            return TotalScoreRed;
        }
        set
        {
            TotalScoreRed = value;
            ScoreUIRed.text = TotalScoreRed.ToString();
        }
    }

    public int ScoreBlue
    {
        get
        {
            return TotalScoreBlue;
        }
        set
        {
            TotalScoreBlue = value;
            ScoreUIBlue.text = TotalScoreBlue.ToString();
        }
    }
    */

}

