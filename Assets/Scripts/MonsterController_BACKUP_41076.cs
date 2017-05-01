using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameCharacter))]
<<<<<<< HEAD

public class MonsterController : MonoBehaviour {

=======
[RequireComponent(typeof(CharacterController))]
public class MonsterController : MonoBehaviour
{
>>>>>>> 930f230b85d788a40c6d709e012dce4cfc9ef140
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
    public float wanderTime; // Time to Wander
    public float moveRate; // Rate to change direction
    private float nextMove; 

    //Stop Parameters
    public float stopTime; // Time to stop movement

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
        if (character.CurHP < 20)
            enemyMovement = EnemyMovement.FLEE;

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
    /// <summary>
    /// Stops movement
    /// </summary>
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

    /// <summary>
    /// Free roam. Changes direction at fixed intervals
    /// </summary>
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

            if(CheckForTarget())
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

    /// <summary>
    /// Checks if target is within attack radius
    /// </summary>
    /// <returns></returns>
    bool CheckForTarget()
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

    /// <summary>
    /// Moves towards target to attack
    /// </summary>
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

    /// <summary>
    /// Moves away from target
    /// </summary>
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
