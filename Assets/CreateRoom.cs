using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoom : MonoBehaviour {
    public InputField roomInput;

    public void CreateNewRoom() {
        PhotonNetwork.CreateRoom(roomInput.text, new RoomOptions() { MaxPlayers = 2 }, null);
    }
    public void JoinRoom() {
        PhotonNetwork.JoinRoom(roomInput.text);
    }
}
