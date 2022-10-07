using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeapGrabNextLedgeState : MoveState {

    public LeapGrabNextLedgeState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public GameObject Ledge;
    public GameObject newLedge;
    public bool isDone = false;
    public Vector3 dir = new();

    public override void OnEnter() {
        Ledge = owner.evaluator.CanGrabLedge();

        isDone = false;
    }

    public override void OnExit() {
        isDone = false;
    }

    public override void OnUpdate() {
        if (Mathf.Abs((owner.transform.position.y + 1) - newLedge.transform.position.y) > .01f) {
            owner.velocity = dir * 5;
        }
        else {
            isDone = true;
        }

        base.OnUpdate();
    }
}