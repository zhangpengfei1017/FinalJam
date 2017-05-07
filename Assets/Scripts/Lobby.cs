﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

// This script is to take care of Lobby backend and is to be called by the UI.

public class Lobby : PunBehaviour {

    public static Lobby _instance;
    public GameObject RoomPanel;

    private string username;
    private string _selectedRoomName;

    private void Awake()
    {
        if (null == Lobby._instance)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }else
        {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start () {
        PhotonNetwork.ConnectUsingSettings("1.0f");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void setUserName(UILabel _uiText)
    {
        username = _uiText.text;
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
    }

    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate();
        // Get and Refresh Rooms List here.
        int i = 1;
        Debug.Log("dsa");
        Debug.Log(PhotonNetwork.GetRoomList().Length);
        foreach (var item in PhotonNetwork.GetRoomList())
        {
            Vector3 tempPos = new Vector3(RoomPanel.transform.position.x, RoomPanel.transform.position.y - ( i * 0.15f), RoomPanel.transform.position.z);
            GameObject newObj = Instantiate(RoomPanel, tempPos, Quaternion.identity, RoomPanel.transform.parent);
            newObj.transform.FindChild("Name").GetComponent<UILabel>().text = item.Name;
            newObj.transform.FindChild("Players").GetComponent<UILabel>().text = "" + item.PlayerCount + " / " + item.MaxPlayers;
            Debug.Log(newObj);
            ++i;
        }
    }

    public void SelectRoom(UILabel _text)
    {
        _selectedRoomName = _text.text;
        Debug.Log(_selectedRoomName);
    }

    public void CreateRoomButton(UILabel _text)
    {
        RoomOptions RO = new RoomOptions();
        RO.MaxPlayers = 5;
        bool check = PhotonNetwork.CreateRoom(_text.text, RO, TypedLobby.Default);
        Debug.Log(check);
    }

    public void JoinRoom()
    {
        if (null != _selectedRoomName)
        {
            PhotonNetwork.JoinRoom(_selectedRoomName);
        }
        else
            PhotonNetwork.JoinRandomRoom(); // Should we do this?
    }
}
