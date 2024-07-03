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
    private float normalSize = 1.8f;
    private float jumpingSize = 1.2f;

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
    private bool isSliding;
    private Vector3 slopeSlideVelocity;
    private float heightVelocity = 0.0f;
    private float smoothTime = 0.1f;

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

        isGrounded = controller.isGrounded;

        if (Input.GetKey(KeyCode.LeftShift) && isGrounded || Input.GetKey(KeyCode.JoystickButton1) && isGrounded)
        {
            inputMagnitude *= 2;
        }

        animator.SetFloat("Input Magnitude", inputMagnitude, 0.05f, Time.deltaTime);

        direction.Normalize();

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
        if (isJumping && ySpeed > 0 && Input.GetButton("Jump") == false)
        {
            gravity *= 2;
        }

        ySpeed += gravity * Time.deltaTime;

        SetSlopeSlideVelocity();

        if (slopeSlideVelocity == Vector3.zero)
        {
            isSliding = false;
        }

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
            if (slopeSlideVelocity != Vector3.zero)
            {
                isSliding = true;
            }

            controller.stepOffset = originalStepOffset;

            if (isSliding == false)
            {
                ySpeed = -0.5f;
            }

            animator.SetBool("IsGrounded", true);
            isGrounded = true;
            animator.SetBool("IsJumping", false);
            isJumping = false;

            controller.height = Mathf.SmoothDamp(controller.height, normalSize, ref heightVelocity, smoothTime);
            controller.center = new Vector3(0, controller.height / 2, 0);
            animator.SetBool("IsFalling", false);

            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod && isSliding == false)
            {
                ySpeed = Mathf.Sqrt(jumpHeight * -3f * gravity);
                animator.SetBool("IsJumping", true);
                isJumping = true;

                controller.height = Mathf.SmoothDamp(controller.height, jumpingSize, ref heightVelocity, smoothTime);
                controller.center = new Vector3(0, controller.height / 2, 0);

                jumpButtonPressedTime = null;
                lastGroundedTime = null;
            }
        }
        else
        {
            controller.stepOffset = 0;
            animator.SetBool("IsGrounded", false);
            isGrounded = false;
            if ((isJumping && ySpeed < -2) || ySpeed < -5f)
            {
                animator.SetBool("IsFalling", true);

                controller.height = Mathf.SmoothDamp(controller.height, jumpingSize, ref heightVelocity, smoothTime);
                controller.center = new Vector3(0, controller.height / 2, 0);
            }
        }

        Debug.Log("yVelocity: " + ySpeed);

        if (isGrounded == false && isSliding == false)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton1))
            {
                inputMagnitude *= 2.5f;
            }

            Vector3 velocity = moveDir * inputMagnitude * jumpHorizontalSpeed;
            velocity.y = ySpeed;
            controller.Move(velocity * Time.deltaTime);
        }

        if (isSliding)
        {
            Vector3 velocity = slopeSlideVelocity;
            velocity.y = ySpeed;

            controller.Move(velocity * Time.deltaTime);
        }
    }

    private void OnAnimatorMove()
    {
        if (isGrounded && isSliding == false)
        {
            Vector3 velocity = animator.deltaPosition;
            velocity.y = ySpeed * Time.deltaTime;

            controller.Move(velocity);
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private void SetSlopeSlideVelocity()
    {
        if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hitInfo, 5))
        {
            float angle = Vector3.Angle(hitInfo.normal, Vector3.up);

            if (angle >= controller.slopeLimit)
            {
                slopeSlideVelocity = Vector3.ProjectOnPlane(new Vector3(0, ySpeed, 0), hitInfo.normal);
                return;
            }
        }
        if (isSliding)
        {
            slopeSlideVelocity -= slopeSlideVelocity * Time.deltaTime * 3;

            if (slopeSlideVelocity.magnitude > 1)
            {
                return;
            }
        }

        slopeSlideVelocity = Vector3.zero;
    }
}
