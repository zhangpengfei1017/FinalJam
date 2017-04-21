using UnityEngine;
using System.Collections;

public class restart : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R)) {
            Restart();
        }
        if (Input.GetKeyDown(KeyCode.E)) {
            Application.Quit();
        }	
	}
    public void Restart() {
        Application.LoadLevel(0);
        Time.timeScale = 1;
    }
}
