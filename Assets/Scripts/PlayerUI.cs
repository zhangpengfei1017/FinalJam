using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    public UILabel nameLabel;

    public UILabel readyLabel;

    public UILabel classLabel;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}


    public void SetInfo(string name, HeroController.Class c,bool ready) {
        nameLabel.text = name;
        switch (c) {
            case HeroController.Class.Archer:
                classLabel.text = "Archer";
                break;
            case HeroController.Class.Knight:
                classLabel.text = "Knight";
                break;
            case HeroController.Class.Priest:
                classLabel.text = "Priest";
                break;
            case HeroController.Class.Sorceress:
                classLabel.text = "Sorceress";
                break;
        }
        if (ready)
        {
            readyLabel.text = "Ready";
            readyLabel.color = Color.green;
        }
        else {
            readyLabel.text = "Unready";
            readyLabel.color = Color.red;
        }
    }
}
