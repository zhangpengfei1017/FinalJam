﻿using UnityEngine;
using System.Collections;

public class SkillTriggerAction : MonoBehaviour
{
    public float speed;
    public int damage;
    public float dis;


    private GameObject owner;
    public string target;

    // Use this for initialization
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        if (Vector3.Distance(owner.transform.position, transform.position) > dis)
        {
            if (owner.tag == "Player")
            {
                owner.GetComponent<PlayerController>().RemoveTrigger(gameObject);
            }
            else if (owner.tag == "Enemy")
            {
                owner.GetComponent<EnemyController>().RemoveTrigger(gameObject);
            }
            Destroy(gameObject);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.tag == target) {
            if (target == "Enemy")
            {
                other.GetComponent<EnemyController>().GetHit(damage);
            }
            if (target == "Player")
            {
                other.GetComponent<PlayerController>().GetHit(damage);
            }
        }      
    }


    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }
}
