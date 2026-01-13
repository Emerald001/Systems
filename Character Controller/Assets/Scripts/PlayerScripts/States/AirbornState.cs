using UnityEngine;

public class AirbornState : MoveState {
    private float airbornSpeed;
    private float airDrag;
    private float maxSpeed;

    private int jumpAmount;
    private int doubleJumps;

    public AirbornState(StateMachine<MovementManager> owner, float airbornSpeed, float airDrag, float maxSpeed, int jumpAmount) : base(owner) {
        this.owner = StateMachine.Owner;
        this.airbornSpeed = airbornSpeed;
        this.airDrag = airDrag;
        this.maxSpeed = maxSpeed;
        this.jumpAmount = jumpAmount;
    }

    public override void OnEnter() {
        owner.LookAtMoveDir = true;

        owner.Animations.ResetIK();

        owner.Animator.SetBool("HangingFromEdge", false);
        owner.Animator.SetBool("Falling", true);
        doubleJumps = jumpAmount;
    }

    public override void OnExit() {
        owner.Animator.SetBool("Falling", false);
        owner.Animator.SetTrigger("TouchedGround");
    }

    public override void OnUpdate() {
        Vector3 newVelocity = owner.Velocity;
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        newVelocity.x *= airDrag;
        newVelocity.z *= airDrag;

        newVelocity += (owner.YRotationParent.right * input.x + owner.YRotationParent.forward * input.z) * airbornSpeed;
        newVelocity.x = Mathf.Clamp(newVelocity.x, -maxSpeed, maxSpeed);
        newVelocity.y += owner.Gravity * Time.deltaTime;
        newVelocity.z = Mathf.Clamp(newVelocity.z, -maxSpeed, maxSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && doubleJumps > 0) {
            newVelocity += new Vector3(0, Mathf.Sqrt((owner.JumpHeight / 2) * -2 * owner.Gravity), 0);
            doubleJumps--;
        }

        if (owner.Evaluator.TouchedRoof() && newVelocity.y > 0)
            newVelocity.y = 0;

        owner.Velocity = newVelocity;
        base.OnUpdate();
    }
}