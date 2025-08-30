using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable; 

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    public GameObject InitUI, RoomUI, PlayerUI, CreateOrJoinRoomUI, NumPlayers;
    //edited 22/07/01
    public GameObject ConnectingUI;
    public GameObject NumObs;

    public InputField RoomName, RobotName;
    public Dropdown Position, PlayerNum;
    //added 22/06/30
    public Dropdown ObNum;
    public string SceneToLoad;
    private bool IsCreateRoom;

    public GameObject JoinRoom;

    void Start()
    {
        InitUI.SetActive(true);
        RoomUI.SetActive(false);
        PlayerUI.SetActive(false);
        ConnectingUI.SetActive(false);
        CreateOrJoinRoomUI.SetActive(false);
    }

    public void ButtonSingle()
    {
        PhotonNetwork.OfflineMode = true;
        IsCreateRoom = true;
        InitUI.SetActive(false);
        PlayerUI.SetActive(true);
        CreateOrJoinRoomUI.SetActive(false);
    }

    public void ButtonMulti()
    {
        // edited 22/07/01
        InitUI.SetActive(false);
        PlayerUI.SetActive(false);
        RoomUI.SetActive(false);
        ConnectingUI.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomUI.SetActive(false);
        ConnectingUI.SetActive(false);
        CreateOrJoinRoomUI.SetActive(true);
    }

    public void ButtonCreateRoom()
    {
        CreateOrJoinRoomUI.SetActive(false);
        PlayerUI.SetActive(true);
        IsCreateRoom = true;
    }

    public void ButtonJoinRoom()
    {
        CreateOrJoinRoomUI.SetActive(false);
        PlayerUI.SetActive(true);
        NumObs.SetActive(false);
        NumPlayers.SetActive(false);
        IsCreateRoom = false;
    }
    
    public void ButtonPlayer()
    {
        PhotonNetwork.NickName = RobotName.text;
        RoomUI.SetActive(true);
        PlayerUI.SetActive(false);
        if(PhotonNetwork.OfflineMode)
        {
            NumPlayers.SetActive(false);
            // edited 22/07/01
            NumObs.SetActive(false);
        }
    }

    public void ButtonJoin()
    {
        if(RoomName.text.Length < 1)
            return;
        JoinRoom.SetActive(false);
        PlayerSetup();
        RoomOptions op = new RoomOptions();
        if(PhotonNetwork.OfflineMode)
        {
            op.MaxPlayers = (byte)1;
        }
        else
        {
            // op.MaxPlayers = (byte)((int)PlayerNum.value + 2); edited 22/06/30
            op.MaxPlayers = (byte)((int)PlayerNum.value + 1 + (int)ObNum.value);
        }
        
        if (IsCreateRoom)
        {
            PhotonNetwork.CreateRoom(RoomName.text, op, TypedLobby.Default);
        }
        else
        {
            PhotonNetwork.JoinRoom(RoomName.text);
        }
        
    }

    public override void OnJoinedRoom()
    {
        Hashtable hash = new Hashtable();
        hash.Add("start", false);  //flag that game has started;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

        PhotonNetwork.LoadLevel(SceneToLoad);
    }

    public void PlayerSetup()  //setting custom properties to transfer data to other scenes;
    {
        Hashtable hash = new Hashtable();
        hash.Add("position", Position.value);
        hash.Add("is_ready", false);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }
}
