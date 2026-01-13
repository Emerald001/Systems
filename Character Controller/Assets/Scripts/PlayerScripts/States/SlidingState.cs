using UnityEngine;

public class SlidingState : MoveState {
    private Vector3 currentDir;
    private float slideSpeed;

    public SlidingState(StateMachine<MovementManager> owner, float slideSpeed) : base(owner) {
        this.owner = StateMachine.Owner;
        this.slideSpeed = slideSpeed;
    }

    public override void OnEnter() {
        owner.Animator.SetBool("Sliding", true);

        owner.Controller.height = 1;
        owner.Controller.center = new Vector3(0, .5f, 0);

        Vector3 input = new(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        currentDir = owner.SlopeTransform.TransformDirection(input.normalized);
    }

    public override void OnExit() {
        owner.Animator.SetBool("Sliding", false);

        owner.Controller.height = 2;
        owner.Controller.center = new Vector3(0, 1, 0);
    }

    public override void OnUpdate() {
        Vector3 downDir = Vector3.Cross(Vector3.Cross(Vector3.up, owner.Evaluator.GetSlopeNormal()), owner.Evaluator.GetSlopeNormal());

        currentDir = Vector3.Lerp(currentDir, downDir, 1f * Time.deltaTime);
        owner.Velocity = currentDir * slideSpeed;

        if (Input.GetKeyDown(KeyCode.Space)) {
            owner.Animator.SetBool("Sliding", false);
            owner.Animator.SetTrigger("Jump");
            owner.Velocity += new Vector3(0, Mathf.Sqrt(owner.JumpHeight * -2 * owner.Gravity), 0);
        }

        base.OnUpdate();
    }
}