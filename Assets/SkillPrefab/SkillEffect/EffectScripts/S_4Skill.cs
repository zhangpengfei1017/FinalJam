using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_4Skill : MonoBehaviour {
    public LineRenderer l1;
    public LineRenderer l2;
    SkillEffect se;

    // Use this for initialization
    void Start () {
        se = GetComponent<SkillEffect>();
        float length = se.GetRayLength();
        l1.SetPosition(1, new Vector3(length, 0, 0));
        l2.SetPosition(1, new Vector3(length, 0, 0));
    }
	
	// Update is called once per frame
	void Update () {
        
		
	}
}
