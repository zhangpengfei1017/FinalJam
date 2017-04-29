using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCharacter : MonoBehaviour {
    public enum CharacterType {
        Player,
        Npc,
        Monster
    }
    public Transform characterCenter;
    public CharacterType characterType;

    void OnMouseDown()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<HereController>().SetTarget(gameObject);
    }
}
