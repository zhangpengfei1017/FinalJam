using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameCharacter))]

public class MonsterController : MonoBehaviour {

    public enum MonsterType
    {
        Trash,
        Boss
    };

    [SerializeField]
    private string monsterName;

    [SerializeField]
    private MonsterType monsterType = MonsterType.Trash;

    private GameCharacter character;

    // Use this for initialization
    void Start () {
        character = GetComponent<GameCharacter>();
	}
	
	// Update is called once per frame
	void Update () {

	}
}
