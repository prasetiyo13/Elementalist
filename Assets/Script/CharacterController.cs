using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

    private Transform camTransform;
    private Camera cam;
    private Animator anim;

    private float speed;
    private float angle;
    private float direction;

    private float walkSpeed = 0.6f;
    private float runSpeed = 1.2f;
    private float sprintSpeed = 2.0f;
    private float directionSpeed = 1.5f;
    private float fovDampTime = 3f;
    [SerializeField]
    private float speedDampTime = 0.05f;
    [SerializeField]
    private float stopDampTime = 0.5f;
    [SerializeField]
    private float directionDampTime = 0.5f;
    private float LocomotionThreshold = 0.2f;

    private float turnAngle = 0;
    private bool turnOnPivot = true;
    public enum AnimationMode {
        BASIC = 0,
        SWORD_AND_SHIELD = 1
    }
    [SerializeField]
    private AnimationMode characterMode = 0;

    public AnimationMode CharacterMode {
        get {
            return characterMode;
        }

        set {
            characterMode = value;
        }
    }

    void Awake () {
        camTransform = Camera.main.transform;
        cam = camTransform.GetComponent<Camera>();
        anim = GetComponent<Animator>();

    }

    void Update () {
        //
        if (Input.GetKeyDown(KeyCode.Q) || (Input.GetMouseButtonDown(0) && (characterMode == AnimationMode.BASIC))) {
            if(characterMode == AnimationMode.BASIC) {
                anim.SetTrigger("Draw");
            }
            else {
                anim.SetTrigger("Sheath");
            }
        }
        if (Input.GetMouseButtonDown(0) && anim.GetLayerWeight((int)AnimationMode.SWORD_AND_SHIELD) == 1) {
            anim.SetTrigger("Attack");
        }
        if (Input.GetMouseButtonDown(1) && anim.GetLayerWeight((int) AnimationMode.SWORD_AND_SHIELD) == 1) {
            anim.SetTrigger("SpAttack");
        }

        anim.SetBool("OnAttack", anim.GetCurrentAnimatorStateInfo(1).IsTag("Attack"));
        
        //input
        float v = Input.GetAxisRaw("Vertical");
        float h = Input.GetAxisRaw("Horizontal");

        //reset sped,direction and angle
        float moveSpeed = 0f;
        direction = 0f;
        angle = 0f;

        //get transform direction
        Vector3 myDirection = transform.forward;

        Vector3 inputDirection = new Vector3(h, 0, v);

        moveSpeed = inputDirection.normalized.sqrMagnitude;

        // Get camera rotation
        Vector3 CameraDirection = camTransform.forward;
        CameraDirection.y = 0.0f; // kill Y
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(CameraDirection));

        // Convert joystick input in Worldspace coordinates
        Vector3 moveDirection = referentialShift * inputDirection;
        Vector3 axisSign = Vector3.Cross(moveDirection, myDirection);

        float angleRootToMove = Vector3.Angle(myDirection, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);

        angle = angleRootToMove;

        angleRootToMove /= 180f;

        direction = angleRootToMove * directionSpeed;

        // Sprint
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            speed = Mathf.Lerp(speed, moveSpeed * sprintSpeed, Time.deltaTime);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 75f, fovDampTime * Time.deltaTime);
        }
        else {
            // walk
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                speed = Mathf.Lerp(speed, moveSpeed * walkSpeed, Time.deltaTime);
            }
            else {
                speed = moveSpeed * runSpeed;
            }
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60f, fovDampTime * Time.deltaTime);
        }

        float turnSpeed = Mathf.Lerp(180f , 360f, speed);
        transform.Rotate(0, anim.GetFloat("Direction") / 2f * turnSpeed * Time.deltaTime, 0);

        float dampTime = (anim.GetFloat("Speed") > Mathf.Max(speed, 0.8f)) ? stopDampTime : speedDampTime;
        anim.SetFloat("Speed", speed, dampTime, Time.deltaTime);
        anim.SetFloat("Direction", direction, directionDampTime, Time.deltaTime);

        if (speed < LocomotionThreshold && Mathf.Abs(h) < 0.05f) {
            anim.SetFloat("Direction", 0f);
            angle = 0f;
        }
        bool startTurn = anim.GetAnimatorTransitionInfo(0).IsUserName("StartTurn") || anim.GetCurrentAnimatorStateInfo(0).IsTag("PivotTurn");

        if (startTurn && turnOnPivot) {
            turnAngle = angle;
            turnOnPivot = false;
        }
        if (!startTurn) {
            turnAngle = angle;
            turnOnPivot = true;
        }
        anim.SetFloat("Angle", turnAngle);
    }

}
