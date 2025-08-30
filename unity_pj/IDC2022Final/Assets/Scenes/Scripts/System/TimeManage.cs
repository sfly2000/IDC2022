using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class TimeManage : MonoBehaviour
{
    public float CountDownTime;
    private float GameTime;
    private int PrepareTime = 5;
    private float timer = 0;
    public Text GameCountTimeText;
    public Text PrepareCountTimeText;
    public bool start = false;
    public bool prepared = false;
    private int M;
    private float S;

    private AudioSource music;
    private AudioClip sound1;
    private AudioClip sound2;
    private AudioClip sound3;

    public GameObject TreeSpecial;


    void Start()
    {
        GameTime = CountDownTime;
        M = (int)(GameTime / 60);
        S = GameTime % 60;
        music = gameObject.AddComponent<AudioSource>();

        music.playOnAwake = false;
        sound1 = Resources.Load<AudioClip>("sound/chrip1");
        sound2 = Resources.Load<AudioClip>("sound/chrip2");
        sound3 = Resources.Load<AudioClip>("sound/last15sec");
    }
    void Update()
    {
        int i_pos = 0;
        if (PhotonNetwork.CurrentRoom != null)
        {
            i_pos = (int)PhotonNetwork.LocalPlayer.CustomProperties["position"];
        }
        
        if (prepared)
        {
            // bool IsToPlay = false;
            if (PrepareCountTimeText.text == "")
            {
                PrepareCountTimeText.text = string.Format("{0}", PrepareTime);
            }
            // PrepareCountTimeText.SetActive("true");0
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                timer = 0f;
                // IsToPlay = true;
                PrepareTime--;
                PrepareCountTimeText.text = string.Format("{0}", PrepareTime);
            }
            if (PrepareTime == 0)
            {
                timer = 0f;
                prepared = false;
                start = true;
                PrepareCountTimeText.text = "";
                // PrepareCountTimeText.SetActive("false");
                music.clip = sound2;
            }
            else
            {
                music.clip = sound1;
            }
            if (timer == 0f)
            {
                Debug.Log("play music");
                music.Play();
            }
        }

        if(start)
        {
            timer += Time.deltaTime;
            if (timer >= 1f)
            {
                timer = 0;
                GameTime--;
                //Debug.Log(GameTime);
                M = (int)(GameTime / 60);
                S = GameTime % 60;

                if (M == 0 && S == 15)
                {
                    music.clip = sound3;
                    music.Play();
                }

                if ((i_pos == 1 || PhotonNetwork.OfflineMode == true) && M == 1 && S == 0)
                {
                    Transform tf_target = TreeSpecial.GetComponent<Transform>();
                    PhotonNetwork.Instantiate(TreeSpecial.name, tf_target.position, tf_target.rotation);
                }
            }

            if (M >= 0 && S >= 0)
            {
                GameCountTimeText.text = M + ": " + string.Format("{0:00}", S);
            }
            else
            {
                //Time.timeScale = 0;  //pause game
                //Invoke("EndGame", 3);
                EndGame();
            }

            if (M == 0 && S <= 15)
            {
                GameCountTimeText.fontSize = 20;
                if (timer <= 0.5f)
                {
                    GameCountTimeText.color = Color.red;
                }
                else
                {
                    GameCountTimeText.color = Color.black;
                }
            }
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