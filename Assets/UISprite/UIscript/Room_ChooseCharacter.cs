using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room_ChooseCharacter : MonoBehaviour {
    int characternum;
    public string[] characters;
    public int i;
	// Use this for initialization
	void Start () {
        characternum = 0;
        i = 0;
	}
	
	// Update is called once per frame
	void Update () {
        this.GetComponentInChildren<UILabel>().text = characters[i];
        GameObject.Find("characterinfo name").GetComponent<UILabel>().text= characters[i];
    }

   

}
