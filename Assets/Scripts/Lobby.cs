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

    public GameObject RoomPanel;

    public GameObject CreateRoomPopup;

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
        LoginObject.SetActive(false);
        RoomListObj.SetActive(true);
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

        if ( PhotonNetwork.connectionState == ConnectionState.Disconnected)
        {
            PhotonNetwork.ConnectUsingSettings("1.0f");
        }

        RoomOptions RO = new RoomOptions();
        RO.MaxPlayers = 5;
        bool check = PhotonNetwork.CreateRoom(_text.text, RO, TypedLobby.Default);
        if (check)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void JoinRoom()
    {
        bool check;
        if (null != _selectedRoomName)
        {
            check = PhotonNetwork.JoinRoom(_selectedRoomName);
        }
        else
            check = PhotonNetwork.JoinRandomRoom(); // Should we do this?

        if (check) SceneManager.LoadScene(1);
    }

    public void OpenCreateRoomPopup()
    {
        RoomListObj.SetActive(false);
        CreateRoomPopup.SetActive(true);
    }
}
