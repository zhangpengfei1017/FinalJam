using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HereController : MonoBehaviour
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
    private GameObject indicator;

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

    public void ChooseTarget(GameObject target)
    {
        if (target != null)
        {
            character.SetTarget(target);
            if (curIndicator == null)
            {
                curIndicator = Instantiate(indicator, target.transform.position, transform.transform.rotation, target.transform) as GameObject;
            }
            else
            {
                curIndicator.transform.parent = target.transform;
                curIndicator.transform.position = target.transform.position;
                curIndicator.transform.rotation = target.transform.rotation;
            }
        }
        else
        {
            Destroy(curIndicator);
            character.SetTarget(null);
        }
        character.CancelCast(true);
    }





}
