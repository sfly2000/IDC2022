// Created by Gzy in 22/07/01
// to controll the RoomUI to display room infos(room name, room users...),
// turn on/off the room UI with the key(Tab);
// Get room info with Photon and updating text is in GameManager.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UIRoom : MonoBehaviour
{
    public GameObject RoomUI;
    public GameObject RoomNameText;
    public GameObject RoomUserText;
    public string keyShowUI = "tab";
    public string keyCloseUI = "tab";

    // Start is called before the first frame update
    void Start()
    {
        RoomUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(keyShowUI))
        {
            RoomUI.SetActive(false);
        }
        else if (Input.GetKeyDown(keyCloseUI)) {
            RoomUI.SetActive(true);
        }
    }

}
