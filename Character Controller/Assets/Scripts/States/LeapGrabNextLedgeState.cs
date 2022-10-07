using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapGrabNextLedgeState : MoveState {

    public LeapGrabNextLedgeState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public bool isDone = false;

    public override void OnEnter() {
        isDone = false;

        owner.animator.SetTrigger("HangJumpUp");
    }

    public override void OnExit() {
        isDone = false;
    }

    public override void OnUpdate() {
        if (Mathf.Abs(owner.transform.position.y + 1.5f - owner.CurrentLedge.transform.position.y) > .05f) {
            var dif = new Vector3(owner.CurrentLedge.transform.position.x - owner.transform.position.x, 1.5f, 0);
            owner.velocity = (owner.CurrentLedge.transform.position - (owner.transform.position + dif)).normalized * 10;
        }
        else {
            isDone = true;
        }

        base.OnUpdate();
    }
}