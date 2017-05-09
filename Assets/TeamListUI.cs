using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamListUI : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetTeammateInfo(int ID, string pName, float hp, float mp) {
        PlayerFrameUI[] pui = GetComponentsInChildren<PlayerFrameUI>();
        pui[ID].SetPlayerName(pName);
        pui[ID].SetHpValue(hp);
        pui[ID].SetMpValue(mp);
    }
}
