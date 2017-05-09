using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NewPlayerController))]
public class TriggerSkillEffect : MonoBehaviour
{
    public int skillId = 0;
    public Transform target;
    public bool trigger = false;

    void Start()
    {
#if !UNITY_EDITOR
        Debug.LogError("Remove me!");
#endif
    }

    void Update()
    {
        if (trigger)
        {
            trigger = false;
            GetComponent<NewPlayerController>().StartSkill(skillId, target);
        }
    }

}
