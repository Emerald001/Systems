using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeGrabbingState : MoveState {
    public LedgeGrabbingState(StateMachine<MovementManager> owner) : base(owner) {

    }

    public GameObject Ledge;
    public Vector3 AlongEdge;

    public Vector2 MinMax;
    public float maxDis;
    private float skinWidth = .5f;

    public override void OnEnter() {
        owner.lookAtMoveDir = false;
        owner.velocity = Vector3.zero;

        Ledge = owner.CurrentLedge;

        owner.animator.SetBool("HangingFromEdge", true);

        var tmp = Ledge.transform.localScale.x / 2 - skinWidth;
        maxDis = Ledge.transform.localScale.x - skinWidth;
        MinMax = new Vector2(-tmp, tmp);
    }

    public override void OnExit() {

    }
    
    public override void OnUpdate() {
        var velocity = Vector3.zero;

        var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        var movedir = Ledge.transform.TransformDirection(input.normalized);

        var forwards = owner.transform.forward * .63f;

        var leftDis = Vector3.Distance((owner.transform.position + new Vector3(0, 1.6f, 0) + forwards), Ledge.transform.position + Ledge.transform.TransformDirection(new Vector3(Ledge.transform.localScale.x / 2, 0, 0)));
        var rightDis = Vector3.Distance((owner.transform.position + new Vector3(0, 1.6f, 0) + forwards), Ledge.transform.position - Ledge.transform.TransformDirection(new Vector3(Ledge.transform.localScale.x / 2, 0, 0)));

        if(leftDis < maxDis && input.x > 0) {
            velocity += -movedir;
            owner.animator.SetBool("EdgeWalkRight", true);
            owner.animator.SetBool("EdgeWalkLeft", false);
        }
        else if (rightDis < maxDis && input.x < 0) {
            velocity += -movedir;
            owner.animator.SetBool("EdgeWalkRight", false);
            owner.animator.SetBool("EdgeWalkLeft", true);
        }
        else {
            owner.animator.SetBool("EdgeWalkRight", false);
            owner.animator.SetBool("EdgeWalkLeft", false);
        }

        Vector3 desiredForward = Vector3.RotateTowards(owner.transform.forward, -owner.CurrentLedge.transform.forward, 1 * Time.deltaTime, 0f);
        desiredForward.y = 0;
        owner.transform.LookAt(owner.transform.position + desiredForward);

        owner.velocity = velocity;

        base.OnUpdate();
    }
}