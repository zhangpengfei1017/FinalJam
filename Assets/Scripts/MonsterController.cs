using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameCharacter))]
[RequireComponent(typeof(CharacterController))]
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

    private GameObject targetToAttack; //Gameobject to attack. Retrieved from GameCharacter

    // Radius to check for player
    public float attackRadius;

    //Wander Parameters
    public float wanderJitter; // 'Wandering' amount
    public float wanderTime;
    public float moveRate;
    private float nextMove;

    //Stop Parameters
    public float stopTime;

    private GameCharacter character;
    private CharacterController characterController;

    private enum EnemyMovement
    {
        STOPMOVEMENT,
        WANDER,
        SEEK,
        FLEE
    }

    private EnemyMovement enemyMovement;

    private Vector3 dir;

    private static float wanderCounter; // Counters to switch between movement
    private static float stopCounter;

    // Use this for initialization
    void Start()
    {
        character = GetComponent<GameCharacter>();
        characterController = GetComponent<CharacterController>();

        enemyMovement = EnemyMovement.WANDER;
    }

    // Update is called once per frame
    void Update()
    {
        switch (enemyMovement)
        {
            case EnemyMovement.STOPMOVEMENT:
                StopMovement();
                break;

            case EnemyMovement.WANDER:
                Wander();
                break;

            case EnemyMovement.SEEK:
                Seek();
                break;

            case EnemyMovement.FLEE:
                Flee();
                break;

            default:
                break;
        }
    }

    #region MoveFunctions
    void StopMovement()
    {
        //Stop enemy and walk animation
        if (stopCounter < stopTime)
        {
            if (targetToAttack != null)
            {
                targetToAttack = null;
            }
             
            stopCounter += Time.deltaTime;

        }
        else
        {
            stopCounter = 0;
            enemyMovement = EnemyMovement.WANDER;
        }
    }

    void Wander()
    {
        if (wanderCounter < wanderTime)
        {
            wanderCounter += Time.deltaTime;

            Vector3 wanderTarget = Vector3.zero;

            if (nextMove < Time.time)
            {
                nextMove = Time.time + (1 / moveRate);
                float randomValue = Random.Range(-1.0f, 1.0f);
                
                transform.Rotate(Vector3.up, randomValue * wanderJitter);
            }

            characterController.SimpleMove(transform.forward * character.MoveSpeed * Time.deltaTime);

            if(CheckForEnemy())
            {
                enemyMovement = EnemyMovement.SEEK;
            }

        }
        else
        {
            wanderCounter = 0;
            enemyMovement = EnemyMovement.STOPMOVEMENT;
        }
    }

    bool CheckForEnemy()
    {
        targetToAttack = character.GetTarget();

        if(targetToAttack != null)
        {
            if (Vector3.Distance(transform.position, targetToAttack.transform.position) < attackRadius)
            {
                return true;
            }
        }

        return false;
    }


    void Seek()
    {
            Vector3 seekDirection = (targetToAttack.transform.position - transform.position).normalized;
            seekDirection = new Vector3(seekDirection.x, 0.0f, seekDirection.z);
            transform.forward = seekDirection;

            characterController.SimpleMove(seekDirection * character.MoveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetToAttack.transform.position) > (attackRadius * 2))
            {
                enemyMovement = EnemyMovement.WANDER;
            }
    }

    void Flee()
    {
        Vector3 fleeDirection = (transform.position - targetToAttack.transform.position).normalized;
        fleeDirection = new Vector3(fleeDirection.x, 0.0f, fleeDirection.z);

        transform.forward = (fleeDirection);

        characterController.SimpleMove(fleeDirection * character.MoveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetToAttack.transform.position) > (attackRadius * 2.0f))
        {
            enemyMovement = EnemyMovement.STOPMOVEMENT;
            targetToAttack = null;
        }
    }
    #endregion
}
