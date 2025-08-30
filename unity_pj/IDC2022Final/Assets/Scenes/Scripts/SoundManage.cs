using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class SoundManage : MonoBehaviour
{
    public float CountDownTime;
    private float GameTime;
    private float timer = 0;
    public Text GameCountTimeText;
    public bool start = false;
    private bool is_end;
    private int M;
    private float S;

    private AudioSource music;
    private AudioClip sound1;
    private AudioClip sound2;

    void Start()
    {
        GameTime = CountDownTime;
        M = (int)(GameTime / 60);
        S = GameTime % 60;

        is_end = false;
        music = gameObject.AddComponent<AudioSource>();
        music.playOnAwake = false;
        sound1 = Resources.Load<AudioClip>("sound/chrip1");
        sound2 = Resources.Load<AudioClip>("sound/chrip2");
    }
    void Update()
    {
        if (start)
        {
            timer += Time.deltaTime;
        }

        if (timer >= 1f)
        {
            timer = 0;
            GameTime--;
            //Debug.Log(GameTime);
            M = (int)(GameTime / 60);
            S = GameTime % 60;
            if (M <= 0 && S <= 3 && !is_end)
            {
                if (S == 0)
                {
                    music.clip = sound2;
                    is_end = true;
                }
                else
                {
                    music.clip = sound1;
                }
                if (music.clip == null)
                {
                    Debug.Log("clips empty");
                }
                Debug.Log("play music");
                music.Play();
            }
        }

        if (M >= 0 && S >= 0)
        {
            GameCountTimeText.text = M + ": " + string.Format("{0:00}", S);
        }
        else
        {
            //Time.timeScale = 0;  //pause game
            Invoke("EndGame", 3);
        }
    }

    void EndGame()
    {
        Debug.Log("End Game");
        /*#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
              UnityEngine.Application.Quit();
        #endif*/
        Time.timeScale = 0;
        music.playOnAwake = false;
    }

    public void StartCountDown()
    {
        start = true;
    }
}