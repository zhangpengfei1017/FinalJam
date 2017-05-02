using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public enum SkillEffectType
    {
        mine,
        other,
        move,
        ray
    }
    public Vector3 offset;

    public SkillEffectType skillEffectType = SkillEffectType.mine;

    public float speed;

    public float destroyTime;

    public GameObject collisionEffect;

    public float collisionDestroyTime;

    public float delayCollisionTime;

    public bool isGround;

    private float timer = 0;


    private GameObject from;
    private GameObject to;

    private bool delayEffect = false;

    //void Start()
    //{
    //    var tm = GetComponentInChildren<RFX4_TransformMotion>(true);
    //    if (tm != null)
    //    {
    //        //print("yes");
    //        tm.CollisionEnter += Tm_CollisionEnter;
    //    }

    //    //else print("null");
    //}

    //private void Tm_CollisionEnter(object sender, RFX4_TransformMotion.RFX4_CollisionInfo e)
    //{
    //    Debug.Log(e.Hit.transform.name); //will print collided object name to the console.
    //}

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= destroyTime)
        {
            Destroy(gameObject);
        }
        switch (skillEffectType)
        {
            case SkillEffectType.mine:
                //show on youself

                break;
            case SkillEffectType.other:
                //show on the target

                break;
            case SkillEffectType.move:
                //point to point 
                Vector3 target;
                if (isGround)
                {
                    target = to.transform.position;
                }
                else
                {
                    target = to.GetComponent<GameCharacter>().characterCenter.position;
                }

                Vector3 dir = (target - transform.position).normalized;
                transform.position += dir * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, target) <= 1f)
                {
                    GameObject col = Instantiate(collisionEffect, transform.position, transform.rotation) as GameObject;
                    autoDestroy ad = col.AddComponent<autoDestroy>();
                    ad.destroyTime = collisionDestroyTime;
                    Destroy(gameObject);
                }
                break;
            case SkillEffectType.ray:
                if (timer >= delayCollisionTime && !delayEffect)
                {
                    Vector3 tar;
                    if (isGround)
                    {
                        tar = to.transform.position;
                    }
                    else
                    {
                        tar = to.GetComponent<GameCharacter>().characterCenter.position;
                    }
                    GameObject col = Instantiate(collisionEffect, tar, transform.rotation) as GameObject;
                    Transform colt = col.GetComponent<Transform>();
                    colt.forward = transform.forward;
                    autoDestroy ad = col.AddComponent<autoDestroy>();
                    ad.destroyTime = collisionDestroyTime;
                    delayEffect = true;
                }
                break;
        }
    }
    public void SetLine(GameObject from, GameObject to)
    {
        this.from = from;
        this.to = to;
    }
    public float GetRayLength()
    {
        return Vector3.Distance(transform.position, to.GetComponent<GameCharacter>().characterCenter.position);
    }
}
