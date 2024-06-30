using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator playerAnimation;
    public Rigidbody playerRigidBody;
    public float f_speed, fs_speed, oldfs_speed, ro_speed, b_speed;
    public bool walking;
    public Transform playerTransform;

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            playerRigidBody.velocity = transform.forward * f_speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            playerRigidBody.velocity = -transform.forward * b_speed * Time.deltaTime;
        }
        if(Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            playerRigidBody.velocity = transform.forward * 0;

        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerAnimation.SetTrigger("Walk");
            playerAnimation.ResetTrigger("Idle");
            walking = true;        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            playerAnimation.ResetTrigger("Walk");
            playerAnimation.SetTrigger("Idle");         
            walking = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            playerAnimation.SetTrigger("Backward");
            playerAnimation.ResetTrigger("Idle");
            walking = true;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            playerAnimation.ResetTrigger("Backward");
            playerAnimation.SetTrigger("Idle");
            walking = false;
        }

        if (Input.GetKey(KeyCode.A))
        {
            playerTransform.Rotate(0, -ro_speed * Time.deltaTime, 0);
        }

        if (Input.GetKey(KeyCode.D))
        {
            playerTransform.Rotate(0, ro_speed * Time.deltaTime, 0);
        }


        if (walking)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                f_speed += fs_speed;
                playerAnimation.SetTrigger("Sprint");
                playerAnimation.ResetTrigger("Walk");
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                f_speed = oldfs_speed;
                playerAnimation.ResetTrigger("Sprint");
                playerAnimation.SetTrigger("Walk");
                
            }
        }

    }
}
