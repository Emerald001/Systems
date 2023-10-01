using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int health = 100;
    public int Health { get => health; protected set => health = value; }

    private StateMachine<Enemy> coupledStateMachine;

    void Start()
    {
        coupledStateMachine = new StateMachine<Enemy>(this);
        GroundedState idleState = new GroundedState(coupledStateMachine);
        AirbornState attackState = new AirbornState(coupledStateMachine);

        AddTransition(idleState, KeyCode.Space, typeof(AirbornState));
        AddTransition(attackState, KeyCode.LeftShift, typeof(GroundedState));

        coupledStateMachine.AddState(typeof(GroundedState), idleState);
        coupledStateMachine.AddState(typeof(AirbornState), attackState);

        coupledStateMachine.SwitchState(typeof(GroundedState));
    }

    private void Update()
    {
        coupledStateMachine.RunUpdate();
    }

    public void AddTransition(State<Enemy> _state, KeyCode _keyCode, System.Type _stateTo)
    {
        _state.AddTransition(new Transition<Enemy>(
            (x) => {
                if (Input.GetKeyDown(_keyCode))
                    return true;
                return false;
            }, _stateTo));
    }
}
