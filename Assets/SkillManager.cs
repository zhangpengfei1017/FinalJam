using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour {

    public  Skill[] allSkills;

    public  Skill FindSkillWithName(string name)
    {
        foreach (Skill s in allSkills)
        {
            if (s.skillName == name)
            {
                return s;
            }
        }
        return null;
    }
}
