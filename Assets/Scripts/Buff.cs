using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public enum BuffName
    {
        Freezed,
        Burn
    };
    #region BuffInfomation

    public BuffName buffName = BuffName.Freezed;

    public GameObject icon;

    public float lastTime;

    private float lastTimer;

    public float interval;

    public int maxLevels;

    private int curLevels;


    #endregion

    #region functions
    void Start() {
        lastTimer = lastTime;
    }

    public void BuffEnter(GameObject target) { }

    public void BuffEffect(GameObject target)
    {
        if (target.tag == "Player")
        {
            HeroController hc = target.GetComponent<HeroController>();
            //
            switch (buffName) {
                case BuffName.Freezed:
                    break;
                case BuffName.Burn:
                    break;
            }









        //
        }
        else if (target.tag == "Enemy")
        {
            //EnemyController ec = target.GetComponent<EnemyController>();
            //
            switch (buffName)
            {
                case BuffName.Freezed:
                    break;
                case BuffName.Burn:
                    break;
            }







            //
        }

    }

    public void BuffExit(GameObject target) { }

    public void AddLevel() {
        curLevels = Mathf.Clamp(curLevels + 1, 0, maxLevels);
        Refresh();
    }
    public void Refresh() {
        lastTimer = lastTime;
    }

    public void Copy(Buff other) {
        this.buffName = other.buffName;
        this.icon=other.icon;
        this.lastTime=other.lastTime;
        this.interval=other.interval;
        this.maxLevels=other.maxLevels;

}
    #endregion
}
