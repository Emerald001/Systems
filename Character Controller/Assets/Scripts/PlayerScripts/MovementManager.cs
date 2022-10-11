using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class MovementManager : MonoBehaviour
{
    public Vector3 velocity;

    //StateMachine
    private StateMachine<MovementManager> movementStateMachine;

    [Header("Objects Needed")]
    public Animator animator;
    public GameObject Visuals;
    public GameObject GroundCheck;
    public Transform SlopeTransform;
    public Transform YRotationParent;
    public LayerMask GroundLayer;
    public LayerMask EdgeLayer;
    public GameObject DebugObject;

    [HideInInspector] public CharacterController controller;
    [HideInInspector] public MovementEvaluator evaluator;
    [HideInInspector] public AnimationManager animations;

    [Header("World Settings")]
    public float gravity = -19.62f;
    public float airDrag = 10;
    public float Acceleration;

    [Header("Player Settings")]
    public float maxSpeed;
    public float speed;
    public float airbornSpeed;
    public float runSpeed;
    public float slideSpeed;
    public float crouchSpeed;
    public float jumpHeight;
    public int jumpAmount;
    public float climbDistance;
    public float leapClimbDistance;

    //Keep Track of Info
    [HideInInspector] public Vector3 CurrentDirection; 
    [HideInInspector] public Vector3 GroundAngle;
    [HideInInspector] public bool sprinting;
    [HideInInspector] public bool lookAtMoveDir = true;
    [HideInInspector] public GameObject CurrentLedge = null;

    void Start() {
        controller = GetComponent<CharacterController>();

        movementStateMachine = new(this);
        animations = new(animator);

        evaluator = new();
        evaluator.owner = this;

        var groundedState = new GroundedState(movementStateMachine);
        movementStateMachine.AddState(typeof(GroundedState), groundedState);
        AddTransitionWithKey(groundedState, KeyCode.Space, typeof(AirbornState));
        AddTransitionWithPrediquete(groundedState, (x) => { return !evaluator.IsGrounded(); }, typeof(AirbornState));
        AddTransitionWithBool(groundedState, !evaluator.IsGrounded(), typeof(AirbornState));
        AddTransitionWithKey(groundedState, KeyCode.LeftControl, typeof(CrouchingState));
        AddTransitionWithKey(groundedState, KeyCode.LeftShift, typeof(SprintingState));

        var airbornState = new AirbornState(movementStateMachine);
        movementStateMachine.AddState(typeof(AirbornState), airbornState);
        AddTransitionWithPrediquete(airbornState, (x) => { return evaluator.IsGrounded() && !sprinting; }, typeof(GroundedState));
        AddTransitionWithPrediquete(airbornState, (x) => { return evaluator.IsGrounded() && sprinting; }, typeof(SprintingState));
        AddTransitionWithPrediquete(airbornState, (x) => { return evaluator.CanGrabLedge() != null && Input.GetKey(KeyCode.W); }, typeof(LedgeGrabbingState));

        var crouchingState = new CrouchingState(movementStateMachine);
        AddTransitionWithKey(crouchingState, KeyCode.Space, typeof(AirbornState));
        AddTransitionWithPrediquete(crouchingState, (x) => { return !evaluator.IsGrounded(); }, typeof(AirbornState));
        movementStateMachine.AddState(typeof(CrouchingState), crouchingState);
        AddTransitionWithPrediquete(crouchingState, (x) => {
            if (Input.GetKeyDown(KeyCode.LeftControl)) {
                if (!evaluator.TouchedRoof()) {
                    return true;
                }
            }
            return false;
        }, typeof(GroundedState));
        AddTransitionWithPrediquete(crouchingState, (x) => {
            if (Input.GetKeyDown(KeyCode.LeftShift)) {
                if (!evaluator.TouchedRoof()) {
                    return true;
                }
            }
            return false;
        }, typeof(SprintingState));

        var slidingState = new SlidingState(movementStateMachine);
        movementStateMachine.AddState(typeof(SlidingState), slidingState);
        AddTransitionWithKey(slidingState, KeyCode.Space, typeof(AirbornState));
        AddTransitionWithPrediquete(slidingState, (x) => { return !evaluator.IsGrounded(); }, typeof(AirbornState));
        AddTransitionWithPrediquete(slidingState, (x) => { return velocity.magnitude < 5f; }, typeof(CrouchingState));
        AddTransitionWithPrediquete(slidingState, (x) => {
            if (Input.GetKeyUp(KeyCode.LeftControl)) 
                if (evaluator.TouchedRoof()) {
                    return true;
                }
            return false;
        }, typeof(CrouchingState));
        AddTransitionWithPrediquete(slidingState, (x) => {
            if (Input.GetKeyUp(KeyCode.LeftControl)) 
                if (!evaluator.TouchedRoof()) {
                    return true;
                }
            return false;
        }, typeof(SprintingState));

        var sprintingState = new SprintingState(movementStateMachine);
        movementStateMachine.AddState(typeof(SprintingState), sprintingState);
        AddTransitionWithKey(sprintingState, KeyCode.Space, typeof(AirbornState));
        AddTransitionWithKey(sprintingState, KeyCode.LeftControl, typeof(SlidingState));
        AddTransitionWithPrediquete(sprintingState, (x) => { return !evaluator.IsGrounded(); }, typeof(AirbornState));
        AddTransitionWithPrediquete(sprintingState, (x) => { if (Input.GetAxisRaw("Vertical") <= 0) { sprinting = false; return true; } return false; }, typeof(GroundedState));
        AddTransitionWithPrediquete(sprintingState, (x) => { if (Input.GetKeyDown(KeyCode.LeftShift)) { sprinting = false; return true; } return false; }, typeof(GroundedState));

        var wallLatchState = new LedgeGrabbingState(movementStateMachine);
        movementStateMachine.AddState(typeof(LedgeGrabbingState), wallLatchState);
        AddTransitionWithKey(wallLatchState, KeyCode.C, typeof(AirbornState));
        AddTransitionWithPrediquete(wallLatchState, (x) => { return Input.GetKey(KeyCode.W) && evaluator.CanGoOntoLedge() != Vector3.zero; }, typeof(GetUpOnPlatformState));
        AddTransitionWithPrediquete(wallLatchState, (x) => { return Input.GetKey(KeyCode.W) && evaluator.CanGrabLedge(Vector3.up); }, typeof(GrabNextLedgeState));
        AddTransitionWithPrediquete(wallLatchState, (x) => { return Input.GetKey(KeyCode.W) && Input.GetKeyDown(KeyCode.Space) && evaluator.CanGrabLedgeLeap(Vector3.up); }, typeof(LeapGrabNextLedgeState));
        AddTransitionWithPrediquete(wallLatchState, (x) => { return Input.GetKey(KeyCode.S) && evaluator.CanGrabLedge(Vector3.down); }, typeof(GrabNextLedgeState));
        AddTransitionWithPrediquete(wallLatchState, (x) => { return Input.GetKey(KeyCode.S) && Input.GetKeyDown(KeyCode.Space) && evaluator.CanGrabLedgeLeap(Vector3.down); }, typeof(LeapGrabNextLedgeState));
        AddTransitionWithPrediquete(wallLatchState, (x) => { 
            if (transform.position.x < CurrentLedge.transform.position.x - CurrentLedge.transform.localScale.x / 2 - .5f || transform.position.x > CurrentLedge.transform.position.x + CurrentLedge.transform.localScale.x / 2 - .5f) {
                if (evaluator.LookAroundCorner())
                    return true;
                }
            return false;
        }, typeof(GetUpOnPlatformState));

        var wallClimbState = new GrabNextLedgeState(movementStateMachine);
        movementStateMachine.AddState(typeof(GrabNextLedgeState), wallClimbState);
        AddTransitionWithPrediquete(wallClimbState, (x) => { return wallClimbState.isDone; }, typeof(LedgeGrabbingState));

        var leapWallState = new LeapGrabNextLedgeState(movementStateMachine);
        movementStateMachine.AddState(typeof(LeapGrabNextLedgeState), leapWallState);
        AddTransitionWithPrediquete(leapWallState, (x) => { return leapWallState.isDone; }, typeof(LedgeGrabbingState));

        var goOntoPlatformState = new GetUpOnPlatformState(movementStateMachine);
        movementStateMachine.AddState(typeof(GetUpOnPlatformState), goOntoPlatformState);
        AddTransitionWithPrediquete(goOntoPlatformState, (x) => { return goOntoPlatformState.IsDone; }, typeof(AirbornState));

        var goAroundCornerState = new GoAroundCornerState(movementStateMachine);
        movementStateMachine.AddState(typeof(GoAroundCornerState), goAroundCornerState);
        AddTransitionWithPrediquete(goAroundCornerState, (x) => { return goAroundCornerState.IsDone; }, typeof(LedgeGrabbingState));

        movementStateMachine.ChangeState(typeof(GroundedState));
    }

    void Update() {
        movementStateMachine.OnUpdate();

        SlopeTransform.rotation = Quaternion.FromToRotation(SlopeTransform.up, evaluator.GetSlopeNormal()) * SlopeTransform.rotation;
        SlopeTransform.localEulerAngles = new Vector3(SlopeTransform.localEulerAngles.x, 0, SlopeTransform.localEulerAngles.z);

        controller.Move(velocity * Time.deltaTime);

        if(velocity.magnitude > 1 && lookAtMoveDir) {
            Vector3 desiredForward = Vector3.RotateTowards(transform.forward, velocity.normalized, 30 * Time.deltaTime, 0f);
            desiredForward.y = 0;
            transform.LookAt(transform.position + desiredForward);
        }
    }

    public void AddTransitionWithKey(State<MovementManager> state, KeyCode keyCode, System.Type stateTo) {
        state.AddTransition(new Transition<MovementManager>(
            (x) => {
                if (Input.GetKeyDown(keyCode)) {
                    return true;
                }
                return false;
            }, stateTo));
    }

    public void AddTransitionWithBool(State<MovementManager> state, bool check, System.Type stateTo) {
        state.AddTransition(new Transition<MovementManager>(
            (x) => {
                if (check)
                    return true;
                return false;
            }, stateTo));
    }

    public void AddTransitionWithPrediquete(State<MovementManager> state, System.Predicate<MovementManager> predicate, System.Type stateTo) {
        state.AddTransition(new Transition<MovementManager>(predicate, stateTo));
    }
}