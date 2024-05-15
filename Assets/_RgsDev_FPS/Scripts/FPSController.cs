using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    public float walkSpeed = 6.0f;
    public float runSpeed = 10.0f;
    [SerializeField] float jumpSpeed = 8.0f;
    [SerializeField] float mouseVelocityX = 3.0f;
    [SerializeField] float mouseVelocityY = 3.0f;
    [SerializeField] float gravity = 20.0f;
    [Tooltip("If has head bobbing effect when walk/run")]
    [SerializeField] bool headBobbing;
    [SerializeField] float bobbingSpeed = 0.2f;
    [SerializeField] float bobbingAmount = 0.2f;
    [Tooltip("If lock and hide mouse cursor when game is running")]
    [SerializeField] bool lockCursor;
    [SerializeField] AudioClip JumpAudio;
    [SerializeField] AudioClip landingAudio;

    GameObject mainCamera;
    CharacterController controller;
    Animator anim;
    AudioSource audioSource;
    Vector3 moveDirection = Vector3.zero;

    float rotationX = 0.0f, rotationY = 0.0f;
    float speed;
    bool running;
    CameraShake cameraBobbing;

    void Start()
    {
        mainCamera = GetComponentInChildren(typeof(Camera)).transform.gameObject;
        mainCamera.transform.localRotation = Quaternion.identity;
        cameraBobbing = mainCamera.GetComponent<CameraShake>();
        controller = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        speed = walkSpeed; //Current speed receives walk speed

        if (lockCursor)
        {
            LockAndHideCursor();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveZ = new Vector3(mainCamera.transform.forward.x, 0, mainCamera.transform.forward.z);
        Vector3 moveX = new Vector3(mainCamera.transform.right.x, 0, mainCamera.transform.right.z);
        moveZ.Normalize();
        moveX.Normalize();
        moveZ = moveZ * Input.GetAxis("Vertical");
        moveX = moveX * Input.GetAxis("Horizontal");
        Vector3 direction = moveZ + moveX;

        running = Input.GetKey(KeyCode.LeftShift);

        if (direction.sqrMagnitude > 1)
            direction.Normalize();

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(direction.x, 0, direction.z);
            moveDirection *= speed;

            //If player is moving
            if(moveDirection.x != 0 || moveDirection.z != 0)
            {
                anim.SetBool("isMoving", true);                

                if (!running)
                {
                    running = false;
                    //Head bobbing effect
                    if (headBobbing)
                        cameraBobbing.HeadBobbing(bobbingSpeed, bobbingAmount);
                }
                else
                {
                    running = true;
                    //Head bobbing effect
                    if (headBobbing)
                        cameraBobbing.HeadBobbing(bobbingSpeed * 2, bobbingAmount);
                }
            }
            else
            {
                running = false;
                anim.SetBool("isMoving", false);
            }

            if (running)
            {
                speed = runSpeed;
                anim.SetBool("isRunning", true);
            }
            else
            {
                speed = walkSpeed;
                anim.SetBool("isRunning", false);
            }

            if (Input.GetButtonDown("Jump"))
            {
                audioSource.PlayOneShot(JumpAudio);
                moveDirection.y = jumpSpeed;
            }            
        }
        //Apply gravity
        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        MouseLook();
        //Get current active child animator in weapons list
        anim = GetComponentInChildren<Animator>();
    }

    void MouseLook()
    {
        rotationX += Input.GetAxis("Mouse X") * mouseVelocityX;
        rotationY += Input.GetAxis("Mouse Y") * mouseVelocityY;
        rotationX = ClampFPSAngle(rotationX, -360, 360);
        rotationY = ClampFPSAngle(rotationY, -80, 80);
        Quaternion QuaternionX = Quaternion.AngleAxis(rotationX, Vector3.up);
        Quaternion QuaternionY = Quaternion.AngleAxis(rotationY, -Vector3.right);
        Quaternion rotation = Quaternion.identity * QuaternionX * QuaternionY;
        mainCamera.transform.localRotation = Quaternion.Lerp(mainCamera.transform.localRotation, rotation, Time.deltaTime * 10.0f);
    }

    float ClampFPSAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;

        if (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    void LockAndHideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnTriggerEnter(Collider other)
    {
        //If player collides with ground tag
        if (other.gameObject.tag == "Ground")
        {
            audioSource.PlayOneShot(landingAudio);
        }
    }
}
