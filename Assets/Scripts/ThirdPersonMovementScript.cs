using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThirdPersonMovementScript : MonoBehaviour
{
    private CharacterController controller;
    private float originalStepOffset;
    public float jumpButtonGracePeriod;

    public Transform cam;
    private float ySpeed;
    private float jumpHeight = 1f;
    private float gravityMultiplier = 1f;

    private Animator animator;
    private float? lastGroundedTime;
    private float? jumpButtonPressedTime;

    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;
    private float targetAngle;
    private Vector3 moveDir;

    private bool isJumping;
    private bool isGrounded;
    private float jumpHorizontalSpeed = 3;
    private float jumpVelocity;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        originalStepOffset = controller.stepOffset;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        float inputMagnitude = Mathf.Clamp01(direction.magnitude);
        inputMagnitude /= 2;

        //hold down shift to walk

        isGrounded = controller.isGrounded;

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded || Input.GetKey(KeyCode.JoystickButton1) && isGrounded)
        {
            inputMagnitude *= 2;
        }

        animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);

        direction.Normalize();

        // Only update rotation if there is input


        if (direction.magnitude >= 0.1f)
        {
            animator.SetBool("IsWalking", true);
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            moveDir = moveDirection;
            moveDirection.Normalize();
        }
        else
        {
            animator.SetBool("IsWalking", false);

        }

        float gravity = Physics.gravity.y * gravityMultiplier;
        if(isJumping && ySpeed > 0 && Input.GetButton("Jump") == false)
        {
            gravity *= 2;
        }

        ySpeed += gravity * Time.deltaTime;

        if (isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpButtonPressedTime = Time.time;
        }

        if (Time.time - lastGroundedTime <= jumpButtonGracePeriod)
        {
            controller.stepOffset = originalStepOffset;
            ySpeed = -0.5f;

            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            isJumping = false;
            animator.SetBool("IsFalling", false);

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {

                ySpeed = Mathf.Sqrt(jumpHeight * -3f * gravity);
                animator.SetBool("IsJumping", true);
                isJumping = true;

                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            controller.stepOffset = 0;
            animator.SetBool("IsGrounded", false);
            isGrounded = false;
            Debug.Log("velocity: "+ ySpeed );
            if ((isJumping && ySpeed < -2) || !isJumping && ySpeed < -5f)
            {
                animator.SetBool("IsFalling", true);
            }
        }

        if(isGrounded == false)
        {

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton1))
            {
                inputMagnitude *= 2.5f;
                
            }

            Vector3 velocity = moveDir * inputMagnitude * jumpHorizontalSpeed;
            velocity.y = ySpeed;
            controller.Move(velocity * Time.deltaTime);
        }


    }

    private void OnAnimatorMove()
    {
        if (isGrounded)
        {
            Vector3 velocity = animator.deltaPosition;
            velocity.y = ySpeed * Time.deltaTime;

            controller.Move(velocity);
        }


    }
    private void OnApplicationFocus(bool focus)
    {
        if(focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
