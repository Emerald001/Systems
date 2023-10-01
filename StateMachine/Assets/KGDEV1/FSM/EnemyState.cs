using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : State<Enemy>
{
    public EnemyState(StateMachine<Enemy> stateMachine) : base(stateMachine)
    {
    }

    public override void OnEnter()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnUpdate()
    {
        foreach (Transition<Enemy> transition in transitions)
        {
            if (transition.condition.Invoke(stateMachine.Controller))
            {
                stateMachine.SwitchState(transition.toState);
                return;
            }
        }
    }
}
