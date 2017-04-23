using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public GameObject cam;
    public GameObject skillTrigger;
    //
    private Animator ani;
    private CharacterController cc;
    //
    private bool isActive;
    private bool isRunable;
    private bool isAttackable;
    private bool isRollable;
    //
    public GameObject target;
    //
    private float attackTimer;
    private bool isAttackTiming;
    private int attackState;
    private ArrayList allTrigger;
    //
    private float curhp;
    private float maxhp;
    private float curmp;
    private float maxmp;
    //
    public GameObject hpBar;
    public GameObject mpBar;
    public GameObject restart;
    // Use this for initialization
    void Start()
    {

        if ( null == hpBar)
        {
            hpBar = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<VariableDataForNetworking>().hpBar;
        }
        if (null == mpBar)
        {
            mpBar = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<VariableDataForNetworking>().mpbar;
        }
        if (null == restart)
        {
            restart = GameObject.FindGameObjectWithTag("UIRoot").GetComponent<VariableDataForNetworking>().Restart;
        }
        if ( null == cam)
        {
            cam = Camera.main.gameObject;
        }


        //
        isActive = true;
        isRunable = true;
        isAttackable = true;
        isRollable = true;
        //
        isAttackTiming = false;
        attackTimer = 0;
        attackState = 0;
        allTrigger = new ArrayList();
        //
        ani = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
        //
        maxhp = 100;
        maxmp = 100;
        curhp = maxhp;
        curmp = maxmp;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (restart.activeInHierarchy)
            {
                restart.SetActive(false);
                Time.timeScale = 1;
            }
            else if(Time.timeScale!=0){
                Time.timeScale = 0;
                restart.SetActive(true);
            }
        }

        ResetAnimation();
        ResetState();
        hpBar.GetComponent<UIProgressBar>().value = (float)curhp / (float)maxhp;
        mpBar.GetComponent<UIProgressBar>().value = (float)curmp / (float)maxmp;
        if (isActive)
        {//is alive
            curmp = Mathf.Clamp(curmp + 10 * Time.deltaTime, 0, 100);
            curhp = Mathf.Clamp(curhp + 2 * Time.deltaTime, 0, 100);
            if (isRollable)
            {//can use dodge
                Dodge();
                if (isAttackable)
                {//can attack
                    Attack();
                    if (isRunable)
                    {//free to walk
                        RunAndRotate();
                    }
                }
            }
        }

    }
    public bool isAlive()
    {
        if (curhp != 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RemoveTrigger(GameObject g)
    {
        allTrigger.Remove(g);
    }
    void Attack()
    {
        if (isAttackTiming)
        {
            if (attackState == 1 && ani.GetCurrentAnimatorStateInfo(0).IsName("attack2"))
            {
                attackState = 2;
                attackTimer = 0;
                CostMp(20);
            }
            if (attackState == 2 && ani.GetCurrentAnimatorStateInfo(0).IsName("attack3"))
            {
                attackState = 3;
                attackTimer = 0;
                CostMp(20);
            }
            attackTimer += Time.deltaTime;
        }
        if (Input.GetMouseButtonDown(0))
        {
            //attack
            ani.SetBool("isAttack", true);
            if (ani.GetInteger("attackNum") == 0 && curmp >= 20)
            {
                ani.SetInteger("attackNum", 1);
                attackTimer = 0;
                isAttackTiming = true;
                attackState = 1;
                CostMp(20);
            }
            else if (attackState == 1 && attackTimer >= (0.7 * 1.263f) && curmp >= 20)
            {
                ani.SetInteger("attackNum", 2);

            }
            else if (attackState == 2 && attackTimer >= (0.7 * 0.71f) && curmp >= 20)
            {
                ani.SetInteger("attackNum", 3);

            }

        }

    }
    void CostMp(float i)
    {
        curmp -= i;
    }
    void Dodge()
    {

        float ro = 0;
        if (Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.Space))
        {
            ro = -90;
        }
        else if (Input.GetKey(KeyCode.D) && Input.GetKeyDown(KeyCode.Space))
        {
            ro = 90;
        }
        if (ro != 0 && curmp >= 30)
        {
            ani.SetBool("isRoll", true);
            ani.SetBool("isRun", false);
            isRollable = false;
            RotateToCamDir(ro);
            isRunable = false;
            curmp -= 30;
            ani.SetBool("isGethit", false);
        }


    }

    void RotateToCamDir(float ro)
    {
        Quaternion q = Quaternion.Euler(transform.rotation.eulerAngles.x, cam.transform.rotation.eulerAngles.y + ro, transform.rotation.eulerAngles.z);
        transform.rotation = q;
    }

    void Skill(int i)
    {
        int damage = 0;
        if (i == 1)
        {
            damage = 15;
        }
        else if (i == 2)
        {
            damage = 10;
        }
        else if (i == 3)
        {
            damage = 20;
        }
        GameObject go = Instantiate(skillTrigger, transform.position + transform.up * 2, Quaternion.identity) as GameObject;
        go.GetComponent<SkillTriggerAction>().damage = damage;
        go.GetComponent<SkillTriggerAction>().SetOwner(gameObject);
        go.transform.forward = transform.forward;
        allTrigger.Add(go);
    }


    void CancelSkill()
    {
        foreach (GameObject i in allTrigger)
        {
            Destroy(i);
        }
        allTrigger.Clear();
    }
    public void GetHit(int damage)
    {
        if (isRollable)
        {
            CancelSkill();
            curhp = Mathf.Clamp(curhp - damage, 0, 100);
            ani.SetBool("isGethit", true);
            if (ani.GetCurrentAnimatorStateInfo(0).IsName("gethit"))
            {
                ani.Play("gethit", 0, 0);
            }
            if (curhp == 0)
            {
                isActive = false;
                ani.SetBool("isDie", true);
            }
        }
    }

    void ResetState()
    {
        AnimatorStateInfo asi = ani.GetCurrentAnimatorStateInfo(0);
        isActive = !asi.IsTag("die") && (!ani.GetBool("isDie"));
        isRollable = isActive && (!asi.IsTag("roll")) && (!ani.GetBool("isRoll"));
        isAttackable = isRollable && (!asi.IsTag("gethit")) && (!ani.GetBool("isGethit"));
        isRunable = isAttackable && (!asi.IsTag("attack")) && (!ani.GetBool("isRoll"));
    }

    void RunAndRotate()
    {
        Vector3 dir = Vector3.zero;
        float ro = 0;
        if (Input.GetKey(KeyCode.W))
        {
            dir += transform.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            dir -= transform.forward;
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
            }
        }
        dir = dir.normalized;
        cc.SimpleMove(dir * speed * Time.deltaTime);
        RotateToCamDir(ro);
        if (dir != Vector3.zero)
        {
            ani.SetBool("isRun", true);
        }
        else
        {
            ani.SetBool("isRun", false);
        }

    }

    void ResetAnimation()
    {
        AnimatorStateInfo asi = ani.GetCurrentAnimatorStateInfo(0);
        if (asi.IsTag("gethit"))
        {
            ani.SetBool("isGethit", false);
        }
        if (asi.IsTag("roll"))
        {
            ani.SetBool("isRoll", false);
            ani.SetBool("isGethit", false);
        }
        if (!asi.IsTag("attack"))
        {
            ani.SetInteger("attackNum", 0);
            ani.SetBool("isAttack", false);
            isAttackTiming = false;
            attackState = 0;
        }
    }
}
