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
		color = new Vector4(0.859f,0.737f,0.47f, 1); 
        this.GetComponent<UISprite>().color = color;
            foreach (Transform child in transform)
                child.GetComponent<UILabel>().color = new Vector4(0.12f, 0.105f, 0.094f, 1);
        }
        if (!choose)
        {
            color = new Vector4(0.12f,0.105f,0.094f,1); 
            this.GetComponent<UISprite>().color = color;
            foreach (Transform child in transform)
                child.GetComponent<UILabel>().color = new Vector4(1,1,1, 1);
        }
	}

    void OnClick()
    {
        choose = !choose;

    }
}
