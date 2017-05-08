using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomPlayer : Photon.PunBehaviour, IPunObservable
{

    public string Name;

    public HeroController.Class playerClass;

    public bool isReady = false;

    public PlayerUI playerUI;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        if (playerUI != null) {
            playerUI.SetInfo(Name, playerClass, isReady);
        }
	}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(Name);
            stream.SendNext(playerClass);
            stream.SendNext(isReady);
        }
        else {
            Name = (string)stream.ReceiveNext();
            playerClass = (HeroController.Class)stream.ReceiveNext();
            isReady = (bool)stream.ReceiveNext();
        }
    }
    public void SetInfo(string name, HeroController.Class c, bool ready) {
        Name = name;
        playerClass = c;
        isReady = ready;
    }
}
