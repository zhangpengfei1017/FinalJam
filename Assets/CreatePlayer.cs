using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedUtilities.Cameras.Components;

public class CreatePlayer : MonoBehaviour {
    public GameObject selectClassUI;
    public GameObject createRoomUI;
    public byte Version = 1;

    void Start() {
        PhotonNetwork.ConnectUsingSettings(Version + "." + SceneManagerHelper.ActiveSceneBuildIndex);
    }

    void OnJoinedRoom()
    {
        createRoomUI.SetActive(false);
        selectClassUI.SetActive(true);
    }

    public virtual void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN. Now this client is connected and could join a room. Calling: PhotonNetwork.JoinRandomRoom();");
        createRoomUI.SetActive(true);
    }



}
