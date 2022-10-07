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

    private GameObject Leftpointer;
    private GameObject Rightpointer;

    public override void OnEnter() {
        owner.lookAtMoveDir = false;

        owner.velocity = Vector3.zero;

        Ledge = owner.CurrentLedge;

        owner.animator.SetBool("HangingFromEdge", true);

        var tmp = Ledge.transform.localScale.x / 2 - skinWidth;
        maxDis = Ledge.transform.localScale.x - skinWidth;
        MinMax = new Vector2(-tmp, tmp);

        Leftpointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Leftpointer.transform.parent = Ledge.transform;
        Leftpointer.transform.localScale = new Vector3(.01f, .1f, .1f);

        Rightpointer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Rightpointer.transform.parent = Ledge.transform;
        Rightpointer.transform.localScale = new Vector3(.01f, .1f, .1f);
    }

    public override void OnExit() {
        Leftpointer.transform.parent = null;
        Rightpointer.transform.parent = null;
    }
    
    public override void OnUpdate() {
        var velocity = Vector3.zero;

        var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, 0);
        var movedir = Ledge.transform.TransformDirection(input.normalized);

        var forwards = owner.transform.forward * .63f;

        var leftDis = Vector3.Distance((owner.transform.position + new Vector3(0, 1.6f, 0) + forwards), new Vector3(Ledge.transform.position.x + Ledge.transform.localScale.x / 2 - skinWidth, Ledge.transform.position.y, Ledge.transform.position.z));
        var rightDis = Vector3.Distance((owner.transform.position + new Vector3(0, 1.6f, 0) + forwards), new Vector3(Ledge.transform.position.x - Ledge.transform.localScale.x / 2 + skinWidth, Ledge.transform.position.y, Ledge.transform.position.z));

        Leftpointer.transform.localPosition = new Vector3(Ledge.transform.localScale.x / 2 - skinWidth, Ledge.transform.position.y + .5f, 0);
        Rightpointer.transform.localPosition = new Vector3(-Ledge.transform.localScale.x / 2 + skinWidth, Ledge.transform.position.y + .5f, 0);

        Debug.Log("Left Dis: " + leftDis.ToString() + " Right Dis: " + rightDis.ToString());

        if(leftDis < maxDis && input.x < 0) {
            velocity += -movedir;
        }
        if (rightDis < maxDis && input.x > 0) {
            velocity += -movedir;
        }

        ////right
        //if (owner.transform.position.x > Ledge.transform.position.x + MinMax.x && input.x > 0) {
        //    velocity += -movedir;
        //}
        ////left
        //if (owner.transform.position.x < Ledge.transform.position.x + MinMax.y && input.x < 0) {
        //    velocity += -movedir;
        //}

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

        Vector3 desiredForward = Vector3.RotateTowards(owner.transform.forward, -owner.CurrentLedge.transform.forward, 1 * Time.deltaTime, 0f);
        desiredForward.y = 0;
        owner.transform.LookAt(owner.transform.position + desiredForward);

        owner.velocity = velocity;

        base.OnUpdate();
    }
}