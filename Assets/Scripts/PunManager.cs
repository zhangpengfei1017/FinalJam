using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PunManager : PunBehaviour {

    public static PunManager _instance;

    private void Awake()
    {
        if ( null == _instance)
        {
            _instance = this;
        }else
        {
            Destroy(this.transform);
        }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
