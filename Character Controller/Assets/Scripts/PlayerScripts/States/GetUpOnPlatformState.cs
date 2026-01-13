using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetUpOnPlatformState : MoveState {

    public GetUpOnPlatformState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public bool IsDone = false;
    public Vector3 endpoint;

    public override void OnEnter() {
        endpoint = owner.Evaluator.CanGoOntoLedge();

        //owner.CurrentLedge = null;

        owner.Animator.SetTrigger("GetOntoPlatform");
        IsDone = false;
    }

    public override void OnExit() {
        IsDone = false;
    }

    public override void OnUpdate() {
        if (Vector3.Distance(owner.transform.position, endpoint) > .2f) {
            owner.Velocity = (endpoint - owner.transform.position).normalized * 5;
        }
        else {
            IsDone = true;
        }

        base.OnUpdate();
    }
}