using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_right : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnClick(){
        if(GameObject.Find("character option").GetComponent<Room_ChooseCharacter>().i < 2)
        GameObject.Find("character option").GetComponent<Room_ChooseCharacter>().i ++;
    }

}
