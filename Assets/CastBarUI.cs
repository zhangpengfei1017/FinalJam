using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastBarUI : MonoBehaviour {

    public UISprite progress;
    public UILabel skillName;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetValue(float value, string name) {
        progress.fillAmount = value;
        skillName.text = name;
    }
}
