using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameCharacter))]

public class BossController : MonoBehaviour
{
    public delegate void SkillDel();

    //[System.Serializable]
    public class BossSkill
    {
        public float cooldown;
        public float count;

        // How long to do the next action
        public float nextAction;

        public int index;
    }

    List<BossSkill> skills = new List<BossSkill>();

    public enum MonsterType
    {
        Trash,
        Boss
    };

    [SerializeField]
    private string monsterName;

    [SerializeField]
    private MonsterType monsterType = MonsterType.Boss;

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

    private EnemyState enemyState;
    private Vector3 oriPos;

    private GameObject _target;

    public GameObject target
    {
        get
        {
            return _target;
        }
        set
        {
            _target = value;
            character.SetTarget(value.GetComponent<GameCharacter>());
        }
    }

    //chase parameters

    private Vector3 chasePos;
    public float maxChaseDistance;

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
        b1.index = 0;
        skills.Add(b1);
    }

    // Update is called once per frame
    void Update()
    {
        //if (!GetComponent<PhotonView>().isMine) {
        //    return;
        //}
        CheckHealth();

        foreach (BossSkill bs in skills)
        {
            bs.count += Time.deltaTime;
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
            //character.SetTarget(target.GetComponent<GameCharacter>());
            //gotoChase = true;
            //hasAttackTarget = true;
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
            // TODO: Find new target in view
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

        if (Vector3.Distance(transform.position, oriPos) < 2)
        {
            Move(oriPos);
        } else
        {
            character.CurHP += (int)(character.MaxHP * 0.02);
            // TODO: Add mp too
        }
    }

    void Move(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;

        // FIXME: Double quaternion in game character
        Quaternion q = Quaternion.LookRotation(direction);
        character.Move(direction, q.eulerAngles.y, 1, 1);
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

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (!character.isBusy)
        {
            foreach (BossSkill bs in skills)
            {
                // FIXME: Allow miss attack?
                if (bs.count <= 0 && distance <= character.skills[bs.index].distance)
                {
                    attack(bs.index);
                    bs.count = bs.cooldown;

                    enemyState = EnemyState.Wait;
                    waitTimer = bs.nextAction;
                    return;
                }
            }
        }

        Move(target.transform.position);
    }

    void attack(int index)
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion q = Quaternion.LookRotation(direction);
        character.Move(Vector3.zero, q.eulerAngles.y, 0, 0);

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
            chasePos = transform.position;
        }
    }

    void Dying() {
        destroyTimer += Time.deltaTime;
        if (destroyTimer >= destroyTime) {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
