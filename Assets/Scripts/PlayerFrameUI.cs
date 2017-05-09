using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFrameUI : MonoBehaviour
{
    public UILabel playerName;

    public UISprite hp;

    public UISprite mp;

    public UIGrid buffList;

    public GameObject[] BuffPrefab;

    public GameObject[] classIcon;

    public int OwnerID;


    public void SetOwner(int id) {
        OwnerID = id;
    }
    public void SetClass(int i) {
        classIcon[i].SetActive(true);
    }
    public void SetPlayerName(string name) {
        playerName.text = name;
    }

    public void SetHpValue(float value) {
        hp.fillAmount = value;
    }
    public void SetMpValue(float value)
    {
        mp.fillAmount = value;
    }
    public void AddBuff(Buff b) {

    }
    public void RemoveBuff(Buff b) {

    }

}
