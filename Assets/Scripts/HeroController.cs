using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameCharacter))]

public class HeroController : MonoBehaviour
{
    public enum Class
    {
        Knight,
        Sorceress,
        Priest,
        Archer,
    };

    [SerializeField]
    private float threat = 1;

    public GameCharacter character;

    [SerializeField]
    private string playerName;

    [SerializeField]
    private Class className = Class.Knight;

    [SerializeField]
    private GameObject indicator_enemy;

    [SerializeField]
    private GameObject indicator_player;

    private GameObject curIndicator;



    //

    private GameController gameController;

    //

    //private NewPlayerController pc;

    void Start()
    {
        character = GetComponent<GameCharacter>();

        gameController = GameController.instance;
        //pc = GetComponent<NewPlayerController>();
    }

    void Update()
    {
        if (!GetComponent<PhotonView>().isMine)
        {
            return;
        }
        DetectAttack();

        DetectMove();

        DetectClick();
    }


    void DetectMove()
    {
        float moveFwd = Input.GetAxis("Vertical");
        float moveRt = Input.GetAxis("Horizontal");
        character.Move(moveFwd, moveRt);
    }

    void DetectAttack()
    {
        int attackIndex = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            attackIndex = 0;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            attackIndex = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            attackIndex = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            attackIndex = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            attackIndex = 4;
        }
        if (attackIndex != -1)
        {
            character.Attack(attackIndex);

        }
    }

    void DetectClick() {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                GameObject hitgo = hit.collider.gameObject;
                if (hitgo.GetComponent<GameCharacter>() != null)
                {
                    ChooseTarget(hitgo);
                }
                else
                {
                    ChooseTarget(null);
                }
            }
            else {
                ChooseTarget(null);
            }
        }
    }

    public void ChooseTarget(GameObject target)
    {
        if (target == character.GetTarget())
        {
            return;
        }
        if (curIndicator != null) {
            Destroy(curIndicator);
        }
        if (target != null && target.GetComponent<GameCharacter>().IsAlive)
        {
            character.SetTarget(target.GetComponent<GameCharacter>());
            if (target.GetComponent<GameCharacter>().characterType == GameCharacter.CharacterType.Monster)
            {
                curIndicator = Instantiate(indicator_enemy, target.transform.position + new Vector3(0, 5, 0), indicator_enemy.transform.rotation, target.transform) as GameObject;
                curIndicator.GetComponent<Projector>().orthographicSize = target.GetComponent<CharacterController>().radius * 2;
            }
            else
            {
                curIndicator = Instantiate(indicator_player, target.transform.position + new Vector3(0, 5, 0), indicator_player.transform.rotation, target.transform) as GameObject;
                curIndicator.GetComponent<Projector>().orthographicSize = target.GetComponent<CharacterController>().radius * 2;
            }
        }
        else {
            character.SetTarget(null);
        }
        character.CancelCast(true);
    }





}
