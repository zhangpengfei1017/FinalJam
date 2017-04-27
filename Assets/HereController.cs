using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HereController : MonoBehaviour
{
    #region Fields
    public enum Class
    {
        Knight,
        Sorceress,
        Priest,
        Archer,
    };

    //Character Status

    [SerializeField]
    private string playerName;

    [SerializeField]
    private Class className = Class.Knight;

    [SerializeField]
    private int maxHP;

    private int curHP;

    [SerializeField]
    private int maxMP;

    private int curMP;

    [SerializeField]
    private int attack;

    [SerializeField]
    private int defense;

    [SerializeField]
    private int moveSpeed;

    [SerializeField]
    private int globalCDTime;

    private int globalCDTimer;


    //State flags

    private bool isDead;

    private bool isFreezed;

    private bool isRestricted;

    //Property ratios

    private float speedRatio;

    private float damageRatio;

    private float healRatio;

    private float cdRatio;

    private float threatRatio;

    private float attackRatio;

    private float defenseRatio;

    private float maxHPRatio;

    private float maxMPRatio;

    //Property temps

    private int maxHPTemp;

    private int maxMPTemp;

    private int attackTemp;

    private int defenseTemp;




    //Target

    private GameObject target;

    //Component

    private Animator animator;

    private CharacterController charCtrl;

    private PhotonView photonView;

    private Transform headPoint;

    private GameObject cam;

    private UIController uiCtrl;

    //Skill

    [SerializeField]
    private Skill[] skills;

    private Skill curCastSkill;

    //BUFF or DEBUFF

    private List<Buff> buffs;



    #endregion

    #region UnityFunctions
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ResetAnimator();
        if (!isDead)
        {
            //
            //
            if (!isFreezed)
            {
                //
                //
                Attack();
                if (!isRestricted)
                {
                    //
                    //
                    Move();
                }
            }
        }
    }
    #endregion

    void Move() { }

    void Attack()
    {

    }

    void CastSkill(Skill skill)
    {
        if (skill.targetType == Skill.TargetType.AOE)
        {
            if (skill.skillType == Skill.SkillType.Instant)
            {

            }
            else if (skill.skillType == Skill.SkillType.Cast)
            {

            }
            else if (skill.skillType == Skill.SkillType.Channeled)
            {

            }
        }
        else
        {
            if (skill.skillType == Skill.SkillType.Instant)
            {
                if (CheckTarget())
                {
                    if (target.tag == "Player")
                    {

                    }
                    else if (target.tag == "Enemy")
                    {
                        target.GetComponent<HereController>().TakeSkill(attack, skill);
                    }
                    Instantiate(skill.gameObject, transform.position, skill.transform.rotation);
                }
            }
            else if (skill.skillType == Skill.SkillType.Cast)
            {
                StartCasting();
            }
            else if (skill.skillType == Skill.SkillType.Channeled)
            {
                StartChanneling();
            }
        }
    }

    bool CheckTarget()
    {
        if (target == null)
        {
            return false;
        }

        //distance
        //direction
        //isDead?
        //
        return true;
    }

    void UpdateBuff()
    {
        foreach (Buff b in buffs)
        {
            b.BuffEffect(gameObject);
        }
    }

    void UpdateState() { }

    void OnDied() { }

    void ResetAnimator()
    {

    }

    public void TakeSkill(int enemyAttack, Skill skill)
    {
        int finalMaxHP = Mathf.FloorToInt(maxHP * maxHPRatio + maxHPTemp);
        int damage = Mathf.FloorToInt((skill.pctDamage * enemyAttack + skill.fixedDamage) * (5000 / (5000 + defense)) * damageRatio);
        int health = Mathf.FloorToInt((skill.pctHealth * finalMaxHP + skill.fixedHealth) * healRatio);
        curHP = Mathf.Clamp(curHP - damage + health, 0, finalMaxHP);
        foreach (Buff b in skill.buffs)
        {
            Buff temp = new Buff();
            temp.Copy(b);         
            AddBuff(temp);
        }
    }

    void StartCasting()
    {
        //ui show cast prograss bar
    }

    void StartChanneling()
    {
        //ui show cast prograss bar
    }

    void AddBuff(Buff buff)
    {
        foreach (Buff b in buffs) {
            if (buff.buffName == b.buffName) {
                b.AddLevel();
                return;
            }
        }
        buffs.Add(buff);
        buff.BuffEnter(gameObject);
    }

    void RemoveBuff(Buff buff)
    {
        buff.BuffExit(gameObject);
        buffs.Remove(buff);
    }

    public bool HasBuff(Buff.BuffName buffName)
    {
        foreach (Buff b in buffs)
        {
            if (b.buffName == buffName)
            {
                return true;
            }
        }
        return false;
    }

    public float GetCastingProgress() {
        if (curCastSkill == null) {
            return 0;
        }
        return curCastSkill.GetCastingProgress();
    }
}
