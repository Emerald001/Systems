using UnityEngine;

public class GroundedState : MoveState {
    private float walkSpeed;

    public GroundedState(StateMachine<MovementManager> owner, float walkSpeed) : base(owner) {
        this.owner = StateMachine.Owner;
        this.walkSpeed = walkSpeed;
    }

    public override void OnEnter() {
        owner.LookAtMoveDir = true;
    }

    public override void OnExit() { }

    public override void OnUpdate() {
        Vector3 velocity = Vector3.zero;
        Vector3 input = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 movedir = owner.SlopeTransform.TransformDirection(input.normalized);
        velocity += movedir * walkSpeed;

        owner.Animator.SetBool("Walking", input.magnitude > 0);

        if (Input.GetKeyDown(KeyCode.Space)) {
            owner.Animator.SetTrigger("Jump");
            owner.Velocity += new Vector3(0, Mathf.Sqrt(owner.JumpHeight * -2 * owner.Gravity), 0);
        }

        owner.Velocity = Vector3.MoveTowards(owner.Velocity, velocity, owner.Acceleration * Time.deltaTime);

        base.OnUpdate();
    }
}