using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameCharacter))]

public class MonsterController : MonoBehaviour
{
    public enum MonsterType
    {
        Trash,
        Boss
    };

    [SerializeField]
    private string monsterName;

    [SerializeField]
    private MonsterType monsterType = MonsterType.Trash;

    private GameCharacter character;

    private enum EnemyMovement
    {
        STOPMOVEMENT,
        WANDER,
        SEEK,
        FLEE
    }

    public enum EnemyState
    {
        Wander,
        Stop,
        Chase,
        Attack,
        Flee,
        Die
    }

    private EnemyState enemyState;


    //global parameters

    public GameObject target;

    private bool hasAttackTarget;

    //wander parameters
    private Vector3 oriPos;

    private bool gotoWander;

    public float maxWanderTime;

    private float wanderTimer;

    private bool hasTargetPosition;

    private Vector3 targetPosition;

    //stop parameters

    private bool gotoStop;

    public float maxStopTime;

    private float stopTimer;

    //chase parameters

    private bool gotoChase;

    public float maxChaseDistance;

    //attack parameters

    private bool gotoAttack;

    public float attackCD;

    private float attackTimer;

    //flee parameters

    private bool gotoFlee;

    //death parameters

    private bool gotoDie;



    // Use this for initialization
    void Start()
    {
        character = GetComponent<GameCharacter>();

        enemyState = EnemyState.Wander;
        //
        oriPos = transform.position;
        wanderTimer = maxWanderTime;
        stopTimer = maxStopTime;
        attackTimer = 0;
        hasAttackTarget = false;

        //
        gotoAttack = false;
        gotoChase = false;
        gotoDie = false;
        gotoFlee = false;
        gotoStop = false;
        gotoWander = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckHealth();
        CheckTarget();
        UpdateState();
        switch (enemyState)
        {
            case EnemyState.Wander:
                Wandering();
                break;
            case EnemyState.Stop:
                Stopping();
                break;
            case EnemyState.Chase:
                Chasing();
                break;
            case EnemyState.Attack:
                Attacking();
                break;
            case EnemyState.Flee:
                Fleeing();
                break;
            case EnemyState.Die:
                break;
        }
    }
    void UpdateState()
    {
        bool exit = false;
        switch (enemyState)
        {
            case EnemyState.Wander:
                if (gotoChase)
                {
                    enemyState = EnemyState.Chase;
                    exit = true;
                }

                if (gotoStop)
                {
                    enemyState = EnemyState.Stop;
                    exit = true;
                }

                if (gotoFlee)
                {
                    enemyState = EnemyState.Flee;
                    exit = true;
                }

                if (gotoDie)
                {
                    enemyState = EnemyState.Die;
                    exit = true;
                }

                if (exit)
                {
                    hasTargetPosition = false;
                    wanderTimer = maxWanderTime;
                    exit = false;
                }
                break;
            case EnemyState.Stop:
                if (gotoChase)
                {
                    enemyState = EnemyState.Chase;
                    exit = true;
                }
                if (gotoWander)
                {
                    enemyState = EnemyState.Wander;
                    exit = true;
                }
                if (gotoFlee)
                {
                    enemyState = EnemyState.Flee;
                    exit = true;
                }
                if (gotoDie)
                {
                    enemyState = EnemyState.Die;
                    exit = true;
                }
                if (exit)
                {
                    stopTimer = maxStopTime;
                    exit = false;
                }
                break;
            case EnemyState.Chase:
                if (gotoAttack)
                {
                    enemyState = EnemyState.Attack;
                    exit = true;
                }
                if (gotoWander)
                {
                    enemyState = EnemyState.Wander;
                    exit = true;
                }

                if (gotoFlee)
                {
                    enemyState = EnemyState.Flee;
                    exit = true;
                }

                if (gotoDie)
                {
                    enemyState = EnemyState.Die;
                    exit = true;
                }
                if (exit)
                {
                    exit = false;
                }
                break;
            case EnemyState.Attack:
                if (gotoChase)
                {
                    enemyState = EnemyState.Chase;
                    exit = true;
                }

                if (gotoWander)
                {
                    enemyState = EnemyState.Wander;
                    exit = true;
                }

                if (gotoFlee)
                {
                    enemyState = EnemyState.Flee;
                    exit = true;
                }

                if (gotoDie)
                {
                    enemyState = EnemyState.Die;
                    exit = true;
                }
                if (exit)
                {
                    exit = false;
                }
                break;
            case EnemyState.Flee:
                if (gotoWander)
                {
                    enemyState = EnemyState.Wander;
                    exit = true;
                }

                if (gotoDie)
                {
                    enemyState = EnemyState.Die;
                    exit = true;
                }

                if (exit)
                {
                    exit = false;
                }
                break;
        }
        //all to flee
        gotoWander = false;
        gotoStop = false;
        gotoChase = false;
        gotoAttack = false;
        gotoFlee = false;
        gotoDie = false;
    }

    void CheckHealth()
    {
        int curHP = character.CurHP;
        int maxHP = character.MaxHP;
        if (!character.IsAlive)
        {
            gotoDie = true;
        }
        else
        {
            if (((float)curHP / (float)maxHP) < 0.1)
            {
                gotoFlee = true;
            }
        }
    }

    public void TakeSkill(Skill.CastedSkillStruct sck) {
        if (!hasAttackTarget && (enemyState == EnemyState.Wander || enemyState == EnemyState.Stop)) {
            target = sck.owner;
            character.SetTarget(target);
            gotoChase = true;
            hasAttackTarget = true;
        }
    }

    void CheckTarget() {
        if (target != null)
        {
            if (!target.GetComponent<GameCharacter>().IsAlive)
            {
                hasAttackTarget = false;
                target = null;
                gotoWander = true;
            }
        }          
    }

    void Wandering()
    {
        wanderTimer -= Time.deltaTime;
        if (!hasTargetPosition)
        {
            targetPosition = new Vector3(oriPos.x + Random.Range(-8, 8), oriPos.y, oriPos.z + Random.Range(-8, 8));
            hasTargetPosition = true;
        }
        else
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            character.Move(direction,0, -1, 0.5f);
            if (direction != Vector3.zero) {
                transform.forward = direction;
            }            
            if (Vector3.Distance(transform.position, targetPosition) <= 1 || wanderTimer <= 0)
            {
                gotoStop = true;
            }
        }
    }

    void Stopping()
    {
        stopTimer -= Time.deltaTime;
        character.Move(Vector3.zero, transform.rotation.eulerAngles.y, 0, 0);
        if (stopTimer <= 0)
        {
            gotoWander = true;
        }
    }

    void Chasing()
    {
        Vector3 targetPlayer = character.GetTarget().transform.position;
        Vector3 direction = (targetPlayer - transform.position).normalized;
        character.Move(direction, 0, 1, 1);
        transform.forward = direction;
        float distance = Vector3.Distance(transform.position, targetPlayer);
        if (distance <= 4)
        {
            gotoAttack = true;
        }
        if (distance > maxChaseDistance)
        {
            gotoWander = true;
        }
    }

    void Attacking()
    {
        if (target == null) {
            return;
        }
        attackTimer += Time.deltaTime;
        Vector3 targetPlayer = target.transform.position;
        Vector3 direction = (targetPlayer - transform.position).normalized;
        character.Move(Vector3.zero, 0, 0, 0);
        transform.forward = direction;
        float distance = Vector3.Distance(transform.position, targetPlayer);
        if (attackTimer >= attackCD)
        {
            character.Attack(0);
            attackTimer = 0;
        }
        if (distance > 4) {
            gotoChase = true;
        }
    }

    void Fleeing()
    {

    }


    //#region MoveFunctions
    ///// <summary>
    ///// Stops movement
    ///// </summary>
    //void StopMovement()
    //{
    //    //Stop enemy and walk animation
    //    if (stopCounter < stopTime)
    //    {
    //        if (targetToAttack != null)
    //        {
    //            targetToAttack = null;
    //        }

    //        stopCounter += Time.deltaTime;

    //    }
    //    else
    //    {
    //        stopCounter = 0;
    //        enemyMovement = EnemyMovement.WANDER;
    //    }
    //}

    ///// <summary>
    ///// Free roam. Changes direction at fixed intervals
    ///// </summary>
    //void Wander()
    //{
    //    if (wanderCounter < wanderTime)
    //    {
    //        wanderCounter += Time.deltaTime;

    //        Vector3 wanderTarget = Vector3.zero;

    //        if (nextMove < Time.time)
    //        {
    //            nextMove = Time.time + (1 / moveRate);
    //            float randomValue = Random.Range(-1.0f, 1.0f);

    //            transform.Rotate(Vector3.up, randomValue * wanderJitter);
    //        }
    //        character.Move(transform.forward, 0, 1, 1);

    //        if (CheckForTarget())
    //        {
    //            enemyMovement = EnemyMovement.SEEK;
    //        }

    //    }
    //    else
    //    {
    //        wanderCounter = 0;
    //        enemyMovement = EnemyMovement.STOPMOVEMENT;
    //    }
    //}

    ///// <summary>
    ///// Checks if target is within attack radius
    ///// </summary>
    ///// <returns></returns>
    //bool CheckForTarget()
    //{
    //    targetToAttack = character.GetTarget();

    //    if (targetToAttack != null)
    //    {
    //        if (Vector3.Distance(transform.position, targetToAttack.transform.position) < attackRadius)
    //        {
    //            return true;
    //        }
    //    }

    //    return false;
    //}

    ///// <summary>
    ///// Moves towards target to attack
    ///// </summary>
    //void Seek()
    //{
    //    Vector3 seekDirection = (targetToAttack.transform.position - transform.position).normalized;
    //    seekDirection = new Vector3(seekDirection.x, 0.0f, seekDirection.z);
    //    transform.forward = seekDirection;
    //    character.Move(seekDirection, 0, 1, 1);
    //    //characterController.SimpleMove(seekDirection * character.MoveSpeed * Time.deltaTime);

    //    if (Vector3.Distance(transform.position, targetToAttack.transform.position) > (attackRadius * 2))
    //    {
    //        enemyMovement = EnemyMovement.WANDER;
    //    }
    //}

    ///// <summary>
    ///// Moves away from target
    ///// </summary>
    //void Flee()
    //{
    //    Vector3 fleeDirection = (transform.position - targetToAttack.transform.position).normalized;
    //    fleeDirection = new Vector3(fleeDirection.x, 0.0f, fleeDirection.z);

    //    transform.forward = (fleeDirection);

    //    character.Move(fleeDirection, 0, 1, 1);

    //    //characterController.SimpleMove(fleeDirection * character.MoveSpeed * Time.deltaTime);

    //    if (Vector3.Distance(transform.position, targetToAttack.transform.position) > (attackRadius * 2.0f))
    //    {
    //        enemyMovement = EnemyMovement.STOPMOVEMENT;
    //        targetToAttack = null;
    //    }
    //}
    //#endregion
}
