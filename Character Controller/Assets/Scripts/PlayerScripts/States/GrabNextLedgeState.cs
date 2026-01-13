using UnityEngine;

public class GrabNextLedgeState : MoveState {
    private Transform ledgeCheck;

    public GrabNextLedgeState(StateMachine<MovementManager> owner, Transform ledgeCheck) : base(owner) {
        this.ledgeCheck = ledgeCheck;
    }

    public bool isDone = false;
    private float dis;

    public override void OnEnter() {
        isDone = false;
        dis = Vector3.Distance(ledgeCheck.transform.position, ledgeCheck.transform.position);

        owner.CanGrabNextLedge = false;
        owner.Animator.SetTrigger("HangJumpUp");
    }

    public override void OnExit() {
        if (dis > 1)
            owner.ResetTimer(owner.ClimbTime);
        else
            owner.ResetTimer(owner.ClimbTime / 2);
        isDone = false;
    }

    public override void OnUpdate() {
        if ((owner.CurrentLedge.transform.position - ledgeCheck.transform.position).magnitude > .01f) {
            owner.Velocity = (owner.CurrentLedge.transform.position - ledgeCheck.transform.position).normalized * 3;

            if ((owner.CurrentLedge.transform.position - ledgeCheck.transform.position).magnitude < .2f)
                owner.Animations.HandToObject(owner.CurrentLedge, true);
        }
        else
            isDone = true;

        Vector3 desiredForward = Vector3.RotateTowards(owner.transform.forward, -owner.CurrentLedge.transform.forward, 1 * Time.deltaTime, 0f);
        desiredForward.y = 0;
        owner.transform.LookAt(owner.transform.position + desiredForward);

        base.OnUpdate();
    }
}