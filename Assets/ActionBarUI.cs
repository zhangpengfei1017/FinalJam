using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionBarUI : MonoBehaviour {

    public UISprite[] cdfill;

    public GameObject[] classSkills;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void SetCD(int i,float value) {
        cdfill[i].fillAmount = value;
    }

    public void SetClass(int i) {
        classSkills[i].SetActive(true);
    }

    
}
