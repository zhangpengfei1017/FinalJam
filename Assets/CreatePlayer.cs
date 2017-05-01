using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvancedUtilities.Cameras.Components;

public class CreatePlayer : MonoBehaviour {
    public Transform startPosition;
    public GameObject cam;

    void OnJoinedRoom()
    {
        CreatePlayerObject();
    }

    void CreatePlayerObject()
    {
        GameObject newPlayerObject = PhotonNetwork.Instantiate("Player", startPosition.position, Quaternion.identity, 0);
        cam = GameObject.Find("MainCam");
        cam.GetComponent<AdvancedUtilities.Cameras.BasicCameraController>().Target.Target = newPlayerObject.transform.GetChild(5).transform;
    }
}
