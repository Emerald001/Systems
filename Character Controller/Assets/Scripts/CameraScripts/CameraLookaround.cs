using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookaround : MonoBehaviour 
{
    public Transform ObjectToFollow;
    public Transform XRotTransform;

    public Transform player;

    public float mouseSensitivity = 100f;
    public float followSpeed;
    public float MaxAngleLook = 65f;
    public float MinAngleLook = 65f;
    
    public bool turnPlayer;

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
        xRotation = Mathf.Clamp(xRotation, MinAngleLook, MaxAngleLook);

        XRotTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        if(turnPlayer)
            player.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}