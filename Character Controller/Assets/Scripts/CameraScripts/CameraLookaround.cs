using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookaround : MonoBehaviour 
{
    public Transform ObjectToFollow;
    public Transform XRotTransform;
    public float mouseSensitivity = 100f;
    public float followSpeed;
    private float xRotation = 0f;
    private float yRotation = 0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update() {
        transform.position = Vector3.Lerp(transform.position, ObjectToFollow.transform.position, followSpeed * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, 0, 65f);

        XRotTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
        //transform.Rotate(Vector3.up * mouseX);
    }
}