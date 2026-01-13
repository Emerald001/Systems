using System.Collections.Generic;

public abstract class State<T> {
    public abstract void OnEnter();
    public abstract void OnUpdate();
    public abstract void OnExit();

    public StateMachine<T> StateMachine { get; protected set; }
    public List<Transition<T>> transitions = new();

    public State(StateMachine<T> owner) {
        this.StateMachine = owner;
    }

    public virtual void AddTransition(Transition<T> transition) {
        transitions.Add(transition);
    }
}