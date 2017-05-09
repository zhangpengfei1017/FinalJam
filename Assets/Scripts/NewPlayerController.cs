using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    public Camera cam;

    public float moveSpeed = 1.0f;

    public NewSkillEffect[] skillEffects;

    private Animator anim;

    private CharacterController charCtrl;

    private int speedFwdHash = 0;
    private int speedRtHash = 0;
    private int skillIdxHash = 0;
    private int isMovingHash = 0;
    private int dieHash = 0;

    private int stateLocomotionHash = 0;
    private int stateNoControlHash = 0;

    private NewSkillEffect curFx = null;
    private int curSkillId = -1;
    private int curEventId = 0;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        charCtrl = GetComponent<CharacterController>();
        speedFwdHash = Animator.StringToHash("SpeedForward");
        speedRtHash = Animator.StringToHash("SpeedRight");
        skillIdxHash = Animator.StringToHash("SkillIdx");
        isMovingHash = Animator.StringToHash("IsMoving");
        dieHash = Animator.StringToHash("Die");

        stateLocomotionHash = Animator.StringToHash("Locomotion");
        stateNoControlHash = Animator.StringToHash("NoControl");

        if (null == cam)
        {
            cam = Camera.main;
        }
        //AdjustCamera(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (curSkillId != -1)
        {
            if (null == curFx)
            {
                curSkillId = -1;
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            CancelSkill();
        }
    }

    public void Move(float moveFwd, float moveRt)
    {
        Vector3 moveDelta = new Vector3(moveRt, 0, Mathf.Clamp(moveFwd, -0.5f, 1.0f));
        Quaternion camRotY = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
        transform.rotation = camRotY;

        charCtrl.SimpleMove(camRotY * moveDelta * Time.deltaTime * moveSpeed);
        
        anim.SetBool(isMovingHash, moveDelta.sqrMagnitude > 0.01f);
        anim.SetFloat(speedFwdHash, moveFwd);
        anim.SetFloat(speedRtHash, moveRt);
    }

    public void Move(Vector3 direction, float rotation)
    {
        Quaternion rot = Quaternion.Euler(0, rotation, 0);
        transform.rotation = rot;
        
        charCtrl.SimpleMove(direction * Time.deltaTime * moveSpeed);

        float speed = direction.magnitude;
        anim.SetBool(isMovingHash, speed > 0.01f);
        anim.SetFloat(speedFwdHash, speed);
    }

    public void StartSkill(int skillId, Transform target)
    {
        if (skillId >= skillEffects.Length || skillId < 0)
            return;

        curSkillId = skillId;
        anim.SetInteger(skillIdxHash, skillId + 1);
        curEventId = 0;

        if (null != skillEffects[curSkillId])
        {
            var fx = Instantiate(skillEffects[curSkillId], transform);
            fx.Target = target;
            fx.gameObject.SetActive(true);
            curFx = fx;
        }
    }

    void CreateSkillEffect()
    {
        anim.SetInteger(skillIdxHash, 0);
        if (null != curFx)
            curFx.OnEventTriggered(curEventId++);
    }

    public void CancelSkill()
    {
        anim.SetInteger(skillIdxHash, 0);
        anim.Play(stateLocomotionHash, 0);
        anim.Play(stateNoControlHash, 1);
        if (null != curFx)
        {
            Destroy(curFx.gameObject);
        }
    }

    public void Die()
    {
        CancelSkill();
        anim.SetTrigger(dieHash);
    }

    public void SetAnimationSpeed(float speed)
    {
        anim.speed = speed;
    }

}
