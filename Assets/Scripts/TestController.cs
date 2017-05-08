using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : Photon.PunBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (!photonView.isMine) {
            return;
        }
        if (Input.GetKey(KeyCode.A)) {
            transform.position += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            transform.position -= new Vector3(1, 0, 0);
        }
	}
}
