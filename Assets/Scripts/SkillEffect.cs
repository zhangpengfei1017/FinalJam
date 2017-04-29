using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    public enum SkillEffectType
    {
        mine,
        other,        
        line,
        ray
    }
    public Vector3 offset;

    public SkillEffectType skillEffectType = SkillEffectType.mine;

    public float speed;

    public float destroyTime;    

    public GameObject collisionEffect;

    public float delayCollisionTime;

    private float timer = 0;


    private GameObject from;
    private GameObject to;

    private bool delayEffect=false;


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
            case SkillEffectType.line:
                //point to point 
                Vector3 target = to.GetComponent<GameCharacter>().characterCenter.position;
                Vector3 dir = (target - transform.position).normalized;
                transform.position += dir * speed * Time.deltaTime;
                if (Vector3.Distance(transform.position, target) <= 1f)
                {
                    GameObject col = Instantiate(collisionEffect, transform.position, transform.rotation) as GameObject;
                    autoDestroy ad=col.AddComponent<autoDestroy>();
                    ad.destroyTime = 3;
                    Destroy(gameObject);
                }
                break;
            case SkillEffectType.ray:
                if (timer >= delayCollisionTime  && !delayEffect) {
                    Transform t = to.GetComponent<GameCharacter>().characterCenter;
                    GameObject col = Instantiate(collisionEffect,t.position, transform.rotation) as GameObject;
                    Transform colt = col.GetComponent<Transform>();
                    colt.forward = transform.forward;
                    autoDestroy ad = col.AddComponent<autoDestroy>();
                    ad.destroyTime = 3;
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
    public float GetRayLength() {
        return Vector3.Distance(transform.position, to.GetComponent<GameCharacter>().characterCenter.position);
    }
}
