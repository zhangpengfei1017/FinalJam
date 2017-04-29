﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    #region SkillInformation
    public enum SkillName
    {
        S_Skill1,
        S_Skill2,
        S_Skill3,
        S_Skill4,
        S_Skill5,
    }


    public enum SkillType
    {
        Instant,
        Cast,
        Channeled
    };



    public SkillName skillName = SkillName.S_Skill1;

    public GameObject icon;

    public int animationIndex;

    public SkillType skillType = SkillType.Instant;

    public float castTime;

    private float castTimer;

    public float channelTime;

    private float channelTimer;

    public float distance;

    public float channelInterval;

    public bool isMovingCast;

    public GameCharacter.CharacterType targetType = GameCharacter.CharacterType.Monster;

    public int mpCost;

    public float CDTime;

    public float CDTimer;

    public int pctDamage;

    public int fixedDamage;

    public int pctHealth;

    public int fixedHealth;

    public int fixedThreat;

    public int maxTargets;

    public Buff[] buffs;

    public GameObject effect;

    public float GetCastingProgress() {
        return (castTimer / castTime);
    }

    public void Copy(Skill other) {

    }

    #endregion
}