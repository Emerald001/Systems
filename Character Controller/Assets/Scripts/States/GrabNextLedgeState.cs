using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabNextLedgeState : MoveState
{
    public GrabNextLedgeState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public GameObject Ledge;
    public GameObject newLedge;
    public bool isDone = false;
    public Vector3 dir = new();

    public override void OnEnter() {
        Ledge = owner.evaluator.CanGrabLedge();

        if (Input.GetKey(KeyCode.W)) {
            var newLedge = owner.evaluator.CanGrabLedge(owner.transform.up);

            if (newLedge) {
                dir = new Vector3(0, newLedge.transform.position.y - owner.transform.position.y, 0).normalized;
                this.newLedge = newLedge;
            }
        }

        if (Input.GetKey(KeyCode.S)) {
            var newLedge = owner.evaluator.CanGrabLedge(-owner.transform.up);

            if (newLedge) {
                dir = new Vector3(0, owner.transform.position.y - newLedge.transform.position.y, 0).normalized;
                this.newLedge = newLedge;
            }
        }

        isDone = false;
    }

    public override void OnExit() {
        isDone = false;
    }

    public override void OnUpdate() {
        if (owner.evaluator.CanGrabLedge() != newLedge) {
            owner.velocity = dir * 5;
        }
        else {
            isDone = true;
        }

        base.OnUpdate();
    }
}