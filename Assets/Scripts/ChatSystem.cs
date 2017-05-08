using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatSystem : Photon.PunBehaviour
{
    public UILabel history;

    public UILabel input;

    public UIInput inputField;

    public UIProgressBar inputProgress;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Chat()
    {
        inputField.RemoveFocus();
        if (input.text == "") {
            return;
        }
        string msg = PlayerInfo.instance.playerName + ":" + input.text;
        photonView.RPC("ChatToAll", PhotonTargets.AllViaServer, msg);
        inputField.Set("");         
    }

    [PunRPC]
    public void ChatToAll(string msg)
    {
        history.text = history.text + "\r\n" + msg ;
        inputProgress.value = 1;
    }

    public void SystemMsg(string msg) {
        history.text = history.text + "\r\n" + msg ;
        inputProgress.value = 1;
    }
}
