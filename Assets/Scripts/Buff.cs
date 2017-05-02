using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff : MonoBehaviour
{
    public enum BuffType
    {
        None,
        Freezed,
        Burn
    };

    #region BuffInfomation

    public string BuffName;

    public BuffType type;

    public Texture2D icon;

    public float duration;

    [HideInInspector]
    public float count;

    public float interval;

    [HideInInspector]
    public float intervalCount;

    public int maxLevel;

    //[HideInInspector]
    public int level;

    #endregion

    #region functions
    //void Start() {
    //    reset();
    //}

    public void reset()
    {
        count = duration;
        intervalCount = interval;
    }

    public virtual void onEnter(GameCharacter target) {
        Debug.LogError("error!");
    }

    public virtual void onEffect(GameCharacter target)
    {
        Debug.LogError("error!");

        //if (target.tag == "Player")
        //{
        //    HeroController hc = target.GetComponent<HeroController>();
        //    //
        //    switch (buffName) {
        //        case BuffName.Freezed:
        //            break;
        //        case BuffName.Burn:
        //            break;
        //    }
        //}
        //else if (target.tag == "Enemy")
        //{
        //    //EnemyController ec = target.GetComponent<EnemyController>();
        //    //
        //    switch (buffName)
        //    {
        //        case BuffName.Freezed:
        //            break;
        //        case BuffName.Burn:
        //            break;
        //    }
        //}
    }

    public virtual void onExit(GameCharacter target) {
        Debug.LogError("error!");
    }

    public void AddLevel() {
        level = Mathf.Clamp(++level, 0, maxLevel);
        reset();
    }

    //public void Copy(Buff other) {
    //    this.buffName = other.buffName;
    //    this.icon=other.icon;
    //    //this.lastTime=other.lastTime;
    //    this.interval=other.interval;
    //    this.maxLevels=other.maxLevels;
    //}

    #endregion
}
