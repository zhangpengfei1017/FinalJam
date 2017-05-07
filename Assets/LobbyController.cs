using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyController : MonoBehaviour
{

    public UILabel playerName;

    public UILabel createRoomName;

    public byte Version = 1;

    private GameObject currentRoom;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        PhotonNetwork.GetRoomList();
    }

    public void LogIn()
    {
        PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
    }

    public void ClickOnRoom(GameObject room)
    {
        currentRoom = room;
    }

    public void JoinRoom()
    {
        if (currentRoom != null) {
            //string roomName=currentRoom.GetComponent<RoomUI>().GetName();
            PhotonNetwork.JoinRoom("");
        }
    }
    public void CreateRoom()
    {
        if (createRoomName.text != "")
        {
            CreateRoomOnServer();
        }
        else
        {
            createRoomName.text = "Need a name!";
        }
    }

    void CreateRoomOnServer()
    {
        PhotonNetwork.CreateRoom(createRoomName.text, new RoomOptions() { MaxPlayers = 5 }, null);
    }
}
