using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectClass : MonoBehaviour {
    public Transform startPosition;
    public GameObject cam;
    public InputField nameField;


    // Use this for initialization
    void Start () {
        
    }



    // Update is called once per frame
    void Update () {
		
	}

    public void ChoosePriest() {
        GameObject newPlayerObject = PhotonNetwork.Instantiate("Player_Priest", startPosition.position, Quaternion.identity, 0);
        cam = GameObject.Find("MainCam");
        cam.GetComponent<AdvancedUtilities.Cameras.BasicCameraController>().Target.Target = newPlayerObject.transform.FindChild("headPoint").transform;
        if (nameField.text != "")
        {
            newPlayerObject.GetComponent<GameCharacter>().characterName = nameField.text;
        }
        else {
            newPlayerObject.GetComponent<GameCharacter>().characterName = "Unknown";
        }
        
        gameObject.SetActive(false);
    }
    public void ChooseSorceress() {
        GameObject newPlayerObject = PhotonNetwork.Instantiate("Player_Sorceress", startPosition.position, Quaternion.identity, 0);
        cam = GameObject.Find("MainCam");
        cam.GetComponent<AdvancedUtilities.Cameras.BasicCameraController>().Target.Target = newPlayerObject.transform.FindChild("headPoint").transform;
        if (nameField.text != "")
        {
            newPlayerObject.GetComponent<GameCharacter>().characterName = nameField.text;
        }
        else
        {
            newPlayerObject.GetComponent<GameCharacter>().characterName = "Unknown";
        }
        gameObject.SetActive(false);

    }
}
