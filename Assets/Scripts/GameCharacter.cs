using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCharacter : MonoBehaviour
{
    public enum CharacterType
    {
        Player,
        Npc,
        Monster
    }

    #region Fields

    public enum TargetCheckResult
    {
        Available,
        NoTarget,
        TooFar,
        NotFaced,
        UnknowError
    }

    //Character Status

    [SerializeField]
    public CharacterType characterType = CharacterType.Monster;

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
    private float globalCDTime;

    private float globalCDTimer;


    //State flags

    private bool isDead;

    public int freezedCount = 0;
    
    public bool isFreezed
    {
        get
        {
            return freezedCount != 0;
        }
    }

    private bool isRestricted;

    private bool isCasting;

    private float castTimer;

    private bool isChanneling;

    private float channeledTimer;

    private float channeledInterval;

    private bool isInstant;

    private float instantTimer;

    //Property ratios

    private float speedRatio = 1;

    private float damageRatio = 1;

    private float healRatio = 1;

    private float cdRatio = 1;

    private float threatRatio = 1;

    private float attackRatio = 1;

    private float defenseRatio = 1;


    //Final properties

    private int finalMaxHP;

    private int finalMaxMP;

    private int finalAttack;

    private int finalDefense;

    //Target

    private GameObject target;

    //Component

    private Animator animator;

    private CharacterController charCtrl;

    private PhotonView photonView;

    [SerializeField]
    private Transform headPoint;

    public Transform characterCenter;


    //Skill

    [SerializeField]
    private Skill[] skillPrefabs;

    private List<Skill> skills;

    private Skill curCastSkill;

    //BUFF or DEBUFF

    private List<Buff> buffs;

    private List<GameObject> skillEffects = new List<GameObject>();

    public int MoveSpeed
    {
        get
        {
            return moveSpeed;
        }
    }

    public int CurHP
    {
        get
        {
            return curHP;
        }
    }

    public int MaxHP
    {
        get
        {
            return finalMaxHP;
        }
    }

    public bool IsAlive {
        get
        {
            return !isDead;
        }
    }

    #endregion

    #region UnityFunctions

    void Awake()
    {
        skills = new List<Skill>();
        buffs = new List<Buff>();

        foreach (Skill s in skillPrefabs)
        {
            GameObject skill = Instantiate(s.gameObject, transform) as GameObject;
            skills.Add(skill.GetComponent<Skill>());
        }
    }

    void Start()
    {
        charCtrl = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        globalCDTimer = 0;
        curHP = maxHP;
        curMP = maxMP;
        finalAttack = attack;
        finalDefense = defense;
        finalMaxHP = maxHP;
        finalMaxMP = maxMP;
        castTimer = 0;
        channeledInterval = 0;
        channeledTimer = 0;
        instantTimer = 0;
        isDead = false;
    }

    void Update()
    {
        //--------------------------------------
        //--------------test code---------------
        //--------------------------------------
        if (characterType == CharacterType.Player)
        {
            GameObject.Find("CD1").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(0) * 100).ToString() + "%";
            GameObject.Find("CD2").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(1) * 100).ToString() + "%";
            GameObject.Find("CD3").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(2) * 100).ToString() + "%";
            GameObject.Find("CD4").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(3) * 100).ToString() + "%";
            GameObject.Find("CD5").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(4) * 100).ToString() + "%";
            if (isCasting || isChanneling)
            {
                GameObject.Find("CastProgress").GetComponent<Text>().text = Mathf.FloorToInt(GetCastingProgress() * 100).ToString() + "%";
            }
            else
            {
                GameObject.Find("CastProgress").GetComponent<Text>().text = "";
            }
            GameObject.Find("HPBar").GetComponent<Text>().text = curHP.ToString() + "/" + finalMaxHP.ToString();
            GameObject.Find("MPBar").GetComponent<Text>().text = curMP.ToString() + "/" + finalMaxMP.ToString();
            Text targetName = GameObject.Find("TargetName").GetComponent<Text>();
            Text targetHp = GameObject.Find("TargetHPBar").GetComponent<Text>();
            Text targetMp = GameObject.Find("TargetMPBar").GetComponent<Text>();
            if (target != null)
            {
                targetHp.text = target.GetComponent<GameCharacter>().curHP.ToString() + "/" + target.GetComponent<GameCharacter>().finalMaxHP;
                targetMp.text = target.GetComponent<GameCharacter>().curMP.ToString() + "/" + target.GetComponent<GameCharacter>().finalMaxMP;
                targetName.text = target.name;
            }
            else
            {
                targetHp.text = "";
                targetMp.text = "";
                targetName.text = "";
            }
        }



        //--------------------------------------
        //--------------test code---------------
        //--------------------------------------
        UpdateState();
        UpdateBuffs();
        ResetAnimator();
    }

    #endregion

    #region Cast Skill Functions

    void UpdateInstanct()
    {
        if (instantTimer >= curCastSkill.instantDelayTime)
        {
            CastSkill(curCastSkill);
            EndInstant();
        }
    }

    void StartInstant(Skill skill)
    {
        curCastSkill = skill;
        isInstant = true;
    }

    void EndInstant()
    {
        curMP = Mathf.Clamp(curMP - curCastSkill.mpCost, 0, finalMaxMP);
        curCastSkill.CDTimer = curCastSkill.CDTime;
        curCastSkill = null;
        isInstant = false;
        skillEffects.Clear();
    }

    void UpdateCast()
    {
        if (CheckTarget(curCastSkill.targetType, curCastSkill.distance) == TargetCheckResult.Available)
        {
            if (castTimer >= curCastSkill.castTime)
            {
                CastSkill(curCastSkill);
                EndCasting(true);
            }
        }
        else
        {
            CancelCast(true);
        }
    }

    void StartCasting(Skill skill)
    {
        isCasting = true;
        curCastSkill = skill;
    }

    void EndCasting(bool cd)
    {
        if (cd)
        {
            curCastSkill.CDTimer = curCastSkill.CDTime;
            curMP = Mathf.Clamp(curMP - curCastSkill.mpCost, 0, finalMaxMP);
        }
        isCasting = false;
        curCastSkill = null;
        skillEffects.Clear();
    }

    void UpdateChanneled()
    {
        if (CheckTarget(curCastSkill.targetType, curCastSkill.distance) == TargetCheckResult.Available)
        {
            if (channeledInterval >= curCastSkill.channelInterval)
            {
                channeledInterval = 0;
                CastSkill(curCastSkill);
            }
            if (channeledTimer >= curCastSkill.channelTime)
            {
                EndChanneling();
            }
        }
        else
        {
            CancelCast(true);
        }

    }

    void StartChanneling(Skill skill)
    {
        isChanneling = true;
        curCastSkill = skill;
        curCastSkill.CDTimer = curCastSkill.CDTime;
        curMP = Mathf.Clamp(curMP - curCastSkill.mpCost, 0, finalMaxMP);
    }

    void EndChanneling()
    {
        isChanneling = false;
        curCastSkill = null;
        skillEffects.Clear();
    }

    #endregion

    #region Move Functions

    public void Move(Vector3 dir, float rotation, int d, float speed)
    {
        if (dir == Vector3.zero || this.isFreezed)
        {
            animator.SetBool("isRun", false);
            animator.SetInteger("moveDir", 0);
            return;
        }

        charCtrl.SimpleMove(dir.normalized * speed * moveSpeed * Time.deltaTime);
        Quaternion qRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, rotation, transform.rotation.eulerAngles.z);
        transform.rotation = qRotation;

        animator.SetBool("isRun", true);
        animator.SetInteger("moveDir", d);

        if (curCastSkill != null && curCastSkill.skillType != Skill.SkillType.Instant)
        {
            if (!curCastSkill.isMovingCast)
            {
                CancelCast(true);
            }
        }
    }

    #endregion

    #region Character State Functions

    void UpdateState()
    {
        if (globalCDTimer > 0)
        {
            globalCDTimer = Mathf.Clamp(globalCDTimer - Time.deltaTime, 0, globalCDTime);
        }

        foreach (Skill s in skills)
        {
            if (s.CDTimer > 0)
            {
                s.CDTimer = Mathf.Clamp(s.CDTimer - Time.deltaTime, 0, s.CDTime);
            }
        }

        if (isCasting)
        {
            castTimer += Time.deltaTime;
            UpdateCast();
        }
        else
        {
            castTimer = 0;
        }

        if (isChanneling)
        {
            channeledTimer += Time.deltaTime;
            channeledInterval += Time.deltaTime;
            UpdateChanneled();
        }
        else
        {
            channeledTimer = 0;
            channeledInterval = 0;
        }
        if (isInstant)
        {
            instantTimer += Time.deltaTime;
            UpdateInstanct();
        }
        else
        {
            instantTimer = 0;
        }
    }

    void ResetAnimator()
    {
        AnimatorStateInfo asi0 = animator.GetCurrentAnimatorStateInfo(0);
        if (asi0.IsTag("attack") && animator.GetInteger("attackIndex") != 0)
        {
            animator.SetInteger("attackIndex", 0);
        }
    }

    void OnDied()
    {

    }

    #endregion

    #region Buff Functions

    void AddBuff(Buff buff)
    {
        Buff temp = GetBuff(buff.name);

        if (temp)
        {
            temp.AddLevel();
        } else
        {
            GameObject g = Instantiate(buff.gameObject, transform) as GameObject;
            Buff newBuff = g.GetComponent<Buff>();
            buffs.Add(newBuff);
            newBuff.reset();
            newBuff.onEnter(this);
        }
    }

    void RemoveBuff(Buff buff)
    {
        buff.onExit(this);
        buffs.Remove(buff);
    }

    public Buff GetBuff(string buffName)
    {
        foreach (Buff b in buffs)
        {
            if (b.BuffName == buffName)
            {
                return b;
            }
        }
        return null;
    }

    void UpdateBuffs()
    {
        List<Buff> removes = new List<Buff>();

        foreach (Buff b in buffs)
        {
            b.duration -= Time.deltaTime;

            if (b.duration <= 0)
            {
                removes.Add(b);
                continue;
            }

            if (b.interval != 0)
            {
                b.intervalCount += Time.deltaTime;

                if (b.intervalCount > b.interval)
                {
                    b.intervalCount -= b.intervalCount;
                    b.onEffect(this);
                }
            }
        }

        foreach (Buff b in removes)
        {
            RemoveBuff(b);
        }
    }

    #endregion

    #region General Attack/Hit Functions

    public void Attack(int attackIndex)
    {
        if (attackIndex != -1)
        {
            if (globalCDTimer > 0 || skills[attackIndex].CDTimer > 0)
            {

            }
            else
            {
                PrepareAttack(attackIndex);
            }

        }
    }

    void PrepareAttack(int i)
    {
        Skill skill = skills[i];

        if (CheckTarget(skill.targetType, skill.distance) == TargetCheckResult.Available)
        {
            if (curCastSkill != null)
            {
                if (curCastSkill.skillType == Skill.SkillType.Channeled)
                {
                    CancelCast(true);
                }
                else
                {
                    return;
                }
            }
            if (curMP >= skill.mpCost)
            {
                switch (skill.skillType)
                {
                    case Skill.SkillType.Instant:
                        StartInstant(skill);
                        break;
                    case Skill.SkillType.Cast:
                        StartCasting(skill);
                        break;
                    case Skill.SkillType.Channeled:
                        StartChanneling(skill);
                        break;
                }
                animator.Play("Idle");
                animator.SetInteger("attackIndex", skill.animationIndex);
                globalCDTimer = globalCDTime;
            }
            else
            {
                print("No enough MP");
            }
        }
        else
        {
            print("wrong target");
        }
    }

    void CastSkill(Skill skill)
    {
        Skill.CastedSkillStruct sck = new Skill.CastedSkillStruct();
        sck.skill = skill;
        sck.attack = attack;
        sck.owner = gameObject;
        target.SendMessage("TakeSkill", sck);
    }

    public void CancelCast(bool self)
    {
        if (isCasting || isChanneling)
        {
            animator.SetInteger("attackIndex", 0);
            animator.Play("Idle");
            foreach (GameObject g in skillEffects)
            {
                if (g != null)
                {
                    Destroy(g);
                }
            }
            if (isCasting)
            {
                EndCasting(!self);
            }
            else if (isChanneling)
            {
                EndChanneling();
            }
            skillEffects.Clear();
            if (self)
            {
                print("Cancel");
            }
            else
            {
                print("Interrupt");
            }
        }
    }

    public void TakeSkill(Skill.CastedSkillStruct sck)
    {
        int otherAttack = sck.attack;
        Skill skill = sck.skill;

        otherAttack = Mathf.FloorToInt(Random.Range(otherAttack * 0.95f, otherAttack * 1.05f));
        int damage = Mathf.FloorToInt((skill.pctDamage * otherAttack + skill.fixedDamage) * (5000 / (5000 + (float)finalDefense)) * damageRatio);
        curHP = Mathf.Clamp(curHP - damage, 0, finalMaxHP);

        foreach (Buff b in sck.skill.buffs)
        {
            AddBuff(b);
        }
    }

    public void CreateSkillEffect()
    {
        if (curCastSkill != null)
        {
            GameObject effect = curCastSkill.effect;
            if (effect == null)
            {
                return;
            }
            SkillEffect e = effect.GetComponent<SkillEffect>();
            GameObject newEffect;
            Vector3 offset = e.offset;
            switch (e.skillEffectType)
            {
                case SkillEffect.SkillEffectType.mine:

                    newEffect = Instantiate(effect, transform.position + offset, transform.rotation) as GameObject;
                    skillEffects.Add(newEffect);
                    break;
                case SkillEffect.SkillEffectType.other:

                    newEffect = Instantiate(effect, target.transform.position + offset, target.transform.rotation, target.transform) as GameObject;
                    skillEffects.Add(newEffect);
                    break;
                case SkillEffect.SkillEffectType.move:

                    newEffect = Instantiate(effect, transform.position + offset.x * transform.right + offset.y * transform.up + offset.z * transform.forward, transform.rotation) as GameObject;
                    skillEffects.Add(newEffect);
                    newEffect.GetComponent<SkillEffect>().SetLine(gameObject, target);
                    break;
                case SkillEffect.SkillEffectType.ray:

                    newEffect = Instantiate(effect, transform.position + offset.x * transform.right + offset.y * transform.up + offset.z * transform.forward, transform.rotation) as GameObject;
                    newEffect.transform.LookAt(target.GetComponent<GameCharacter>().characterCenter);
                    skillEffects.Add(newEffect);
                    newEffect.GetComponent<SkillEffect>().SetLine(gameObject, target);
                    break;
            }
        }
    }

    void CleanEffects()
    {
        foreach (GameObject g in skillEffects)
        {
            if (g == null)
            {
                skillEffects.Remove(g);
            }
        }
    }

    #endregion

    #region Target Functions

    TargetCheckResult CheckTarget(GameCharacter.CharacterType t, float distance)
    {
        if (target == null)
        {
            return TargetCheckResult.NoTarget;
        }

        if (target.GetComponent<GameCharacter>().characterType != t)
        {
            return TargetCheckResult.UnknowError;
        }
        
        // click self
        if(Vector3.Equals(transform.position, target.transform.position))
        {
            return TargetCheckResult.Available;
        }

        if (Vector3.Distance(transform.position, target.transform.position) > distance)
        {
            return TargetCheckResult.TooFar;
        }

        Vector3 offest = target.transform.position - transform.position;
        if (Vector3.Angle(transform.forward, offest) > 80)
        {
            return TargetCheckResult.NotFaced;
        }

        return TargetCheckResult.Available;
    }

    public void SetTarget(GameObject target)
    {
        this.target = target;
    }

    public GameObject GetTarget()
    {
        return target;
    }

    #endregion

    #region Calculate Final Properties Functions

    void ChangeMaxHP(float ratio, int amount)
    {
        finalMaxHP += Mathf.FloorToInt(ratio * maxHP) + amount;
    }

    void ChangeMaxMP(float ratio, int amount)
    {
        finalMaxMP += Mathf.FloorToInt(ratio * maxMP) + amount;
    }

    void ChangeAttack(float ratio, int amount)
    {
        finalAttack += Mathf.FloorToInt(ratio * attack) + amount;
    }

    void ChangeDefense(float ratio, int amount)
    {
        finalDefense += Mathf.FloorToInt(ratio * defense) + amount;
    }

    #endregion

    #region Get Skill Information Functions

    public float GetCDProgress(int i)
    {
        float pGlobal = globalCDTimer / globalCDTime;
        float pSelf;
        if (skills[i].CDTime != 0)
        {
            pSelf = skills[i].CDTimer / skills[i].CDTime;
        }
        else
        {
            pSelf = 0;
        }

        return pGlobal > pSelf ? pGlobal : pSelf;
    }

    public float GetCastingProgress()
    {
        if (isCasting)
        {
            return castTimer / curCastSkill.castTime;
        }

        if (isChanneling)
        {
            return channeledTimer / curCastSkill.channelTime;
        }

        return 0;
    }

    #endregion
}
