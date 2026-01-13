using UnityEngine;

public class SprintingState : MoveState {
    private float sprintSpeed;

    public SprintingState(StateMachine<MovementManager> owner, float sprintSpeed) : base(owner) {
        this.owner = StateMachine.Owner;
        this.sprintSpeed = sprintSpeed;
    }

    public override void OnEnter() {
        owner.Animator.SetBool("Sprinting", true);

        owner.Sprinting = true;
    }

    public override void OnExit() {
        owner.Animator.SetBool("Sprinting", false);
    }

    public override void OnUpdate() {
        Vector3 input = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 movedir = owner.SlopeTransform.TransformDirection(input.normalized);
        Vector3 velocity = movedir * sprintSpeed;

        if (input.magnitude == 0)
            owner.Animator.SetBool("Walking", false);

        if (Input.GetKeyDown(KeyCode.Space)) {
            owner.Animator.SetTrigger("Jump");
            owner.Velocity += new Vector3(0, Mathf.Sqrt(owner.JumpHeight * -2 * owner.Gravity), 0);
        }

        owner.Velocity = Vector3.MoveTowards(owner.Velocity, velocity, owner.Acceleration * Time.deltaTime);

        base.OnUpdate();
    }
}