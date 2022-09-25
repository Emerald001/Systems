using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionMethods : MonoBehaviour
{
    private StateMachine<TransitionMethods> StateMachine;

    public void Awake() {
        //make a new instance of a state machine.
        StateMachine = new(this);

        //make a new instance of the state
        var exampleState = new ExampleState(StateMachine);
        //add made state to the list with its type.
        StateMachine.AddState(typeof(ExampleState), exampleState);

        //transition with key, press key in that state and it transitions to the assigned one
        AddTransitionWithKey(exampleState, KeyCode.Space, typeof(ExampleState));
        //If the bool, a reference to one, changes while in that state and it transitions to the assigned one, cannot do a method, it will just use the output from when it was assigned, not in realtime
        AddTransitionWithBool(exampleState, true, typeof(ExampleState));
        //If the prediquete is true while in that state and it transitions to the assigned one
        AddTransitionWithPrediquete(exampleState, (x) => { return Input.GetKeyDown(KeyCode.Space); }, typeof(ExampleState));
    }

    public void AddTransitionWithKey(State<TransitionMethods> state, KeyCode keyCode, System.Type stateTo) {
        state.AddTransition(new Transition<TransitionMethods>(
            (x) => {
                if (Input.GetKeyDown(keyCode)) {
                    return true;
                }
                return false;
            }, stateTo));
    }

    public void AddTransitionWithBool(State<TransitionMethods> state, bool check, System.Type stateTo) {
        state.AddTransition(new Transition<TransitionMethods>(
            (x) => {
                if (check)
                    return true;
                return false;
            }, stateTo));
    }

    public void AddTransitionWithPrediquete(State<TransitionMethods> state, System.Predicate<TransitionMethods> predicate, System.Type stateTo) {
        state.AddTransition(new Transition<TransitionMethods>(predicate, stateTo));
    }
}