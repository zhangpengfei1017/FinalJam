using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_left : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnClick()
    {
        if (GameObject.Find("character option").GetComponent<Room_ChooseCharacter>().i > -1)
            GameObject.Find("character option").GetComponent<Room_ChooseCharacter>().i--;
    }
}
