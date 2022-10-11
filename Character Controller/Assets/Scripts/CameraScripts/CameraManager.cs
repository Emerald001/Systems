using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    public GameObject Camera;
    public GameObject FocalPoint;
    public Transform YRotTransform;
    public Transform XRotTransform;

    [Header("Settings")]
    public float mouseSensitivity = 100f;
    public float followSpeed;

    //States
    private StateMachine<CameraManager> CameraStateMachine;
    [HideInInspector] public MovementCameraState movementCameraState;
    [HideInInspector] public AimState aimCameraState;

    void Start() {
        CameraStateMachine = new(this);

        var aimState = new AimState(CameraStateMachine);
        var movementCameraState = new MovementCameraState(CameraStateMachine);

        CameraStateMachine.AddState(typeof(AimState), aimState);
        CameraStateMachine.AddState(typeof(MovementCameraState), movementCameraState);
        CameraStateMachine.ChangeState(typeof(MovementCameraState));
    }

    void Update() {
        CameraStateMachine.OnUpdate();
    }

    public void ChangeState(State<GameObject> state) {
        //CameraStateMachine.ChangeState(state);
    }
}