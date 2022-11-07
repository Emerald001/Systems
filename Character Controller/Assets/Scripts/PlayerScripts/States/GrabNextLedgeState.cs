using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabNextLedgeState : MoveState
{
    public GrabNextLedgeState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public bool isDone = false;
    public GameObject Ledge;
    public Vector3 TargetPos;

    public override void OnEnter() {
        isDone = false;

        Ledge = owner.CurrentLedge;

        TargetPos = owner.CurrentLedgePoint;
        owner.canGrabNextLedge = false;
        //owner.animator.SetTrigger("HangJumpUp");
    }

    public override void OnExit() {
        isDone = false;
    }

    public override void OnUpdate() {
        var forwards = owner.transform.forward * .63f;

        var offset = TargetPos - (owner.transform.position + new Vector3(0, 1.5f, 0));
        if (offset.magnitude > .1f) {
            owner.velocity = offset.normalized * 5;
        }
        else {
            owner.CurrentLedgePoint = Vector3.zero;
            owner.ResetTimer();
            isDone = true;
        }

        Debug.Log("In Here");

        Vector3 desiredForward = Vector3.RotateTowards(owner.transform.forward, -owner.CurrentLedge.transform.forward, 1 * Time.deltaTime, 0f);
        desiredForward.y = 0;
        owner.transform.LookAt(owner.transform.position + desiredForward);

        base.OnUpdate();
    }
}