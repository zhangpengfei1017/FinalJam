using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using UnityEngine.UI;

public class LobbyUser : PunBehaviour {

    public static LobbyUser _instance;

    private void Awake()
    {
        if (null == _instance)
        {
            _instance = this;
        }
        else
        {
            Destroy(this.transform);
        }
    }

    public string name;

    // Use this for initialization
    void Start () {
        name = "noddy";
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(name);
	}

    public void SetName(Text inputName )
    {
        name = inputName.text;
        // Connect to the Lobby here and check all the rooms.
    }
}
