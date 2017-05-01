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

    private GameCharacter character;

    [SerializeField]
    private string playerName;

    [SerializeField]
    private Class className = Class.Knight;

    [SerializeField]
    private GameObject indicator_enemy;

    [SerializeField]
    private GameObject indicator_player;

    private GameObject curIndicator;

    private UIController uiCtrl;

    private GameObject cam;

    void Start()
    {
        character = GetComponent<GameCharacter>();

        cam = GameObject.Find("MainCam");
    }

    void Update()
    {

        DetectAttack();

        DetectMove();

        DetectClick();
    }


    void DetectMove()
    {
        Vector3 dir = Vector3.zero;
        float ro = 0;
        int d = 0;
        float speed = 1; ;
        if (Input.GetKey(KeyCode.W))
        {
            dir += transform.forward;
            d = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir -= transform.forward;
            d = -1;
            speed = 0.4f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.W))
            {
                ro = -45;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                ro = 45;
            }
            else
            {
                dir += transform.forward;
                ro = -90;
                d = 1;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.W))
            {
                ro = 45;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                ro = -45;
            }
            else
            {
                dir += transform.forward;
                ro = 90;
                d = 1;
            }
        }
        character.Move(dir, cam.transform.rotation.eulerAngles.y+ro, d, speed);
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
            Ray ray = cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
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
        if (target != null)
        {            
            character.SetTarget(target);
            if (target.GetComponent<GameCharacter>().characterType == GameCharacter.CharacterType.Monster)
            {
                curIndicator = Instantiate(indicator_enemy, target.transform) as GameObject;
                curIndicator.GetComponent<Projector>().orthographicSize = target.GetComponent<CharacterController>().radius * 2;
            }
            else
            {
                curIndicator = Instantiate(indicator_player, target.transform) as GameObject;
                curIndicator.GetComponent<Projector>().orthographicSize = target.GetComponent<CharacterController>().radius * 2;
            }
        }
        else {
            character.SetTarget(null);
        }
        character.CancelCast(true);
    }





}
