﻿using System.Collections;
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

    public Transform keepToCharacter;

    public bool useOffsetForKeptEffect;

    public bool isGround;

    private float timer = 0;


    private GameObject from;
    private GameObject to;

    private bool delayEffect = false;

    //ray
    private float distance;
    
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
                {
                    //point to point 
                    Vector3 target;
                    if (isGround)
                    {
                        target = to.transform.position;
                    }
                    else
                    {
                        GameCharacter gameChar = to.GetComponent<GameCharacter>();
                        if (null != gameChar)
                        {
                            target = gameChar.characterCenter.position;
                        }
                        else
                        {
                            target = to.GetComponent<Collider>().bounds.center;
                        }
                    }

                    Vector3 dir = (target - transform.position).normalized;
                    transform.position += dir * speed * Time.deltaTime;
                    if (Vector3.Distance(transform.position, target) <= 1f)
                    {
                        if (collisionEffect != null)
                        {
                            GameObject col = Instantiate(collisionEffect, transform.position, transform.rotation) as GameObject;
                            autoDestroy ad = col.AddComponent<autoDestroy>();
                            ad.destroyTime = collisionDestroyTime;
                        }
                        Destroy(gameObject);
                    }
                }
                break;
            case SkillEffectType.ray:
                AdjustRayDirection();
                break;
        }
    }

    private void AdjustRayDirection()
    {
        Vector3 target;
        if (isGround)
        {
            target = to.transform.position;
        }
        else
        {
            GameCharacter gameChar = to.GetComponent<GameCharacter>();
            if (null != gameChar)
            {
                target = gameChar.characterCenter.position;
            }
            else
            {
                target = to.GetComponent<Collider>().bounds.center;
            }
        }
        transform.LookAt(target);
        if (timer >= delayCollisionTime && !delayEffect)
        {
            if (collisionEffect != null)
            {
                GameObject col = Instantiate(collisionEffect, target, transform.rotation) as GameObject;
                Transform colt = col.GetComponent<Transform>();
                colt.forward = transform.forward;
                autoDestroy ad = col.AddComponent<autoDestroy>();
                ad.destroyTime = collisionDestroyTime;
                delayEffect = true;
            }
        }
    }

    public void SetLine(GameObject from, GameObject to)
    {
        this.from = from;
        this.to = to;

        if (null != keepToCharacter)
        {
            keepToCharacter.position = from.transform.position + (useOffsetForKeptEffect ? offset : Vector3.zero);
            keepToCharacter.rotation = from.transform.rotation;
        }
    }

    public void SetTarget(Transform target)
    {
        from = gameObject;
        to = target.gameObject;

        if (skillEffectType == SkillEffectType.ray)
        {
            AdjustRayDirection();
        }

        if (null != keepToCharacter)
        {
            keepToCharacter.position = from.transform.position + (useOffsetForKeptEffect ? offset : Vector3.zero);
            keepToCharacter.rotation = from.transform.rotation;
        }
    }

    public float GetRayLength()
    {
        Vector3 target;
        if (isGround)
        {
            target = to.transform.position;
        }
        else
        {
            GameCharacter gameChar = to.GetComponent<GameCharacter>();
            if (null != gameChar)
            {
                target = gameChar.characterCenter.position;
            }
            else
            {
                target = to.GetComponent<Collider>().bounds.center;
            }
        }
        return Vector3.Distance(transform.position, target);
    }
}
