public abstract class MoveState : State<MovementManager> {
    public MovementManager owner;

    public MoveState(StateMachine<MovementManager> owner) : base(owner) {
        this.owner = StateMachine.Owner;
    }

    public override void OnUpdate() {
        foreach (var transition in transitions) {
            if (transition.condition.Invoke(owner)) {
                StateMachine.ChangeState(transition.toState);
                return;
            }
        }
    }
}