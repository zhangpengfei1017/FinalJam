using UnityEngine;
using System.Collections;
using System;

public class EnemyController : Photon.MonoBehaviour, IPunObservable
{
    public float speed;
    public int maxhp;
    public int curhp;

    private GameObject player;
    public GameObject hpBarPrefab;
    public GameObject HUDpoint;
    public GameObject skillPrefab;

    private GameObject UIRoot;

    private GameObject myHpBar;
    private float timer;
    private bool inAttack;
    private CharacterController cc;
    private Animator ani;
    private Vector3 des;
    private Vector3 dir;
    private bool isWalking;
    private Vector3 oriPosition;

    private bool isGettingHit;

    private int attackCount;
    private float attackTimer;

    private ArrayList skillTrigger;

    //
    public PhotonView pv;
    public int targetPhotonID;
    // Use this for initialization
    void Start () {
        isWalking = false;
        isGettingHit = false;
        cc = GetComponent<CharacterController>();
        ani = GetComponent<Animator>();
        oriPosition = transform.position;
        attackTimer = 0;
        attackCount = 0;
        inAttack = false;
        UIRoot = GameObject.FindGameObjectWithTag("UIRoot");
        myHpBar = NGUITools.AddChild(UIRoot, hpBarPrefab);
        myHpBar.GetComponent<UIFollowTarget>().target = HUDpoint.transform;
        myHpBar.SetActive(false);
        skillTrigger = new ArrayList();
        pv=GetComponent<PhotonView>();
    }
	
	// Update is called once per frame
	void Update () {
        myHpBar.GetComponent<UIProgressBar>().value = (float)curhp / (float)maxhp;
        if (!pv.isMine) {
            return;
        }
        if (attackTimer<3) {
            attackTimer += Time.deltaTime;
        }
        if (ani.GetCurrentAnimatorStateInfo(0).IsName("gethit")&&ani.GetBool("isGethit"))
        {
            ani.SetBool("isGethit", false);
        }
        else {
            isGettingHit = false;
        }
        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("die"))
        {
            if (!inAttack)
            {
                WalkAround();
            }
            else
            {
                //in attack
                //first look at player, then go to his position, if the position is ok, attack over time, if not, chase the player, if can't catch player in a certain time, come back
                if (!isGettingHit) {
                    Attack();
                }                
            }
        }
        if (ani.GetCurrentAnimatorStateInfo(0).IsName("die")) {
            ani.SetBool("isDie", false);
        }
	}
    [PunRPC]
    public void GetHit(int damage,int id) {
        if (!pv.isMine) {
            return;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        targetPhotonID = id;
        PhotonView[] allviews = GameObject.FindObjectsOfType<PhotonView>();
        foreach (PhotonView p in allviews)
        {
            if (p.photonView.ownerId == id)
            {
                player = p.gameObject;
            }
        }
        CancelSkill();
        ani.SetBool("isGethit", true);
        if (ani.GetCurrentAnimatorStateInfo(0).IsName("gethit"))
        {
            ani.Play("gethit", 0, 0);
        }
        curhp = Mathf.Clamp(curhp - damage, 0, maxhp);
        inAttack = true;
        ani.SetBool("isInAttack", true);
        myHpBar.SetActive(true);
        if (curhp == 0) {
            Die();
        }
    }
    void Die() {
        ani.SetBool("isDie", true);
        GetComponent<CharacterController>().enabled = false;
    }

    public void Attack() {
        if (player == null) {
            return;
        }
        Vector3 direction = (player.transform.position - transform.position);
        if (ani.GetCurrentAnimatorStateInfo(0).IsTag("look"))
        {
            transform.forward = direction;
        }
        float distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > 4.5 && distance <= 15)
        {
            //chase
                ani.SetBool("isMove", true);
            cc.SimpleMove(direction.normalized * speed * 1f * Time.deltaTime);
        }
        else if (distance <= 4.5)
        {
            //attack
            ani.SetBool("isMove", false);
            if (ani.GetCurrentAnimatorStateInfo(0).IsTag("attack")) {
                ani.SetBool("isAttack1", false);
                ani.SetBool("isAttack2", false);
            }
            
            if (attackTimer >= 3) {
                int mod = attackCount % 3;
                if (mod < 2)
                {
                    //attack1
                    attackCount++;
                    ani.SetBool("isAttack1", true);       
                }
                else {
                    //attack2
                    attackCount++;
                    ani.SetBool("isAttack2", true);
                }
                if (!player.GetComponent<PlayerController>().isAlive()) {
                    attackTimer = 3;
                    oriPosition = transform.position;
                    inAttack = false;
                    ani.SetBool("isInAttack", false);
                    myHpBar.SetActive(false);
                    curhp = maxhp;
                    ani.SetBool("isAttack1", false);
                    ani.SetBool("isAttack2", false);
                    ani.Play("idle");
                }
                attackTimer = 0;
            }
        }
        else if (distance > 15) {
            //quit
            ani.SetBool("isMove", false);
            attackTimer = 3f;
            oriPosition = transform.position;
            inAttack = false;
            ani.SetBool("isInAttack", false);
            myHpBar.SetActive(false);
            ani.SetBool("isAttack1", false);
            ani.SetBool("isAttack2", false);
            ani.Play("idle");
            curhp = maxhp;
        }
    }
    public void RemoveTrigger(GameObject g) {
        skillTrigger.Remove(g);
    }
    public void Skill(int i) {
        if (i == 1) {
            SpellSkill(30);
        }
        if (i == 2)
        {
            SpellSkill(40);
        }

    }

    public void WalkAround() {
        timer += Time.deltaTime;
        if ((timer > 5 && !isWalking) || (timer > 15))
        {
            des = new Vector3(oriPosition.x + UnityEngine.Random.Range(-8, 8), oriPosition.y, oriPosition.z + UnityEngine.Random.Range(-8, 8));
            dir = (des - transform.position).normalized;
            isWalking = true;
            ani.SetBool("isMove", true);
            transform.forward = dir;
            if (timer > 15)
            {
                timer = 5.1f;
            }
        }
        if (isWalking)
        {
            cc.SimpleMove(dir * speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, des) <= 2)
            {
                ani.SetBool("isMove", false);
                isWalking = false;
                timer = 0;
            }
        }
    }

    void SpellSkill(int damage) {
        GameObject go = Instantiate(skillPrefab, transform.position + transform.up * 2, Quaternion.identity) as GameObject;
        go.GetComponent<SkillTriggerAction>().SetOwner(gameObject);
        go.transform.forward = transform.forward;
        go.GetComponent<SkillTriggerAction>().damage = damage;
        skillTrigger.Add(go);
    }

    void CancelSkill() {
        foreach (GameObject i in skillTrigger) {
            Destroy(i);
        }
        skillTrigger.Clear();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(curhp);
            stream.SendNext(myHpBar.GetActive());
            stream.SendNext(targetPhotonID);
        }
        else {
            curhp = (int)stream.ReceiveNext();
            myHpBar.SetActive((bool)stream.ReceiveNext());
            targetPhotonID = (int)stream.ReceiveNext();
        }
    }
}
