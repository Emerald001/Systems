using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CameraState : State<CameraManager> {
    public CameraManager owner;

    public CameraState(StateMachine<CameraManager> owner) : base(owner) {
        this.owner = stateMachine.Owner;
    }
}