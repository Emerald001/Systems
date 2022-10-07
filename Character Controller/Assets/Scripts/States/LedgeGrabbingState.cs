using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbingState : MoveState {
    public LedgeGrabbingState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public GameObject Ledge;
    public Vector3 AlongEdge;

    public Vector2 MinMax;
    private float skinWidth = .5f;

    public override void OnEnter() {
        owner.lookAtMoveDir = false;

        owner.velocity = Vector3.zero;

        Ledge = owner.CurrentLedge;

        owner.animator.SetBool("HangingFromEdge", true);

        var tmp = Ledge.transform.localScale.x / 2 - skinWidth;
        MinMax = new Vector2(-tmp, tmp);
    }

    public override void OnExit() {

    }
    
    public override void OnUpdate() {
        var velocity = Vector3.zero;

        var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        var movedir = Ledge.transform.TransformDirection(input.normalized);

        if (owner.transform.position.x > Ledge.transform.position.x + MinMax.x && input.x > 0) {
            velocity += -movedir;
        }
        if (owner.transform.position.x < Ledge.transform.position.x + MinMax.y && input.x < 0) {
            velocity += -movedir;
        }

        if (velocity.x < 0) {
            owner.animator.SetBool("EdgeWalkRight", true);
            owner.animator.SetBool("EdgeWalkLeft", false);
        }
        else if (velocity.x > 0) {
            owner.animator.SetBool("EdgeWalkRight", false);
            owner.animator.SetBool("EdgeWalkLeft", true);
        }
        else {
            owner.animator.SetBool("EdgeWalkRight", false);
            owner.animator.SetBool("EdgeWalkLeft", false);
        }

        owner.velocity = velocity;

        base.OnUpdate();
    }
}