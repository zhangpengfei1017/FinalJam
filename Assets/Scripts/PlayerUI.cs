using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour {

    public UILabel nameLabel;

    public UILabel readyLabel;

    public UILabel classLabel;

    public GameObject[] icons;

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
                icons[0].SetActive(true);
                icons[1].SetActive(false);
                icons[2].SetActive(false);
                icons[3].SetActive(false);
                break;
            case HeroController.Class.Knight:
                classLabel.text = "Knight";
                icons[0].SetActive(false);
                icons[1].SetActive(true);
                icons[2].SetActive(false);
                icons[3].SetActive(false);
                break;
            case HeroController.Class.Priest:
                classLabel.text = "Priest";
                icons[0].SetActive(false);
                icons[1].SetActive(false);
                icons[2].SetActive(true);
                icons[3].SetActive(false);
                break;
            case HeroController.Class.Sorceress:
                classLabel.text = "Sorceress";
                icons[0].SetActive(false);
                icons[1].SetActive(false);
                icons[2].SetActive(false);
                icons[3].SetActive(true);
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
