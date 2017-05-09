using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFrameUI : MonoBehaviour {

    public UILabel playerName;

    public UISprite hp;

    public UILabel hpPercent;

    public GameObject castBar;

    public UISprite castProgress;

    public UILabel castName;

    public UIGrid buffList;

    public GameObject[] BuffPrefab;



    public void SetPlayerName(string name)
    {
        playerName.text = name;
    }

    public void SetHpValue(float value)
    {
        hp.fillAmount = value;
        hpPercent.text = Mathf.FloorToInt(value * 100).ToString() + "%";
    }

    public void AddBuff(Buff b)
    {
        
    }
    public void RemoveBuff(Buff b)
    {

    }
    public void SetCast(string name,float value) {
        castBar.SetActive(true);
        castName.text = name;
        castProgress.fillAmount = value;
    }
    public void SetCast(bool active) {
        castBar.SetActive(active);
    }

}
