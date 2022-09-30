using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbingState : MoveState {
    public LedgeGrabbingState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public GameObject Ledge;

    public override void OnEnter() {
        owner.animator.SetBool("HangingFromEdge", true);

        Ledge = owner.evaluator.CanGrabLedge();
    }

    public override void OnExit() {
        owner.animator.SetBool("HangingFromEdge", false);
    }

    public override void OnUpdate() {
        owner.velocity = Vector3.zero;

        var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);



        if (Input.GetKeyDown(KeyCode.Space)) {
            owner.animator.SetTrigger("Jump");
            owner.velocity += new Vector3(0, Mathf.Sqrt(owner.jumpHeight * -2 * owner.gravity), 0);
        }

        base.OnUpdate();
    }
}