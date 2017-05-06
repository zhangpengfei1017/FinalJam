using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator anim;

    private int speedFwdHash = 0;
    private int speedRtHash = 0;
    private int skillIdxHash = 0;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        speedFwdHash = Animator.StringToHash("SpeedForward");
        speedRtHash = Animator.StringToHash("SpeedRight");
        skillIdxHash = Animator.StringToHash("SkillIdx");
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat(speedFwdHash, Input.GetAxis("Vertical"));
        anim.SetFloat(speedRtHash, Input.GetAxis("Horizontal"));

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim.SetInteger(skillIdxHash, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetInteger(skillIdxHash, 2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            anim.SetInteger(skillIdxHash, 3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            anim.SetInteger(skillIdxHash, 4);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            anim.SetInteger(skillIdxHash, 5);
        }
    }

    void CreateSkillEffect()
    {
        anim.SetInteger(skillIdxHash, 0);
    }
}
