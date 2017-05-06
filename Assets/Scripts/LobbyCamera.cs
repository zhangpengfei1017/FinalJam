using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyCamera : MonoBehaviour {

    public Transform LoginObject;
    public Transform LobbyObj;
    public Transform CreateRoomPopup;
    public Transform RoomObj;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MoveToLobby()
    {
        transform.position = LobbyObj.position;
    }

    public void MoveToCreateRoomPopup()
    {
        transform.position = CreateRoomPopup.position;
    }

    public void MoveToRoom()
    {
        transform.position = RoomObj.transform.position;
    }
}
