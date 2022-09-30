using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbingState : MoveState {
    public LedgeGrabbingState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public GameObject Ledge;
    public Vector3 AlongEdge;

    public override void OnEnter() {
        owner.velocity = Vector3.zero;

        owner.animator.SetBool("HangingFromEdge", true);

        Ledge = owner.evaluator.CanGrabLedge();

        Ray rayOne = new(owner.controller.transform.position + new Vector3(0, 2.2f, 0), owner.transform.forward);
        if(Physics.Raycast(rayOne, out var hit, 1f, owner.EdgeLayer)) {
            AlongEdge = hit.normal;
            AlongEdge = Vector3.Cross(AlongEdge, Vector3.up);
        }
    }

    public override void OnExit() {
        owner.animator.SetBool("HangingFromEdge", false);
    }

    public override void OnUpdate() {
        var velocity = Vector3.zero;

        var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);

        var movedir = Ledge.transform.TransformDirection(input.normalized);
        velocity += -movedir;

        if (input.x > 0) {
            owner.animator.SetBool("EdgeWalkRight", true);
            owner.animator.SetBool("EdgeWalkLeft", false);
        }
        else if (input.x < 0) {
            owner.animator.SetBool("EdgeWalkRight", false);
            owner.animator.SetBool("EdgeWalkLeft", true);
        }
        else {
            owner.animator.SetBool("EdgeWalkRight", false);
            owner.animator.SetBool("EdgeWalkLeft", false);
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            owner.animator.SetTrigger("Jump");
            owner.velocity += new Vector3(0, Mathf.Sqrt(owner.jumpHeight * -2 * owner.gravity), 0);
        }

        owner.velocity = velocity;

        base.OnUpdate();
    }
}