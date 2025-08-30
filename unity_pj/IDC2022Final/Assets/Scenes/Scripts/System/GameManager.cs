using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;

// for Selection
using UnityEditor;

[System.Serializable]
public class StartSetting  //position and rotation to spawn your robot;
{
    public Vector3 Pos;
    public Quaternion Rot;
    public GameObject TargetObject;  //objects to be moved by this robot;  this is a workaround for the ownership issue of PUN, change to Fusion is recommended
}

public class GameManager : MonoBehaviourPunCallbacks
{
    public GameObject SetupUI, GameUI;
    public InputField posX, posY, posZ;

    // edited 22/07/01
    public Text TextRoomName;
    public Text TextRoomUser;

    // public GameObject LocalCamera; edited 22/06/30
    public GameObject playerCamFollow;
    public GameObject playerCamMouse;

    // added 22/06/30
    private bool IsPlayer;
    [SerializeField]
    public CameraController cameraController;

    public TimeManage Timer;

    public GameObject Movables;  //objects that can be moved in the scene;

    public List<StartSetting> DefaultList;
    private StartSetting PlayerSetting;
    private string RobotName;
    public GameObject LocalRobot;

    private Room room = PhotonNetwork.CurrentRoom;

    // edited 22/07/11 private->public
    public bool start = false;
    private Vector3 vec_restart;  //for test runs;
    private Quaternion qua_restart;
    // public GameObject TestPrefab;  //added 22/07/19 prefab is for respawning;
    // added 22/07/22
    public string TestPrefabName;

    // added 22/07/11
    private List<Player> ListOfPlayers = new List<Player>();
    public Dictionary<string, GameObject> DictOfRobots = new Dictionary<string, GameObject>();
    // private int CamFollowNum = 0;

    // added 22/07/14
    PhotonView view;

    // added 22/07/15
    private bool IsFirstFrame = true;

    [PunRPC]
    public void RobotTagger(string text)
    {
        Debug.Log("RobotTagger: ");
        if (LocalRobot == null)
        {
            Debug.Log("LocalRobot is null");
            return;
        }
        Debug.Log("LocalRobot.name: " + LocalRobot.name);
        Debug.Log("LocalRobot.tag: " + LocalRobot.tag);
        Debug.Log("text: " + text);
        LocalRobot.tag = text;
    }

    // Start is called before the first frame update
    void Start()
    {
        // added 22/07/14
        view = this.GetComponent<PhotonView>();

        Debug.Log("GameManager Activated");
        if(room == null)  //for cases that have skipped lobby scene such as test runs;
        {
            TestRun();
            return;
        }
        else
        {
            Debug.Log("room exists");
        }

        GameUI.SetActive(false);
        SetupUI.SetActive(true);
        Movables.SetActive(false);
        RobotName = PhotonNetwork.LocalPlayer.NickName;
        Debug.Log(PhotonNetwork.LocalPlayer.CustomProperties);

        int i_pos = (int)PhotonNetwork.LocalPlayer.CustomProperties["position"];

        // if (i_pos <= 4)
        if (i_pos <= 3)
        {
            PlayerSetting = DefaultList[i_pos];
        }

        if (i_pos <= 3)
        {
            // PlayerSetting = DefaultList[i_pos];
            Debug.Log("i_pos<=3:");
            LocalRobot = PhotonNetwork.Instantiate(RobotName, PlayerSetting.Pos, PlayerSetting.Rot);
            Debug.Log("SetTag: ");
            LocalRobot.tag = RobotName;
            Debug.Log("RPC: ");
            view.RPC("RobotTagger", RpcTarget.All, LocalRobot.tag);
            CamSetTarget(LocalRobot);
            IsPlayer = true;
        }
        else
        {
            IsPlayer = false;
            print("start as observer");
            // SetupUI.ButtonReady.SetActive(false);
            // 22/07/18
        }

        cameraController.SetIsPlayer(IsPlayer);

        // added 22/07/01
        // update room info
        TextRoomName.text = "room: " + room.Name;
        // SetRoomUserText
    }

    public void TestRun()
    {
        Debug.Log("TestRun");
        Transform tf_restart = LocalRobot.GetComponent<Transform>();

        // added 22/06/30
        IsPlayer = true;
        cameraController.SetIsPlayer(IsPlayer);
        // added 22/07/01
        TextRoomName.text = "room: test run";
        TextRoomUser.text = "current players in room: local";

        vec_restart = tf_restart.position;
        qua_restart = tf_restart.rotation;
        GameUI.SetActive(true);
        SetupUI.SetActive(false);
        Movables.SetActive(true);
        CamSetTarget(LocalRobot);

        Debug.Log("test run restart position");
        Debug.Log(vec_restart);
        Debug.Log("test run restart qua");
        Debug.Log(qua_restart);

    }

    // Update is called once per frame
    void Update()
    {

        if (room != null)
        {
            // if (start)
            // {
                // check every player if their robot has been located(or referred?)
                foreach (Player player in ListOfPlayers)
                {
                    if (!DictOfRobots.ContainsKey(player.NickName) || DictOfRobots[player.NickName] == null)
                    {
                        // DictOfRobots[player.NickName] = GameObject.FindWithTag(player.NickName);
                        GameObject FindObject = GameObject.FindWithTag(player.NickName);
                        if (FindObject == null)
                        {
                            Debug.Log("* Debug: Failed to Find GameObject with Tag: " + player.NickName);
                        }
                        else
                        {
                            Debug.Log("* Debug: Find GameObject with Tag: " + player.NickName);
                            DictOfRobots[player.NickName] = FindObject;
                        }
                    }
                }
            //}
        }

        // added 22/07/15
        // update RoomInfo
        if (IsFirstFrame)
        {
            IsFirstFrame = false;
            Debug.Log("First Frame");
            SetRoomUserText();
            SetListOfPlayers();
        }

        if (Timer.start && !start)
        {
            GameStart();
        }
    }

    public void ButtonReady()
    {
        GameUI.SetActive(true);
        SetupUI.SetActive(false);

        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["is_ready"] = true;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        Debug.Log("Button Ready");

        if(!start && room.PlayerCount == room.MaxPlayers)
        {
            Debug.Log("ready, checking if game can start");
            int ReadyNum = 0;
            foreach(Player p in PhotonNetwork.PlayerList)
            {
                if((bool)p.CustomProperties["is_ready"]) 
                {
                    ReadyNum ++;
                }
                else break;
            }
            if(ReadyNum == room.MaxPlayers) 
            {
                hash = room.CustomProperties;
                hash["start"] = true;
                room.SetCustomProperties(hash);
            }
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if ((bool)propertiesThatChanged["start"])
        {
            Timer.prepared = true;
        }
    }

    public void GameStart()
    {
        Debug.Log("Start Game");
        if (PhotonNetwork.OfflineMode == false)
        {
            int i_pos = (int)PhotonNetwork.LocalPlayer.CustomProperties["position"];
            // if (i_pos <= 4)
            if (i_pos <= 3)
            {
                Debug.Log("get tf_target");
                Transform tf_target = PlayerSetting.TargetObject.GetComponent<Transform>();  //transform of objects to be moved;
                PhotonNetwork.Instantiate(PlayerSetting.TargetObject.name, tf_target.position, tf_target.rotation);
                Respawn();
            }
        }
        else
        {
            for (int i_pos = 0; i_pos <= 3; ++i_pos)
            {
                Debug.Log("get tf_target");
                Transform tf_target = DefaultList[i_pos].TargetObject.GetComponent<Transform>();  //transform of objects to be moved;
                PhotonNetwork.Instantiate(DefaultList[i_pos].TargetObject.name, tf_target.position, tf_target.rotation);
                
            }
            Respawn();
        }

        Debug.Log("start count down");
        Timer.StartCountDown();
        Debug.Log("start game: start = true");
        start = true;
        // added 22/07/11
        // remember deep copy!
        // SetListOfPlayers();
    }

    public void Respawn()
    {
        if (IsPlayer == false)
        {
            return;
        }
        if (room == null)
        {
            // string LocalRobotName = LocalRobot.name;
            Debug.Log("destroy former localrobot");
            Destroy(LocalRobot);
            Debug.Log("create new localrobot");
            // LocalRobot = Instantiate(TestPrefab, vec_restart, qua_restart);
            LocalRobot = Instantiate((GameObject)Resources.Load(TestPrefabName), vec_restart, qua_restart);

            Debug.Log("set target");
            CamSetTarget(LocalRobot);
            return;
        }

        PhotonNetwork.Destroy(LocalRobot);
        LocalRobot = PhotonNetwork.Instantiate(RobotName, PlayerSetting.Pos, PlayerSetting.Rot);
        LocalRobot.tag = RobotName;
        view.RPC("RobotTagger", RpcTarget.All, LocalRobot.tag);
        CamSetTarget(LocalRobot);
    }

    public void ChangeStartX()
    {
        float NewX = PlayerSetting.Pos.x;
        float.TryParse(posX.text, out NewX);  
        if(PlayerSetting.Pos.x == NewX || NewX == 0) return;
        
        PlayerSetting.Pos.x = NewX;
        Respawn();
    }

    public void ChangeStartY()
    {
        float NewY = PlayerSetting.Pos.y;
        float.TryParse(posY.text, out NewY);//C# method, returns false if fails to convert string to float, otherwise gives the new float value to var out;
        if(PlayerSetting.Pos.y == NewY || NewY == 0) return;
        
        PlayerSetting.Pos.y = NewY;
        Respawn();
    }

    public void ChangeStartZ()
    {
        float NewZ = PlayerSetting.Pos.z;
        float.TryParse(posZ.text, out NewZ);
        if(PlayerSetting.Pos.z == NewZ || NewZ == 0) return;
        
        PlayerSetting.Pos.z = NewZ;
        Respawn();
    }

    public void CamSetTarget(GameObject NewTarget)
    {
        if (playerCamFollow != null)
        {
            PlayerCamFollow cam = playerCamFollow.GetComponent<PlayerCamFollow>();
            if (cam != null)
            {
                cam.ChangeTarget(NewTarget);
                Debug.Log("PlayerCamFollow change target correctly");
            }
            else
            {
                Debug.Log("warning in GameManager: cam follow == null");
            }
        }
        else
        {
            Debug.Log("warning in GameManager: playerCamFollow == null");
        }

        if (playerCamMouse != null)
        {
            PlayerCamMouse cam = playerCamMouse.GetComponent<PlayerCamMouse>();
            if (cam != null)
            {
                cam.ChangeTarget(NewTarget);
                Debug.Log("PlayerCamMouse change target correctly");
            }
            else
            {
                Debug.Log("warning in GameManager: cam mouse == null");
            }
        }
        else
        {
            Debug.Log("warning in GameManager: playerCamMouse == null");
        }
    }

    private void SetRoomUserText()
    {
        Debug.Log("SetRoomUserText");
        string NickNameList = "current players in room: ";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            // NickNameList = NickNameList + player.NickName + " ";
            // Debug.Log("Mark #1 " + player.NickName);
            if ((int)player.CustomProperties["position"] <= 3)
            {
                if ((bool)player.CustomProperties["is_ready"])
                {
                    NickNameList = NickNameList + "\n" + player.NickName + ": ready";
                }
                else
                {
                    NickNameList = NickNameList + "\n" + player.NickName + ": not ready";
                }
            }
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((int)player.CustomProperties["position"] > 3)
            {
                if ((bool)player.CustomProperties["is_ready"])
                {
                    NickNameList = NickNameList + "\n" + player.NickName + ": ready";
                }
                else
                {
                    NickNameList = NickNameList + "\n" + player.NickName + ": not ready";
                }
            }
        }
        Debug.Log("NickNameList: " + NickNameList);
        TextRoomUser.text = NickNameList;
    }

    private void SetListOfPlayers()
    {
        // edited 22/07/15
        ListOfPlayers = new List<Player>();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if ((int)player.CustomProperties["position"] <= 3)
            {
                ListOfPlayers.Add(player);
            }
        }
        Debug.Log("Now Display ListOfPlayers II:");
        foreach (Player player in ListOfPlayers)
        {
            Debug.Log(player.NickName);
        }
    }

    /*public override void OnJoinedRoom()
    {
        Debug.Log("JoinedRoom");
        SetRoomUserText();
        SetListOfPlayers();
        // Debug.Log("JoinedRoom");
    }*/

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        SetRoomUserText();
        SetListOfPlayers();
        view.RPC("RobotTagger", RpcTarget.All, LocalRobot.tag);
        Debug.Log("Player Entered Room: " + newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        SetRoomUserText();
        SetListOfPlayers();
        Debug.Log("Player Left Room: " + otherPlayer.NickName);
    }

    public override void OnPlayerPropertiesUpdate(Player player, Hashtable hash)
    {
        SetRoomUserText();
    }
}