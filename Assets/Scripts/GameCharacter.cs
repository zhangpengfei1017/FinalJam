using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour {
    public enum CharacterType {
        Player,
        Npc,
        Monster
    }

    public enum Class
    {
        Knight,
        Sorceress,
        Priest,
        Archer,
    };

    //Character Status

    [SerializeField]
    private string playerName;

    [SerializeField]
    private Class className = Class.Knight;

    [SerializeField]
    private float threatIndex = 1;

    public int maxHP;

    public int curHP;

    [SerializeField]
    public int maxMP;

    public int curMP;

    [SerializeField]
    private int attack;

    [SerializeField]
    public int defense;

    [SerializeField]
    public int moveSpeed;


    //Property ratios

    private float speedRatio = 1;

    private float damageRatio = 1;

    private float healRatio = 1;

    private float cdRatio = 1;

    private float threatRatio = 1;

    private float attackRatio = 1;

    private float defenseRatio = 1;


    //Final properties

    public int finalMaxHP;

    public int finalMaxMP;

    public int finalAttack;

    public int finalDefense;

    public Transform characterCenter;

    public CharacterType characterType;

    void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<HereController>().SetTarget(gameObject);
    }
}
