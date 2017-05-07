using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby_ChooseRoom : MonoBehaviour {
    public Color color;
    bool choose;
	// Use this for initialization
	void Start () {
        choose = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (choose) { 
		color = new Vector4(0.753f,0.682f,0.596f, 1); 
        this.GetComponent<UISprite>().color = color;
            foreach (Transform child in transform)
                child.GetComponent<UILabel>().color = new Vector4(0.122f, 0.102f, 0.09f, 1);
        }
        if (!choose)
        {
            color = new Vector4(0.122f, 0.102f, 0.09f, 1); 
            this.GetComponent<UISprite>().color = color;
            foreach (Transform child in transform)
                child.GetComponent<UILabel>().color = new Vector4(0.753f, 0.682f, 0.596f, 1);
        }
	}

    void OnClick()
    {
        choose = !choose;

    }
}
