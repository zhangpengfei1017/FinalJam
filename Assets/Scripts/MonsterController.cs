using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : MonoBehaviour {

    [SerializeField]
    private int maxHP;

    public int curHP;

    [SerializeField]
    private int maxMP;

    public int curMP;

    [SerializeField]
    private int attack;

    [SerializeField]
    private int defense;

    //Property ratios

    private float speedRatio = 1;

    private float damageRatio = 1;

    private float healRatio = 1;

    private float cdRatio = 1;

    private float threatRatio = 1;

    private float attackRatio = 1;

    private float defenseRatio = 1;

    //Final properties

    public int finalMaxHP;

    public int finalMaxMP;

    private int finalAttack;

    private int finalDefense;

    // Use this for initialization
    void Start () {
        finalMaxHP = maxHP;
        finalMaxMP = maxMP;
        curHP = maxHP;
        curMP = maxMP;
	}
	
	// Update is called once per frame
	void Update () {
        curHP = Mathf.Clamp(curHP + 100, 0, finalMaxHP);
	}

    public void TakeSkill(Skill.CastedSkillStruct sck) {
        int otherAttack = sck.attack;
        Skill skill = sck.skill;
        otherAttack = Mathf.FloorToInt(Random.Range(otherAttack * 0.95f, otherAttack * 1.05f));
        int damage = Mathf.FloorToInt((skill.pctDamage * otherAttack + skill.fixedDamage) * (5000 / (5000 + finalDefense)) * damageRatio);
        curHP = Mathf.Clamp(curHP - damage, 0, finalMaxHP);
    }
}
