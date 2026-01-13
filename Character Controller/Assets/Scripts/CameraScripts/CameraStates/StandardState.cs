using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCameraState : CameraState 
{
    private float xRotation = 0f;
    private float yRotation = 0f;

    private Vector3 followDis = new(0, 0, -8);

    public MovementCameraState(StateMachine<CameraManager> owner) : base(owner) {
        this.owner = StateMachine.Owner;
    }

    public override void OnEnter() {
        xRotation = owner.XRotTransform.eulerAngles.x;
        yRotation = owner.YRotTransform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void OnExit() {

    }

    public override void OnUpdate() {
        owner.YRotTransform.transform.position = Vector3.Lerp(owner.YRotTransform.transform.position, owner.FocalPoint.transform.position, owner.followSpeed * Time.deltaTime);
        owner.Camera.transform.localPosition = Vector3.Lerp(owner.Camera.transform.localPosition, followDis, owner.followSpeed * Time.deltaTime);

        float mouseX = Input.GetAxis("Mouse X") * owner.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * owner.mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, 0, 90f);

        owner.XRotTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        owner.YRotTransform.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            //owner.ChangeState(owner.aimCameraState);
        }
    }
}