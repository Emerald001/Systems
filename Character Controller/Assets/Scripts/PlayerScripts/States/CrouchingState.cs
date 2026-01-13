using UnityEngine;

public class CrouchingState : MoveState {
    private readonly float crouchingSpeed;

    public CrouchingState(StateMachine<MovementManager> owner, float crouchingSpeed) : base(owner) {
        this.owner = StateMachine.Owner;
        this.crouchingSpeed = crouchingSpeed;
    }

    public override void OnEnter() {
        owner.Controller.height = 1;
        owner.Controller.center = new Vector3(0, .5f, 0);

        owner.Animator.SetBool("Crouching", true);
    }

    public override void OnExit() {
        owner.Controller.height = 2;
        owner.Controller.center = new Vector3(0, 1, 0);

        owner.Animator.SetBool("Crouching", false);
    }

    public override void OnUpdate() {
        Vector3 velocity = Vector3.zero;
        Vector3 input = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 movedir = owner.SlopeTransform.TransformDirection(input.normalized);
        velocity += movedir * crouchingSpeed;

        owner.Animator.SetBool("Walking", input.magnitude > 0);
        
        if (Input.GetKeyDown(KeyCode.Space)) {
            owner.Animator.SetTrigger("Jump");
            owner.Velocity += new Vector3(0, Mathf.Sqrt(owner.JumpHeight * -2 * owner.Gravity), 0);
        }

        owner.Velocity = Vector3.MoveTowards(owner.Velocity, velocity, owner.Acceleration * Time.deltaTime);

        base.OnUpdate();
    }
}