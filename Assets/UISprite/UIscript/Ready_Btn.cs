using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ready_Btn : MonoBehaviour {
   // public UISprite buttonSprite;
   // public UISprite unreadySprite;
    bool ready;

    // Use this for initialization
    void Start () {
        ready = false;
	}
	
	// Update is called once per frame
	void Update () {
		 if (!ready)
        {
            this.GetComponent<UISprite>().spriteName="unready";

        }
        if (ready)
        {
            this.GetComponent<UISprite>().spriteName = "ready";
        }
	}

    void OnClick()
    {
       
        ready = !ready;
    }
}
