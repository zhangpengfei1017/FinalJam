using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    #region SkillInformation
    public enum SkillName
    {
        Attack,
        Skill
    }


    public enum SkillType
    {
        Instant,
        Cast,
        Channeled
    };

    public enum TargetType
    {
        Enemy,
        Teammate,
        AOE
    };

    public SkillName skillName = SkillName.Attack;

    public GameObject icon;

    public SkillType skillType = SkillType.Instant;

    public float castTime;

    private float castTimer;

    public float channelTime;

    private float channelTimer;

    public float channelInterval;

    public bool isMovingCast;

    public TargetType targetType = TargetType.Enemy;

    public int mpCost;

    public float CDtime;

    private float CDtimer;

    public int pctDamage;

    public int fixedDamage;

    public int pctHealth;

    public int fixedHealth;

    public int fixedThreat;

    public int maxTargets;

    public Buff[] buffs;

    public GameObject[] FXs;

    public void SkillEnter() { }

    public void Interrupted() { }

    public void SkillExit() { }

    public float GetCastingProgress() {
        return (castTimer / castTime);
    }

    public void Copy(Skill other) {

    }

    #endregion
}
