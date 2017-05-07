﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerController : MonoBehaviour
{
    public Camera cam;

    public float moveSpeed = 1.0f;
    public float mouseSensitive = 1.0f;

    private Animator anim;

    private int speedFwdHash = 0;
    private int speedRtHash = 0;
    private int skillIdxHash = 0;
    private int isMovingHash = 0;
    private int dieHash = 0;


    Vector3 focusPoint {  get { return transform.position + Vector3.up; } }

    Vector3 lookDir = Vector3.zero;
    float lookDistance = 0;


    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        speedFwdHash = Animator.StringToHash("SpeedForward");
        speedRtHash = Animator.StringToHash("SpeedRight");
        skillIdxHash = Animator.StringToHash("SkillIdx");
        isMovingHash = Animator.StringToHash("IsMoving");
        dieHash = Animator.StringToHash("Die");

        if (null == cam)
        {
            cam = Camera.main;
        }
        lookDir = cam.transform.position - focusPoint;
        AdjustCamera(0, 0);
    }

    void AdjustCamera(float deltaUp, float deltaRight)
    {
        Vector3 focus = focusPoint;
        lookDir = Quaternion.Euler(-deltaUp, deltaRight, 0) * lookDir;
        cam.transform.position = focus + lookDir;
        cam.transform.LookAt(focus);
    }

    // Update is called once per frame
    void Update()
    {
        float moveFwd = Input.GetAxis("Vertical");
        float moveRt = Input.GetAxis("Horizontal");
        float lookUp = Input.GetAxis("Mouse Y") * mouseSensitive;
        float lookRt = Input.GetAxis("Mouse X") * mouseSensitive;
        
        Vector3 moveDelta = new Vector3(moveRt, 0, Mathf.Clamp(moveFwd, -0.5f, 1.0f));
        Quaternion camRotY = Quaternion.Euler(0, cam.transform.rotation.eulerAngles.y, 0);
        transform.rotation = camRotY;
        
        transform.transform.Translate(moveDelta * Time.deltaTime * moveSpeed, Space.Self);
        
        if (Input.GetMouseButton(1))
        {
            AdjustCamera(lookUp, lookRt);
        }
        else
        {
            AdjustCamera(0, 0);
        }

        anim.SetBool(isMovingHash, moveDelta.sqrMagnitude > 0.01f);
        anim.SetFloat(speedFwdHash, moveFwd);
        anim.SetFloat(speedRtHash, moveRt);

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
