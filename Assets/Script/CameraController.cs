using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform target;

    public float cameraXSpeed = 1;
    public float cameraYSpeed = 1;
    [Range(-90f, 0)]
    public float minTiltAngle = -45f;
    [Range(0, 90)]
    public float maxTiltAngle = 75f;
    public bool xInverted = false;
    public bool yInverted = false;
    public float cameraSmoothing = 0;
    public float cameraMoveSpeed = 1;
    private Transform cameraPivot;
    private Transform mainCamera;
    private Vector3 pivotEulers;
    private Quaternion pivotTargetRot;
    private Quaternion targetRot;
    private float lookAngle;
    private float tiltAngle;

    void Awake () {
        mainCamera = GetComponentInChildren<Camera>().transform;
        cameraPivot = mainCamera.parent;

        pivotEulers = cameraPivot.rotation.eulerAngles;
        pivotTargetRot = cameraPivot.transform.localRotation;
        targetRot = transform.localRotation;

    }

    void FixedUpdate () {
        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        lookAngle += x * cameraXSpeed;

        // Rotate the rig (the root object) around Y axis only:
        targetRot = Quaternion.Euler(0f, lookAngle, 0f);

        // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
        tiltAngle -= y * cameraYSpeed;
        // and make sure the new value is within the tilt range
        tiltAngle = Mathf.Clamp(tiltAngle, minTiltAngle, maxTiltAngle);

        // Tilt input around X is applied to the pivot (the child of this object)
        pivotTargetRot = Quaternion.Euler(tiltAngle, pivotEulers.y, pivotEulers.z);

        if (cameraSmoothing > 0) {
            cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, pivotTargetRot, cameraSmoothing * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRot, cameraSmoothing * Time.deltaTime);
        }
        else {
            cameraPivot.localRotation = pivotTargetRot;
            transform.localRotation = targetRot;
        }


    }

    void LateUpdate () {

        transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * cameraMoveSpeed);

    }
}
