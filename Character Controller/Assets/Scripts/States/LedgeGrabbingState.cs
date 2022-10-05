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
        owner.velocity = Vector3.zero;

        owner.animator.SetBool("HangingFromEdge", true);

        Ledge = owner.evaluator.CanGrabLedge();

        Ray rayOne = new(owner.controller.transform.position + new Vector3(0, 2.2f, 0), owner.transform.forward);
        if(Physics.Raycast(rayOne, out var hit, 1f, owner.EdgeLayer)) {
            AlongEdge = hit.normal;
            AlongEdge = Vector3.Cross(AlongEdge, Vector3.up);
        }

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

        Vector3 desiredForward = Vector3.RotateTowards(owner.transform.forward, -Ledge.transform.forward, 30 * Time.deltaTime, 0f);
        desiredForward.y = 0;
        owner.transform.LookAt(owner.transform.position + desiredForward);

        owner.velocity = velocity;

        base.OnUpdate();
    }
}