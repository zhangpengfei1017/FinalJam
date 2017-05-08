using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomUI : MonoBehaviour
{

    public string roomName;

    public UILabel rName;

    public UILabel rPlayers;

    public GameObject clickBackground;

    public Color regularColor;

    public Color OnClickColor;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetInfo(string name, int players)
    {
        rName.text = name;
        roomName = name;
        rPlayers.text = players.ToString() + "/5";
    }
    public void Clicked() {
        clickBackground.SetActive(true);
        UILabel[] lbs = GetComponentsInChildren<UILabel>();
        foreach (UILabel u in lbs) {
            u.color = OnClickColor;
        }
        Lobby._instance.SelectRoom(this);
    }
    public void UnClicked() {
        clickBackground.SetActive(false);
        UILabel[] lbs = GetComponentsInChildren<UILabel>();
        foreach (UILabel u in lbs)
        {
            u.color = regularColor;
        }       
    }
}
