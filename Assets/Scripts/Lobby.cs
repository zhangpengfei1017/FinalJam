using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This script is to take care of Lobby backend and is to be called by the UI.

public class Lobby : PunBehaviour {

    public static Lobby _instance;

    public GameObject LoginObject;
    public GameObject RoomListObj;

    public GameObject roomPrefab;
    public UIGrid roomList;

    public GameObject CreateRoomPopup;

    private string username;
    private string _selectedRoomName;
    private RoomUI currentClicked;

    private void Awake()
    {
        if (null == Lobby._instance)
        {
            _instance = this;
        }else
        {
            Destroy(this);
        }
        PlayerInfo pi = new PlayerInfo();
        PlayerInfo.instance = pi;
    }

    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        roomList.Reposition();
	}

    public void setUserName(UILabel _uiText)
    {
        username = _uiText.text;
        PlayerInfo.instance.playerName = _uiText.text;
        if (username == "") {
            LobbyMessage.instance.Msg("you need a name!");
            return;
        }
        PhotonNetwork.ConnectUsingSettings("1.0f");
        PhotonNetwork.autoJoinLobby = true;
        
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        LoginObject.SetActive(false);
        RoomListObj.SetActive(true);
        LobbyMessage.instance.Msg("Joined Lobby");
    }


    public override void OnReceivedRoomListUpdate()
    {
        base.OnReceivedRoomListUpdate();
        // Get and Refresh Rooms List here.
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        foreach (Transform t in roomList.GetChildList())
        {
            roomList.RemoveChild(t);
            Destroy(t.gameObject);
        }
        bool notLostRoom = false;
        foreach (RoomInfo r in rooms)
        {
            GameObject go= Instantiate(roomPrefab,LobbyMessage.instance.transform.parent);
            roomList.AddChild(go.transform);
            go.GetComponent<RoomUI>().SetInfo(r.Name, r.PlayerCount);
            if (r.Name == _selectedRoomName) {
                go.GetComponent<RoomUI>().Clicked();            
                notLostRoom = true;
            }
        }
        if (!notLostRoom) {
            _selectedRoomName = "";
            currentClicked = null;
        }           
    }


    public void SelectRoom(RoomUI r)
    {
        _selectedRoomName = r.roomName;

        if (currentClicked != null && currentClicked!=r)
        {
            currentClicked.UnClicked();
        }
        currentClicked = r;
    }

    public void CreateRoomButton()
    {

        if ( PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            PhotonNetwork.ConnectUsingSettings("1.0f");
        }

        RoomOptions RO = new RoomOptions();
        RO.MaxPlayers = 5;
        bool check = PhotonNetwork.CreateRoom(username+"'s game", RO, TypedLobby.Default);
        if (!check)
        {
            LobbyMessage.instance.Msg("Create room failed");
        }
    }

    public void JoinRoom()
    {
        if (null != _selectedRoomName)
        {
            if (!PhotonNetwork.JoinRoom(_selectedRoomName)) {
                LobbyMessage.instance.Msg("Join failed!");
            }            
        }
        else {
            LobbyMessage.instance.Msg("Please select a room!");
        }
    }

    public override void OnJoinedRoom() {
        base.OnJoinedRoom();
        PhotonNetwork.LoadLevel(1);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
