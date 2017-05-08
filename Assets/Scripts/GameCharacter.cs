using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameCharacter : Photon.MonoBehaviour, IPunObservable
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
    public string characterName;

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

    [HideInInspector]
    public int freezedCount = 0;

    public bool isFreezed
    {
        get
        {
            return freezedCount != 0;
        }
    }

    public bool isBusy
    {
        get
        {
            return globalCDTime != 0 || isCasting || isChanneling || isFreezed;
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

    private GameCharacter target;

    //Component

    private CharacterController charCtrl;

    [SerializeField]
    private Transform headPoint;

    public Transform characterCenter;

    private NewPlayerController pc;


    //Skill

    [SerializeField]
    private Skill[] skillPrefabs;

    public List<Skill> skills;

    private Skill curCastSkill;

    private int curSkillIndex;

    private bool deleteCurSkill;

    private float delayTimer;

    //BUFF or DEBUFF

    private List<Buff> buffs;

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

        set
        {
            curHP = value;
        }
    }

    public int MaxHP
    {
        get
        {
            return finalMaxHP;
        }
    }

    public bool IsAlive
    {
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
        pc = GetComponent<NewPlayerController>();
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
        delayTimer = 0;
        deleteCurSkill = false;
        isDead = false;
    }

    void Update()
    {

        //--------------------------------------
        //--------------test code---------------
        //--------------------------------------
        //if (characterType == CharacterType.Player && photonView.isMine)
        //{
        //    GameObject.Find("CD1").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(0) * 100).ToString() + "%";
        //    GameObject.Find("CD2").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(1) * 100).ToString() + "%";
        //    GameObject.Find("CD3").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(2) * 100).ToString() + "%";
        //    GameObject.Find("CD4").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(3) * 100).ToString() + "%";
        //    GameObject.Find("CD5").GetComponent<Text>().text = Mathf.FloorToInt(GetCDProgress(4) * 100).ToString() + "%";
        //    if (isCasting || isChanneling)
        //    {
        //        GameObject.Find("CastProgress").GetComponent<Text>().text = Mathf.FloorToInt(GetCastingProgress() * 100).ToString() + "%";
        //    }
        //    else
        //    {
        //        GameObject.Find("CastProgress").GetComponent<Text>().text = "";
        //    }
        //    GameObject.Find("HPBar").GetComponent<Text>().text = curHP.ToString() + "/" + finalMaxHP.ToString();
        //    GameObject.Find("MPBar").GetComponent<Text>().text = curMP.ToString() + "/" + finalMaxMP.ToString();
        //    Text targetName = GameObject.Find("TargetName").GetComponent<Text>();
        //    Text targetHp = GameObject.Find("TargetHPBar").GetComponent<Text>();
        //    Text targetMp = GameObject.Find("TargetMPBar").GetComponent<Text>();
        //    if (target != null)
        //    {
        //        targetHp.text = target.GetComponent<GameCharacter>().curHP.ToString() + "/" + target.GetComponent<GameCharacter>().finalMaxHP;
        //        targetMp.text = target.GetComponent<GameCharacter>().curMP.ToString() + "/" + target.GetComponent<GameCharacter>().finalMaxMP;
        //        targetName.text = target.GetComponent<GameCharacter>().characterName;
        //    }
        //    else
        //    {
        //        targetHp.text = "";
        //        targetMp.text = "";
        //        targetName.text = "";
        //    }
        //}



        //--------------------------------------
        //--------------test code---------------
        //--------------------------------------
        UpdateState();
        UpdateTarget();
        UpdateBuffs();
        DelayDeleteCurSkill();


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
        deleteCurSkill = false;
    }

    void EndInstant()
    {
        curMP = Mathf.Clamp(curMP - curCastSkill.mpCost, 0, finalMaxMP);
        curCastSkill.CDTimer = curCastSkill.CDTime;
        isInstant = false;
        deleteCurSkill = true;
        delayTimer = 0;
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
            photonView.RPC("CancelCast", PhotonTargets.All, true);
        }
    }

    void StartCasting(Skill skill)
    {
        isCasting = true;
        curCastSkill = skill;
        deleteCurSkill = false;
    }

    void EndCasting(bool cd)
    {
        if (cd)
        {
            curCastSkill.CDTimer = curCastSkill.CDTime;
            curMP = Mathf.Clamp(curMP - curCastSkill.mpCost, 0, finalMaxMP);
        }
        isCasting = false;
        deleteCurSkill = true;
        delayTimer = 0;
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
            photonView.RPC("CancelCast", PhotonTargets.All, true);
        }

    }

    void StartChanneling(Skill skill)
    {
        isChanneling = true;
        curCastSkill = skill;
        curCastSkill.CDTimer = curCastSkill.CDTime;
        curMP = Mathf.Clamp(curMP - curCastSkill.mpCost, 0, finalMaxMP);
        deleteCurSkill = false;
    }

    void EndChanneling()
    {
        isChanneling = false;
        deleteCurSkill = true;
        delayTimer = 0;
    }

    void DelayDeleteCurSkill()
    {
        if (deleteCurSkill)
        {
            delayTimer += Time.deltaTime;
            if (delayTimer > 3 && curCastSkill != null)
            {
                curCastSkill = null;
                curSkillIndex = -1;
                delayTimer = 0;
                deleteCurSkill = false;
            }
        }

    }

    #endregion

    #region Move Functions

    public void Move(float moveFwd, float moveRt)
    {
        pc.Move(moveFwd, moveRt);
    }

    public void Move(Vector3 direction, float rotation) {
        pc.Move(direction, rotation);
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
        if (curHP <= 0 && !isDead)
        {
            isDead = true;
            OnDied();
        }
        if (isFreezed)
        {
            SetAnimatorSpeed(0);
        }
        else
        {
            SetAnimatorSpeed(1);
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
        }
        else
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
        Destroy(buff.gameObject);
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
        if (isDead || isFreezed)
        {
            return;
        }
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
            if (curCastSkill != null && !deleteCurSkill)
            {
                if (curCastSkill.skillType == Skill.SkillType.Channeled)
                {
                    photonView.RPC("CancelCast", PhotonTargets.All, true);
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
                        curSkillIndex = i;
                        break;
                    case Skill.SkillType.Cast:
                        StartCasting(skill);
                        curSkillIndex = i;
                        break;
                    case Skill.SkillType.Channeled:
                        StartChanneling(skill);
                        curSkillIndex = i;
                        break;
                }
                pc.StartSkill(i, target.transform);
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
        target.photonView.RPC("TakeSkillMessage", PhotonTargets.All, skill.skillName, attack, photonView.viewID);
    }

    [PunRPC]
    public void CancelCast(bool self)
    {
        if ((isCasting || isChanneling) && !curCastSkill.isMovingCast)
        {
            if (isCasting)
            {
                EndCasting(!self);
            }
            else if (isChanneling)
            {
                EndChanneling();
            }
            if (self)
            {
                print("Cancel");
            }
            else
            {
                print("Interrupt");
            }
            pc.CancelSkill();
        }

    }
    [PunRPC]
    public void TakeSkillMessage(string skillName, int attack, int ownerID)
    {
        Skill.CastedSkillStruct scs;
        scs.skill = GameObject.FindObjectOfType<SkillManager>().FindSkillWithName(skillName);
        scs.attack = attack;
        scs.ownerID = ownerID;
        SendMessage("TakeSkill", scs);
    }

    public void TakeSkill(Skill.CastedSkillStruct scs)
    {
        if (!photonView.isMine)
        {
            return;
        }
        int otherAttack = scs.attack;
        Skill skill = scs.skill;

        otherAttack = Mathf.FloorToInt(UnityEngine.Random.Range(otherAttack * 0.95f, otherAttack * 1.05f));
        int damage = Mathf.FloorToInt((skill.pctDamage * otherAttack + skill.fixedDamage) * (5000 / (5000 + (float)finalDefense)) * damageRatio);
        int heal = Mathf.FloorToInt((skill.pctHealth * finalMaxHP + skill.fixedHealth) * healRatio);
        curHP = Mathf.Clamp(curHP - damage + heal, 0, finalMaxHP);

        foreach (Buff b in skill.buffs)
        {
            AddBuff(b);
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
        if (Vector3.Equals(transform.position, target.transform.position))
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

    public void SetTarget(GameCharacter target)
    {
        this.target = target;
    }

    public GameCharacter GetTarget()
    {
        return target;
    }

    void UpdateTarget()
    {
        if (target != null)
        {
            if (!target.IsAlive)
            {
                SetTarget(null);
            }
        }
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

    public void SetAnimatorSpeed(float speed)
    {
        pc.SetAnimationSpeed(speed);
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

    #region photon functions
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(characterName);
            stream.SendNext(curHP);
            stream.SendNext(curMP);
            stream.SendNext(finalAttack);
            stream.SendNext(finalDefense);
            stream.SendNext(finalMaxHP);
            stream.SendNext(finalMaxMP);
            stream.SendNext(isDead);
            stream.SendNext(freezedCount);
            stream.SendNext(isRestricted);
            stream.SendNext(isInstant);
            stream.SendNext(isCasting);
            stream.SendNext(castTimer);
            stream.SendNext(isChanneling);
            stream.SendNext(channeledInterval);
            stream.SendNext(channeledTimer);
            stream.SendNext(curSkillIndex);
            stream.SendNext(deleteCurSkill);
            stream.SendNext(delayTimer);
            for (int i = 0; i < skills.Count; i++)
            {
                stream.SendNext(skills[i].CDTimer);
            }
            stream.SendNext(globalCDTimer);
            if (target != null)
            {
                stream.SendNext(target.photonView.viewID);
            }
            else
            {
                stream.SendNext(-1);
            }


        }
        else
        {
            characterName = (string)stream.ReceiveNext();
            curHP = (int)stream.ReceiveNext();
            curMP = (int)stream.ReceiveNext();
            finalAttack = (int)stream.ReceiveNext();
            finalDefense = (int)stream.ReceiveNext();
            finalMaxHP = (int)stream.ReceiveNext();
            finalMaxMP = (int)stream.ReceiveNext();
            isDead = (bool)stream.ReceiveNext();
            freezedCount = (int)stream.ReceiveNext();
            isRestricted = (bool)stream.ReceiveNext();
            isInstant = (bool)stream.ReceiveNext();
            isCasting = (bool)stream.ReceiveNext();
            castTimer = (float)stream.ReceiveNext();
            isChanneling = (bool)stream.ReceiveNext();
            channeledInterval = (float)stream.ReceiveNext();
            channeledTimer = (float)stream.ReceiveNext();
            curSkillIndex = (int)stream.ReceiveNext();
            deleteCurSkill = (bool)stream.ReceiveNext();
            delayTimer = (float)stream.ReceiveNext();
            if (curSkillIndex == -1)
            {
                curCastSkill = null;
            }
            else
            {
                curCastSkill = skills[curSkillIndex];
            }
            for (int i = 0; i < skills.Count; i++)
            {
                skills[i].CDTimer = (float)stream.ReceiveNext();
            }
            globalCDTimer = (float)stream.ReceiveNext();
            int targetPhotonID = (int)stream.ReceiveNext();
            if (targetPhotonID == -1)
            {
                target = null;
            }
            else
            {
                PhotonView[] photonViews = GameObject.FindObjectsOfType<PhotonView>();
                foreach (PhotonView p in photonViews)
                {
                    if (p.viewID == targetPhotonID)
                    {
                        target = p.GetComponent<GameCharacter>();
                        break;
                    }
                }
            }
            if (isFreezed)
            {
                SetAnimatorSpeed(0);
            }
            else
            {
                SetAnimatorSpeed(1);
            }
        }
    }
    #endregion
}
