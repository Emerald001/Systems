using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class MovementManager : MonoBehaviour {
    [Header("Objects Needed")]
    [SerializeField] private GameObject visuals;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private Transform ledgeCheck;
    [SerializeField] private Transform slopeTransform;
    [SerializeField] private Transform yRotationParent;

    [Header("World Settings")]
    [SerializeField] private float gravity = -19.62f;
    [SerializeField] private float airDrag = 1;
    [SerializeField] private float acceleration;

    [Header("Player Settings")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float airbornSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float slideSpeed;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private int jumpAmount;
    [SerializeField] private float spherecheckRadius;
    [SerializeField] private float climbTime;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private float armSpeed;

    [SerializeField] private Transform leftArmTarget;
    [SerializeField] private TwoBoneIKConstraint leftArm;
    [SerializeField] private Transform rightArmTarget;
    [SerializeField] private TwoBoneIKConstraint rightArm;

    [SerializeField] private Transform leftFootTarget;
    [SerializeField] private TwoBoneIKConstraint leftLeg;
    [SerializeField] private Transform rightFootTarget;
    [SerializeField] private TwoBoneIKConstraint rightLeg;

    public Animator Animator => animator;
    public Transform SlopeTransform => slopeTransform;
    public Transform YRotationParent => yRotationParent;

    public AnimationManager Animations { get; private set; }
    public MovementEvaluator Evaluator { get; private set; }
    public GameObject CurrentLedge { get; set; } = null;
    public CharacterController Controller { get; private set; }

    public Vector3 Velocity { get; set; }

    // change
    public bool Sprinting { get; set; }
    public bool CanGrabNextLedge { get; set; } = true;
    public bool LookAtMoveDir { get; set; } = true;

    public float Gravity => gravity;
    public float Acceleration => acceleration;
    public float JumpHeight => jumpHeight;
    public float ClimbTime => climbTime;

    private StateMachine<MovementManager> movementStateMachine;

    private void Start() {
        Controller = GetComponent<CharacterController>();

        CanGrabNextLedge = true;

        movementStateMachine = new(this);
        Animations = new(this, animator) {
            speed = armSpeed,

            LeftFootTarget = leftFootTarget,
            RightFootTarget = rightFootTarget,
            LeftArmTarget = leftArmTarget,
            RightArmTarget = rightArmTarget,

            leftArm = leftArm,
            rightArm = rightArm,
            leftLeg = leftLeg,
            rightLeg = rightLeg
        };

        Evaluator = new(this, ledgeCheck);

        var groundedState = new GroundedState(movementStateMachine, walkSpeed);
        movementStateMachine.AddState(typeof(GroundedState), groundedState);
        AddTransitionWithKey(groundedState, KeyCode.Space, typeof(AirbornState));
        AddTransitionWithPrediquete(groundedState, (x) => { return !Evaluator.IsGrounded(); }, typeof(AirbornState));
        AddTransitionWithBool(groundedState, !Evaluator.IsGrounded(), typeof(AirbornState));
        AddTransitionWithKey(groundedState, KeyCode.LeftControl, typeof(CrouchingState));
        AddTransitionWithKey(groundedState, KeyCode.LeftShift, typeof(SprintingState));
        AddTransitionWithPrediquete(groundedState, (x) => {
            var tmp = Evaluator.CollectableNearby();
            if (tmp != null) {
                Animations.HandToObject(tmp, false);
                if (Input.GetKeyDown(KeyCode.E))
                    return true;
            }
            else
                Animations.ResetIK();
            return false;
        }, typeof(InteractionState));

        var airbornState = new AirbornState(movementStateMachine, airbornSpeed, airDrag, maxSpeed, jumpAmount);
        movementStateMachine.AddState(typeof(AirbornState), airbornState);
        AddTransitionWithPrediquete(airbornState, (x) => { return Evaluator.IsGrounded() && !Sprinting; }, typeof(GroundedState));
        AddTransitionWithPrediquete(airbornState, (x) => { return Evaluator.IsGrounded() && Sprinting; }, typeof(SprintingState));
        AddTransitionWithPrediquete(airbornState, (x) => {
            var tmp = Evaluator.GetLedge(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0), 0, spherecheckRadius);
            if (tmp != null && CanGrabNextLedge) {
                CurrentLedge = tmp;
                return true;
            }
            return false;
        }, typeof(GrabNextLedgeState));

        var crouchingState = new CrouchingState(movementStateMachine, crouchSpeed);
        AddTransitionWithKey(crouchingState, KeyCode.Space, typeof(AirbornState));
        AddTransitionWithPrediquete(crouchingState, (x) => { return !Evaluator.IsGrounded(); }, typeof(AirbornState));
        movementStateMachine.AddState(typeof(CrouchingState), crouchingState);
        AddTransitionWithPrediquete(crouchingState, (x) => {
            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                if (!Evaluator.TouchedRoof()) {
                    return true;
                }
            }
            return false;
        }, typeof(GroundedState));
        AddTransitionWithPrediquete(crouchingState, (x) => {
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                if (!Evaluator.TouchedRoof()) {
                    return true;
                }
            }
            return false;
        }, typeof(SprintingState));

        var slidingState = new SlidingState(movementStateMachine, slideSpeed);
        movementStateMachine.AddState(typeof(SlidingState), slidingState);
        AddTransitionWithKey(slidingState, KeyCode.Space, typeof(AirbornState));
        AddTransitionWithPrediquete(slidingState, (x) => { return !Evaluator.IsGrounded(); }, typeof(AirbornState));
        AddTransitionWithPrediquete(slidingState, (x) => { return Velocity.magnitude < 5f; }, typeof(CrouchingState));
        AddTransitionWithPrediquete(slidingState, (x) => {
            if (Input.GetKeyUp(KeyCode.LeftControl))
                if (Evaluator.TouchedRoof()) {
                    return true;
                }
            return false;
        }, typeof(CrouchingState));
        AddTransitionWithPrediquete(slidingState, (x) => {
            if (Input.GetKeyUp(KeyCode.LeftControl))
                if (!Evaluator.TouchedRoof()) {
                    return true;
                }
            return false;
        }, typeof(SprintingState));

        var sprintingState = new SprintingState(movementStateMachine, runSpeed);
        movementStateMachine.AddState(typeof(SprintingState), sprintingState);
        AddTransitionWithKey(sprintingState, KeyCode.Space, typeof(AirbornState));
        AddTransitionWithKey(sprintingState, KeyCode.LeftControl, typeof(SlidingState));
        AddTransitionWithPrediquete(sprintingState, (x) => { return !Evaluator.IsGrounded(); }, typeof(AirbornState));
        AddTransitionWithPrediquete(sprintingState, (x) => { if (Input.GetAxisRaw("Vertical") <= 0) { Sprinting = false; return true; } return false; }, typeof(GroundedState));
        AddTransitionWithPrediquete(sprintingState, (x) => { if (Input.GetKeyDown(KeyCode.LeftShift)) { Sprinting = false; return true; } return false; }, typeof(GroundedState));

        var wallLatchState = new LedgeGrabbingState(movementStateMachine);
        movementStateMachine.AddState(typeof(LedgeGrabbingState), wallLatchState);
        AddTransitionWithKey(wallLatchState, KeyCode.C, typeof(AirbornState));
        AddTransitionWithPrediquete(wallLatchState, (x) => { return Input.GetKey(KeyCode.W) && Evaluator.CanGoOntoLedge() != Vector3.zero; }, typeof(GetUpOnPlatformState));
        AddTransitionWithPrediquete(wallLatchState, (x) => {
            var tmp = Evaluator.GetLedge(new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0), 1, spherecheckRadius);
            if (tmp != null && CanGrabNextLedge) {
                CurrentLedge = tmp;
                return true;
            }
            return false;
        }, typeof(GrabNextLedgeState));

        var wallClimbState = new GrabNextLedgeState(movementStateMachine, ledgeCheck);
        movementStateMachine.AddState(typeof(GrabNextLedgeState), wallClimbState);
        AddTransitionWithPrediquete(wallClimbState, (x) => { return wallClimbState.isDone; }, typeof(LedgeGrabbingState));

        var goOntoPlatformState = new GetUpOnPlatformState(movementStateMachine);
        movementStateMachine.AddState(typeof(GetUpOnPlatformState), goOntoPlatformState);
        AddTransitionWithPrediquete(goOntoPlatformState, (x) => { return goOntoPlatformState.IsDone; }, typeof(AirbornState));

        var interactionState = new InteractionState(movementStateMachine);
        movementStateMachine.AddState(typeof(InteractionState), interactionState);

        movementStateMachine.ChangeState(typeof(GroundedState));
    }

    private void Update() {
        slopeTransform.rotation = Quaternion.FromToRotation(slopeTransform.up, Evaluator.GetSlopeNormal()) * slopeTransform.rotation;
        slopeTransform.localEulerAngles = new Vector3(slopeTransform.localEulerAngles.x, 0, slopeTransform.localEulerAngles.z);

        movementStateMachine.OnUpdate();
        Animations.OnUpdate();

        Controller.Move(Velocity * Time.deltaTime);

        if (Velocity.magnitude > 1 && LookAtMoveDir) {
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, Velocity.normalized, 30 * Time.deltaTime, 0f);
            desiredForward.y = 0;
            transform.LookAt(transform.position + desiredForward);
        }
    }

    private void AddTransitionWithKey(State<MovementManager> state, KeyCode keyCode, System.Type stateTo) {
        state.AddTransition(new Transition<MovementManager>(
            (x) => {
                if (Input.GetKeyDown(keyCode)) {
                    return true;
                }
                return false;
            }, stateTo));
    }

    private void AddTransitionWithBool(State<MovementManager> state, bool check, System.Type stateTo) {
        state.AddTransition(new Transition<MovementManager>(
            (x) => {
                if (check)
                    return true;
                return false;
            }, stateTo));
    }

    private void AddTransitionWithPrediquete(State<MovementManager> state, System.Predicate<MovementManager> predicate, System.Type stateTo) {
        state.AddTransition(new Transition<MovementManager>(predicate, stateTo));
    }

    public void ResetTimer(float time) => StartCoroutine(Timer(time));

    private IEnumerator Timer(float time) {
        yield return new WaitForSeconds(time);
        CanGrabNextLedge = true;
    }
}