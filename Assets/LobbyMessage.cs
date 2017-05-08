using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMessage : MonoBehaviour {
    public static LobbyMessage instance;
    void Awake() {
        instance = this;
        
    }

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Msg(string msg) {
        GetComponent<UILabel>().text = msg;
    }
}
