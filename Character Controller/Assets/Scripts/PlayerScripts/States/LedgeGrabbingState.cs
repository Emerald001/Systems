using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbingState : MoveState {
    public LedgeGrabbingState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public GameObject Ledge;

    public override void OnEnter() {
        owner.LookAtMoveDir = false;
        owner.Velocity = Vector3.zero;

        Ledge = owner.CurrentLedge;

        owner.Animator.SetBool("HangingFromEdge", true);
    }

    public override void OnExit() {

    }
    
    public override void OnUpdate() {
        Vector3 desiredForward = Vector3.RotateTowards(owner.transform.forward, -owner.CurrentLedge.transform.forward, 1 * Time.deltaTime, 0f);
        desiredForward.y = 0;
        owner.transform.LookAt(owner.transform.position + desiredForward);

        base.OnUpdate();
    }
}