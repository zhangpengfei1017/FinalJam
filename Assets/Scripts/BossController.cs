﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameCharacter))]

public class BossController : MonoBehaviour
{
    [System.Serializable]
    public class BossSkill
    {
        public int skillIndex;

        [Tooltip("Skill cooldown")]
        public float cooldown;

        [Tooltip("Cooldown for skill activate, set 15 if the skill can only be activated after 15 seconds of the fight!")]
        public float count;

        [Tooltip("How long have to wait to do the next action.")]
        public float nextAction;
    }

    [System.Serializable]
    public class StageBoss
    {
        public BossSkill[] Stage1_Skills;
        float Stage1_LifePercent;

        public BossSkill[] Stage2_Skills;
        float Stage2_LifePercent;

        public BossSkill[] Stage3_Skills;
        float Stage3_LifePercent;
    }


    public StageBoss StageBossParams;

    List<BossSkill> skills = new List<BossSkill>();

    private GameCharacter character;

    public enum EnemyState
    {
        //Attack,
        Chase,
        Wait,
        Camp,
        Die
    }

    //global parameters

    public EnemyState enemyState;
    private Vector3 oriPos;

    public GameObject _target;

    public GameObject target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;

            if (value == null)
            {
                character.SetTarget(null);
            }
            else
            {
                character.SetTarget(value.GetComponent<GameCharacter>());
            }
        }
    }

    //chase parameters

    private Vector3 chasePos;
    public float maxChaseDistance;

    public float distanceToTarget;
  
    // wait parameters

    private float waitTimer;

    // destory parameters

    public float destroyTime;
    private float destroyTimer;

    // Use this for initialization
    void Start()
    {
        character = GetComponent<GameCharacter>();

        enemyState = EnemyState.Camp;

        oriPos = transform.position;

        destroyTimer = 0;

        BossSkill b1 = new BossSkill();
        b1.cooldown = 1;
        b1.count = 0;
        b1.skillIndex = 0;
        skills.Add(b1);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PhotonView>().isMine)
        {
            return;
        }

        CheckHealth();

        foreach (BossSkill bs in skills)
        {
            bs.count -= Time.deltaTime;
        }

        switch (enemyState)
        {
            case EnemyState.Camp:
                Camping();
                break;
            case EnemyState.Chase:
                Chasing();
                break;
            case EnemyState.Wait:
                Waiting();
                break;
            case EnemyState.Die:
                Dying();
                break;
        }
    }

    void CheckHealth()
    {
        int curHP = character.CurHP;
        int maxHP = character.MaxHP;

        if (!character.IsAlive)
        {
            enemyState = EnemyState.Die;
        }
    }


    public void TakeSkill(Skill.CastedSkillStruct scs) {

        // TODO: if OT or Tank skill
        if (target == null) {
            PhotonView[] photonViews = GameObject.FindObjectsOfType<PhotonView>();
            foreach (PhotonView p in photonViews)
            {
                if (p.viewID == scs.ownerID)
                {
                    target = p.gameObject;
                    break;
                }
            }
        }
    }

    void CheckTarget()
    {
        if(target != null && !target.GetComponent<GameCharacter>().IsAlive)
        {
            target = null;
        }
    }

    void UpdateTarget()
    {
        if(target == null)
        {
            // Find target in view;
            // TODO: Within angle?

            float view = 5;

            foreach(HeroController hero in FindObjectsOfType<HeroController>())
            {
                if(hero.GetComponent<GameCharacter>().IsAlive)
                {
                    float d = Vector3.Distance(hero.transform.position, transform.position);

                    if (d < view)
                    {
                        view = d;
                        target = hero.gameObject;
                    }
                }
            }
        }   
    }

    void Camping()
    {
        if(target != null)
        {
            enemyState = EnemyState.Chase;
            chasePos = transform.position;
            return;
        }

        if (Vector3.Distance(transform.position, oriPos) > 2)
        {
            Move(oriPos);
        }
        else
        {
            MoveZero();
            character.CurHP += (int)(character.MaxHP * 0.02);
            
            // TODO: Add mp too
        }
    }

    void Move(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        // FIXME: Double quaternion in game character
        Quaternion q = Quaternion.LookRotation(direction);
        character.Move(direction, q.eulerAngles.y);
    }

    void MoveZero()
    {
        character.Move(Vector3.zero, transform.rotation.eulerAngles.y);
    }

    void Chasing()
    {
        CheckTarget();

        if (Vector3.Distance(transform.position, chasePos) > maxChaseDistance)
        {
            target = null;
        }

        if (target == null)
        {
            enemyState = EnemyState.Camp;
            return;
        }

        if (!character.isBusy)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);

            //foreach (BossSkill bs in skills)
            //{
            //    // FIXME: Allow miss attack?
            //    if (bs.count <= 0 && distance <= character.skills[bs.skillIndex].distance)
            //    {
            //        Debug.Log("Attack " + bs.skillIndex);


            //        attack(bs.skillIndex);
            //        bs.count = bs.cooldown;

            //        enemyState = EnemyState.Wait;
            //        waitTimer = bs.nextAction;
            //        return;
            //    }
            //}

            if (distance > distanceToTarget)
            {
                Move(target.transform.position);
            }
            else
            {
                MoveZero();
            }
        }
    }

    void attack(int index)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion q = Quaternion.LookRotation(direction);
        character.Move(Vector3.zero, q.eulerAngles.y);

        character.Attack(index);
    }

    void Waiting()
    {
        waitTimer -= Time.deltaTime;

        if(waitTimer <= 0)
        {
            CheckTarget();
            UpdateTarget();

            enemyState = EnemyState.Chase;

            //chasePos = transform.position;
        }
    }

    void Dying() {
        destroyTimer += Time.deltaTime;
        if (destroyTimer >= destroyTime) {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
