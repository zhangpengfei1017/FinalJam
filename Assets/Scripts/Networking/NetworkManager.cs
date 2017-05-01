using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class NetworkManager : PunBehaviour {

    public static NetworkManager instance;

    private void Awake()
    {
        if(null == instance)
        {
            instance = this;
        }else if ( this != instance)
        {
            Destroy(GetComponent<GameObject>());
        }
    }

    // Use this for initialization
    void Start () {
        PhotonNetwork.ConnectUsingSettings("1.0");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnJoinedLobby()
    {
        RoomOptions RO = new RoomOptions();
        RO.MaxPlayers = byte.Parse("5");
        PhotonNetwork.CreateRoom("Jaffa", RO, TypedLobby.Default);
    }

    public override void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        base.OnPhotonJoinRoomFailed(codeAndMsg);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.Instantiate("Player", new Vector3(339.3329f, 55.56915f, 234.3181f), Quaternion.identity, 0);
    }
}
